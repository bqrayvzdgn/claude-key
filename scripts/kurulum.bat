@echo off
rem ClaudeTusu kurulum betigi
rem Kullanim: kurulum.bat [copilot|menu]
rem Kaynak kodu derler, baslangica ekler ve programi baslatir.

setlocal
set "KOK=%~dp0.."
for %%I in ("%KOK%") do set "KOK=%%~fI"
set "TUS=%~1"
if "%TUS%"=="" set "TUS=copilot"
set "EXE=%KOK%\bin\ClaudeTusu.exe"
set "LOG=%KOK%\kurulum_log.txt"
set "CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if not exist "%CSC%" set "CSC=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe"

echo === ClaudeTusu kurulumu (%TUS%) %date% %time% === > "%LOG%"

echo [Claude] >> "%LOG%"
powershell -NoProfile -Command "Get-AppxPackage -Name Claude | Select-Object Name,PackageFamilyName | Format-List" >> "%LOG%" 2>&1
if exist "%LOCALAPPDATA%\AnthropicClaude\claude.exe" (echo Klasik kurulum: var >> "%LOG%") else (echo Klasik kurulum: yok >> "%LOG%")

taskkill /IM ClaudeTusu.exe /F >nul 2>&1
timeout /t 1 /nobreak >nul

echo [Derleme] >> "%LOG%"
if not exist "%KOK%\bin" mkdir "%KOK%\bin"
"%CSC%" /nologo /optimize+ /target:winexe /out:"%EXE%" "%KOK%\src\ClaudeTusu.cs" >> "%LOG%" 2>&1
if not exist "%EXE%" (echo DERLEME_HATA >> "%LOG%" & goto son)
echo DERLEME_OK >> "%LOG%"

echo [Baslangic kisayolu] >> "%LOG%"
powershell -NoProfile -Command "$p=[Environment]::GetFolderPath('Startup')+'\ClaudeTusu.lnk'; $s=(New-Object -ComObject WScript.Shell).CreateShortcut($p); $s.TargetPath='%EXE%'; $s.Arguments='%TUS%'; $s.WorkingDirectory='%KOK%'; $s.Save(); if(Test-Path $p){'KISAYOL_OK'}else{'KISAYOL_HATA'}" >> "%LOG%" 2>&1

start "" "%EXE%" %TUS%
timeout /t 2 /nobreak >nul
tasklist /FI "IMAGENAME eq ClaudeTusu.exe" | find /I "ClaudeTusu.exe" >nul && (echo CALISIYOR_OK >> "%LOG%") || (echo CALISMIYOR >> "%LOG%")

:son
echo BITTI >> "%LOG%"
endlocal
