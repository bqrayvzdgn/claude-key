Set fso = CreateObject("Scripting.FileSystemObject")
kok = fso.GetParentFolderName(WScript.ScriptFullName)
CreateObject("WScript.Shell").Run "cmd.exe /c """ & kok & "\scripts\kurulum.bat"" copilot", 0, True
MsgBox "Kurulum bitti. Ayrintilar: kurulum_log.txt", 64, "ClaudeTusu"
