@echo off
chcp 65001 >nul
cd /d "%~dp0MsgoktasMu"

echo [1/2] Release surumu hazirlaniyor (win-x64, .NET dahil)...
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishReadyToRun=true -o "..\publish"
if errorlevel 1 (
    echo.
    echo HATA: Publish basarisiz.
    pause
    exit /b 1
)

echo.
echo [2/2] Tamamlandi.
echo Cikti klasoru: %~dp0publish
echo Sonraki adim: BUILD_SETUP.bat calistirin veya setup\MsgoktasMu.iss dosyasini Inno Setup ile derleyin.
echo.
pause
