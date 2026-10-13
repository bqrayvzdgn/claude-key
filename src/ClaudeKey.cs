// ClaudeKey - Opens the Claude desktop app when a chosen keyboard key is pressed.
//
// Usage:
//   ClaudeKey.exe copilot   Copilot key (sent to Windows as Win + Shift + F23)
//   ClaudeKey.exe menu      Context Menu (Apps) key
//
// Runs in the background with no window. The chosen key is swallowed, so
// Copilot or the context menu does not open; only Claude does.

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

internal static class ClaudeKey
{
    // --- Win32 constants ---
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_SYSKEYDOWN = 0x0104;
    private const uint KEYEVENTF_KEYUP = 0x0002;

    private const int VK_F23 = 0x86;     // Copilot key
    private const int VK_APPS = 0x5D;    // Context Menu key
    private const byte VK_MASK = 0xE8;   // Unassigned key: keeps the Start menu from opening when Win is released

    // --- Claude locations ---
    private const string MsixAppId = @"shell:AppsFolder\Claude_pzs8sxrjxfjjc!Claude";
    private const string ClassicInstallPath = @"AnthropicClaude\claude.exe";

    private const string MutexName = "ClaudeKey_SingleInstance";
    private static readonly TimeSpan RepeatGuard = TimeSpan.FromSeconds(1);

    private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

    // Kept in a field so the garbage collector does not free the callback
    private static readonly HookProc Callback = KeyboardHook;
    private static IntPtr hook = IntPtr.Zero;
    private static int targetKey = VK_F23;
    private static DateTime lastLaunch = DateTime.MinValue;

    [STAThread]
    private static void Main(string[] args)
    {
        targetKey = ParseKey(args.Length > 0 ? args[0] : "copilot");

        bool isFirstInstance;
        using (new Mutex(true, MutexName, out isFirstInstance))
        {
            if (!isFirstInstance) return;   // Already running

            hook = SetWindowsHookEx(WH_KEYBOARD_LL, Callback, GetModuleHandle(null), 0);
            if (hook == IntPtr.Zero) return;

            Application.Run();
            UnhookWindowsHookEx(hook);
        }
    }

    private static int ParseKey(string name)
    {
        switch (name.Trim().ToLowerInvariant())
        {
            case "menu":
            case "apps":
            case "context":
                return VK_APPS;
            default:
                return VK_F23;
        }
    }

    private static IntPtr KeyboardHook(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && Marshal.ReadInt32(lParam) == targetKey)
        {
            int message = wParam.ToInt32();
            if (message == WM_KEYDOWN || message == WM_SYSKEYDOWN)
            {
                if (targetKey == VK_F23) SuppressStartMenu();

                // Don't relaunch Claude repeatedly while the key is held down
                if (DateTime.Now - lastLaunch > RepeatGuard)
                {
                    lastLaunch = DateTime.Now;
                    LaunchClaude();
                }
            }
            return (IntPtr)1;   // Swallow the key
        }
        return CallNextHookEx(hook, nCode, wParam, lParam);
    }

    // The Copilot key arrives together with Win, so an unassigned key press is
    // injected to stop the Start menu from opening when Win is released.
    private static void SuppressStartMenu()
    {
        keybd_event(VK_MASK, 0, 0, UIntPtr.Zero);
        keybd_event(VK_MASK, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
    }

    // Launches the classic install if present, otherwise the Microsoft Store (MSIX) version.
    private static void LaunchClaude()
    {
        string classic = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            ClassicInstallPath);
        try
        {
            if (File.Exists(classic)) Process.Start(classic);
            else Process.Start("explorer.exe", MsixAppId);
        }
        catch
        {
            // Ignore launch failures so the background process keeps running
        }
    }
}
