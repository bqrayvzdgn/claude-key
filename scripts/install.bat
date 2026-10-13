@echo off
rem ClaudeKey installer
rem Usage: install.bat [copilot|menu]
rem Compiles the source, adds it to startup and starts it.

setlocal
set "ROOT=%~dp0.."
for %%I in ("%ROOT%") do set "ROOT=%%~fI"
set "KEY=%~1"
if "%KEY%"=="" set "KEY=copilot"
set "EXE=%ROOT%\bin\ClaudeKey.exe"
set "LOG=%ROOT%\install_log.txt"
set "CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if not exist "%CSC%" set "CSC=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe"

echo === ClaudeKey install (%KEY%) %date% %time% === > "%LOG%"

echo [Claude] >> "%LOG%"
powershell -NoProfile -Command "Get-AppxPackage -Name Claude | Select-Object Name,PackageFamilyName | Format-List" >> "%LOG%" 2>&1
if exist "%LOCALAPPDATA%\AnthropicClaude\claude.exe" (echo Classic install: found >> "%LOG%") else (echo Classic install: none >> "%LOG%")

rem Stop running copies (including the older ClaudeTusu build) and remove its startup shortcut
taskkill /IM ClaudeKey.exe /F >nul 2>&1
taskkill /IM ClaudeTusu.exe /F >nul 2>&1
powershell -NoProfile -Command "$p=[Environment]::GetFolderPath('Startup')+'\ClaudeTusu.lnk'; if(Test-Path $p){Remove-Item $p}"
timeout /t 1 /nobreak >nul

echo [Build] >> "%LOG%"
if not exist "%ROOT%\bin" mkdir "%ROOT%\bin"
"%CSC%" /nologo /optimize+ /target:winexe /out:"%EXE%" "%ROOT%\src\ClaudeKey.cs" >> "%LOG%" 2>&1
if not exist "%EXE%" (echo BUILD_FAILED >> "%LOG%" & goto end)
echo BUILD_OK >> "%LOG%"

echo [Startup shortcut] >> "%LOG%"
powershell -NoProfile -Command "$p=[Environment]::GetFolderPath('Startup')+'\ClaudeKey.lnk'; $s=(New-Object -ComObject WScript.Shell).CreateShortcut($p); $s.TargetPath='%EXE%'; $s.Arguments='%KEY%'; $s.WorkingDirectory='%ROOT%'; $s.Save(); if(Test-Path $p){'SHORTCUT_OK'}else{'SHORTCUT_FAILED'}" >> "%LOG%" 2>&1

start "" "%EXE%" %KEY%
timeout /t 2 /nobreak >nul
tasklist /FI "IMAGENAME eq ClaudeKey.exe" | find /I "ClaudeKey.exe" >nul && (echo RUNNING_OK >> "%LOG%") || (echo NOT_RUNNING >> "%LOG%")

:end
echo DONE >> "%LOG%"
endlocal
