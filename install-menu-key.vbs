Set sh = CreateObject("WScript.Shell")
Set fso = CreateObject("Scripting.FileSystemObject")
root = fso.GetParentFolderName(WScript.ScriptFullName)
sh.Run "cmd.exe /c """ & root & "\scripts\install.bat"" menu", 0, True
logPath = sh.ExpandEnvironmentStrings("%LOCALAPPDATA%\ClaudeKey\install_log.txt")
MsgBox "Installation finished. You can delete this folder now." & vbCrLf & vbCrLf & "Log: " & logPath, 64, "ClaudeKey"
