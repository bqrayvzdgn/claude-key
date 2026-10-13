# claude-key

A tiny Windows utility that opens the **Claude** desktop app when you press the **Copilot** key or the **Context Menu** key, instead of Copilot.

- Nothing extra to install: it is compiled with the C# compiler that ships with Windows (.NET Framework 4).
- Runs in the background with no window and uses roughly 10–15 MB of memory.
- The chosen key is swallowed, so Copilot or the context menu does not open; only Claude does.
- Works with both the Microsoft Store (MSIX) and the classic install of Claude.

## Installation

1. Download the repository (**Code → Download ZIP**) and extract it anywhere (don't run it from inside the ZIP).
2. Double-click the file that matches your keyboard:
   - `install-copilot-key.vbs`
   - `install-menu-key.vbs`
3. When the "Installation finished" message appears, press the key to try it.

The installer compiles the program into `%LOCALAPPDATA%\ClaudeKey`, adds it to Windows startup and starts it right away.
After that, the downloaded folder is no longer needed and can be deleted.
If anything goes wrong, details are written to `%LOCALAPPDATA%\ClaudeKey\install_log.txt`.

## Uninstall

Double-click `uninstall.vbs`, or run `%LOCALAPPDATA%\ClaudeKey\uninstall.bat` if you no longer have the downloaded folder.
The program is stopped, removed from startup and its folder is deleted.

## Manual usage

```
%LOCALAPPDATA%\ClaudeKey\ClaudeKey.exe copilot   # Copilot key
%LOCALAPPDATA%\ClaudeKey\ClaudeKey.exe menu      # Context Menu key
```

Only one instance runs at a time. To stop it, end the **ClaudeKey** process in Task Manager.

## Notes

- When the Context Menu key is remapped, **Shift + F10** still opens the right-click menu.
- While a window running as administrator is in the foreground, Windows does not pass keys to the program for security reasons, so the key won't work at that moment.
- The launchers are `.vbs` files because `.bat` files are set to open in an editor on some machines.

## How it works

The program installs a low-level keyboard hook (`WH_KEYBOARD_LL`):

- The **Copilot key** sends `Win + Shift + F23` to Windows. The program catches `F23`, injects an unassigned key press (`0xE8`) so the Start menu doesn't open when Win is released, and launches Claude.
- The **Context Menu key** (`VK_APPS`) is caught directly.

## Project layout

```
src/ClaudeKey.cs             Program source
scripts/install.bat          Build into %LOCALAPPDATA%\ClaudeKey + add to startup
scripts/uninstall.bat        Stop + remove from startup + delete
install-*.vbs, uninstall.vbs Double-click launchers
.github/workflows/build.yml  Compile check on Windows for every push
```
