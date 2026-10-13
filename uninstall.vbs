Set fso = CreateObject("Scripting.FileSystemObject")
root = fso.GetParentFolderName(WScript.ScriptFullName)
CreateObject("WScript.Shell").Run "cmd.exe /c """ & root & "\scripts\uninstall.bat"" ", 0, True
MsgBox "ClaudeKey has been removed.", 64, "ClaudeKey"
