@echo off
chcp 65001 >nul
cd /d "%~dp0"

if not exist "publish\MsgoktasMu.exe" (
    echo publish klasoru bulunamadi. Once PUBLISH.bat calistirin.
    pause
    exit /b 1
)

set "ISCC=C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
if not exist "%ISCC%" set "ISCC=C:\Program Files\Inno Setup 6\ISCC.exe"
if not exist "%ISCC%" set "ISCC=%LOCALAPPDATA%\Programs\Inno Setup 6\ISCC.exe"

if not exist "%ISCC%" (
    echo Inno Setup 6 bulunamadi.
    echo.
    echo 1. https://jrsoftware.org/isdl.php adresinden Inno Setup 6 indirin ve kurun
    echo 2. setup\MsgoktasMu.iss dosyasini acin
    echo 3. Build ^> Compile ile setup exe olusturun
    echo.
    echo Cikti dosyasi: setup\output\MsgoktasMu_Setup.exe
    pause
    exit /b 1
)

echo Setup derleniyor...
"%ISCC%" "setup\MsgoktasMu.iss"
if errorlevel 1 (
    echo HATA: Setup derlemesi basarisiz.
    pause
    exit /b 1
)

echo.
echo Basarili: setup\output\MsgoktasMu_Setup.exe
echo Bu dosyayi baska bilgisayarlara vererek kurulum yapabilirsiniz.
echo.
pause
