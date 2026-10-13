Set fso = CreateObject("Scripting.FileSystemObject")
root = fso.GetParentFolderName(WScript.ScriptFullName)
CreateObject("WScript.Shell").Run "cmd.exe /c """ & root & "\scripts\install.bat"" menu", 0, True
MsgBox "Installation finished. Details: install_log.txt", 64, "ClaudeKey"
