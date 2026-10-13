@echo off
rem Stops ClaudeKey, removes it from startup and deletes %LOCALAPPDATA%\ClaudeKey.
taskkill /IM ClaudeKey.exe /F >nul 2>&1
taskkill /IM ClaudeTusu.exe /F >nul 2>&1
powershell -NoProfile -Command "foreach($n in 'ClaudeKey.lnk','ClaudeTusu.lnk'){$p=[Environment]::GetFolderPath('Startup')+'\'+$n; if(Test-Path $p){Remove-Item $p}}"
timeout /t 1 /nobreak >nul
cd /d "%TEMP%"
rem This script may live inside the folder being deleted: "(goto)" ends the script
rem first, so cmd doesn't try to read the rest of a file that no longer exists.
(goto) 2>nul & rmdir /s /q "%LOCALAPPDATA%\ClaudeKey"
