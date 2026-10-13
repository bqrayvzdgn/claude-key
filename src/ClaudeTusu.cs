// ClaudeTusu - Secilen klavye tusuna basildiginda Claude masaustu uygulamasini acar.
//
// Kullanim:
//   ClaudeTusu.exe copilot   Copilot tusu (Windows'a Win + Shift + F23 olarak gelir)
//   ClaudeTusu.exe menu      Baglam menusu (Context Menu / Apps) tusu
//
// Program pencere acmadan arka planda calisir. Secilen tus Windows'a iletilmez,
// bu yuzden Copilot ya da sag tik menusu acilmaz; yalnizca Claude acilir.

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

internal static class ClaudeTusu
{
    // --- Win32 sabitleri ---
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_SYSKEYDOWN = 0x0104;
    private const uint KEYEVENTF_KEYUP = 0x0002;

    private const int VK_F23 = 0x86;     // Copilot tusu
    private const int VK_APPS = 0x5D;    // Baglam menusu tusu
    private const byte VK_MASK = 0xE8;   // Atanmamis tus: Win birakilinca Baslat menusunun acilmasini onler

    // --- Claude konumlari ---
    private const string MsixUygulamaKimligi = @"shell:AppsFolder\Claude_pzs8sxrjxfjjc!Claude";
    private const string KlasikKurulumYolu = @"AnthropicClaude\claude.exe";

    private const string MutexAdi = "ClaudeTusu_TekKopya";
    private static readonly TimeSpan TekrarEngelSuresi = TimeSpan.FromSeconds(1);

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

    // Cop toplayicinin geri cagirmayi silmemesi icin alanda tutuluyor
    private static readonly HookProc Geri = KlavyeKancasi;
    private static IntPtr kanca = IntPtr.Zero;
    private static int hedefTus = VK_F23;
    private static DateTime sonAcilis = DateTime.MinValue;

    [STAThread]
    private static void Main(string[] args)
    {
        hedefTus = TusSec(args.Length > 0 ? args[0] : "copilot");

        bool ilkKopya;
        using (new Mutex(true, MutexAdi, out ilkKopya))
        {
            if (!ilkKopya) return;   // Zaten calisiyor

            kanca = SetWindowsHookEx(WH_KEYBOARD_LL, Geri, GetModuleHandle(null), 0);
            if (kanca == IntPtr.Zero) return;

            Application.Run();
            UnhookWindowsHookEx(kanca);
        }
    }

    private static int TusSec(string ad)
    {
        switch (ad.Trim().ToLowerInvariant())
        {
            case "menu":
            case "apps":
            case "context":
                return VK_APPS;
            default:
                return VK_F23;
        }
    }

    private static IntPtr KlavyeKancasi(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && Marshal.ReadInt32(lParam) == hedefTus)
        {
            int mesaj = wParam.ToInt32();
            if (mesaj == WM_KEYDOWN || mesaj == WM_SYSKEYDOWN)
            {
                if (hedefTus == VK_F23) BaslatMenusunuEngelle();

                // Tus basili tutulursa Claude tekrar tekrar acilmasin
                if (DateTime.Now - sonAcilis > TekrarEngelSuresi)
                {
                    sonAcilis = DateTime.Now;
                    ClaudeAc();
                }
            }
            return (IntPtr)1;   // Tusu Windows'a iletme
        }
        return CallNextHookEx(kanca, nCode, wParam, lParam);
    }

    // Copilot tusu Win tusuyla birlikte geldigi icin, Win birakildiginda
    // Baslat menusu acilmasin diye araya atanmamis bir tus basimi eklenir.
    private static void BaslatMenusunuEngelle()
    {
        keybd_event(VK_MASK, 0, 0, UIntPtr.Zero);
        keybd_event(VK_MASK, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
    }

    // Klasik kurulum varsa onu, yoksa Microsoft Store (MSIX) surumunu acar.
    private static void ClaudeAc()
    {
        string klasik = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            KlasikKurulumYolu);
        try
        {
            if (File.Exists(klasik)) Process.Start(klasik);
            else Process.Start("explorer.exe", MsixUygulamaKimligi);
        }
        catch
        {
            // Acilamazsa sessizce gec; arka plan programi cokmesin
        }
    }
}
