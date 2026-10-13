@echo off
rem ClaudeTusu'yu durdurur ve baslangictan kaldirir.
taskkill /IM ClaudeTusu.exe /F >nul 2>&1
powershell -NoProfile -Command "$p=[Environment]::GetFolderPath('Startup')+'\ClaudeTusu.lnk'; if(Test-Path $p){Remove-Item $p}"
