# claude-key

A tiny Windows utility that opens the **Claude** desktop app when you press the **Copilot** key or the **Context Menu** key, instead of Copilot.

- Nothing extra to install: it is compiled with the C# compiler that ships with Windows (.NET Framework 4).
- Runs in the background with no window and uses roughly 10–15 MB of memory.
- The chosen key is swallowed, so Copilot or the context menu does not open; only Claude does.
- Works with both the Microsoft Store (MSIX) and the classic install of Claude.

## Installation

1. Download the repository (**Code → Download ZIP**) and extract it to a permanent folder, e.g. `Documents\claude-key`.
   The program runs from this folder after installation, so don't delete it.
2. Double-click the file that matches your keyboard:
   - `install-copilot-key.vbs`
   - `install-menu-key.vbs`
3. When the "Installation finished" message appears, press the key to try it.

The installer compiles the program to `bin\ClaudeKey.exe`, adds it to Windows startup and starts it right away.
If anything goes wrong, details are written to `install_log.txt` in the folder.

## Uninstall

Double-click `uninstall.vbs`. The program is stopped and removed from startup; you can then delete the folder.

## Manual usage

```
bin\ClaudeKey.exe copilot   # Copilot key
bin\ClaudeKey.exe menu      # Context Menu key
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
scripts/install.bat          Build + add to startup
scripts/uninstall.bat        Stop + remove from startup
install-*.vbs, uninstall.vbs Double-click launchers
.github/workflows/build.yml  Compile check on Windows for every push
```
