@echo off
echo ===============================================
echo   LORE SETUP - KOMUT TESTI
echo ===============================================
echo.

REM Test 1: Publish klasörü kontrolü
echo [TEST 1] Publish dosyalarını kontrol ediliyor...
if exist "bin\Release\net8.0-windows\win-x64\publish\LoreSetupAndDosyaOku.exe" (
    echo ✅ Publish dosyası mevcut
    for %%A in ("bin\Release\net8.0-windows\win-x64\publish\LoreSetupAndDosyaOku.exe") do (
        echo    Boyut: %%~zA bytes
    )
) else (
    echo ❌ Publish dosyası bulunamadı!
)
echo.

REM Test 2: Setup dosyası kontrolü  
echo [TEST 2] Setup dosyası kontrol ediliyor...
if exist "installer\LoreDosyaIzleyici_Setup.exe" (
    echo ✅ Setup dosyası mevcut
    for %%A in ("installer\LoreDosyaIzleyici_Setup.exe") do (
        echo    Boyut: %%~zA bytes
    )
) else (
    echo ❌ Setup dosyası bulunamadı!
)
echo.

REM Test 3: Versiyon bilgileri
echo [TEST 3] Versiyon bilgileri kontrol ediliyor...
find "Version" appsettings.json 2>nul && echo ✅ appsettings.json versiyon bilgisi bulundu
find "MyAppVersion" installer.iss 2>nul && echo ✅ installer.iss versiyon bilgisi bulundu
echo.

REM Test 4: InnoSetup kontrolü
echo [TEST 4] InnoSetup kontrolü...
if exist "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" (
    echo ✅ InnoSetup kurulu
) else (
    echo ❌ InnoSetup bulunamadı!
)
echo.

echo ===============================================
echo   TEST TAMAMLANDI
echo ===============================================
echo.
echo CMD komutları kullanıma hazır!
pause
