@echo off
rem Stops ClaudeKey and removes it from startup.
taskkill /IM ClaudeKey.exe /F >nul 2>&1
taskkill /IM ClaudeTusu.exe /F >nul 2>&1
powershell -NoProfile -Command "foreach($n in 'ClaudeKey.lnk','ClaudeTusu.lnk'){$p=[Environment]::GetFolderPath('Startup')+'\'+$n; if(Test-Path $p){Remove-Item $p}}"
