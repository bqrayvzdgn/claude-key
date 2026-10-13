Set fso = CreateObject("Scripting.FileSystemObject")
kok = fso.GetParentFolderName(WScript.ScriptFullName)
CreateObject("WScript.Shell").Run "cmd.exe /c """ & kok & "\scripts\kaldir.bat"" ", 0, True
MsgBox "ClaudeTusu kaldirildi. Bu klasoru silebilirsiniz.", 64, "ClaudeTusu"
