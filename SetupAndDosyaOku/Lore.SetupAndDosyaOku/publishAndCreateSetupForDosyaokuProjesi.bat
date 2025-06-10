@echo off
chcp 65001 >nul
echo ===============================================
echo   LORE DOSYA OKU - PUBLISH VE SETUP OLUŞTURUCU
echo ===============================================
echo.
echo 🚀 Otomatik publish ve setup oluşturma işlemi başlatılıyor...
echo.

REM Ana dizine geç
cd /d "d:\Development\ozel\TemhaPdks\SetupAndDosyaOku\Lore.SetupAndDosyaOku"

REM Önceki build dosyalarını temizle
echo [ADIM 1/4] Eski build dosyaları temizleniyor...
if exist "bin\Release" (
    rmdir /s /q "bin\Release" 2>nul
    echo ✅ Eski publish dosyaları temizlendi
) else (
    echo ✅ Temizlenecek dosya yok
)
echo.

REM Publish işlemi
echo [ADIM 2/4] Proje publish ediliyor...
echo ⏳ Bu işlem 30-60 saniye sürebilir...
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=false

if %errorlevel% == 0 (
    echo ✅ Publish işlemi başarıyla tamamlandı!
    echo.
    
    REM Publish dosyası kontrolü
    if exist "bin\Release\net8.0-windows\win-x64\publish\LoreSetupAndDosyaOku.exe" (
        for %%A in ("bin\Release\net8.0-windows\win-x64\publish\LoreSetupAndDosyaOku.exe") do (
            set /a sizeInMB=%%~zA/1048576
        )
        echo 📁 Publish dosyası: bin\Release\net8.0-windows\win-x64\publish\LoreSetupAndDosyaOku.exe
        echo 💾 Boyut: !sizeInMB! MB
    )
) else (
    echo ❌ Publish işlemi başarısız oldu!
    echo ⚠️  Hatayı kontrol edin ve tekrar deneyin.
    pause
    exit /b 1
)
echo.

REM Setup oluşturma
echo [ADIM 3/4] Setup dosyası oluşturuluyor...

REM InnoSetup kontrolü
if exist "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" (
    echo ✅ InnoSetup bulundu, setup oluşturuluyor...
    
    REM Eski setup dosyasını sil
    if exist "installer\LoreDosyaIzleyici_Setup.exe" (
        del "installer\LoreDosyaIzleyici_Setup.exe" 2>nul
    )
    
    REM Setup oluştur
    "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" installer.iss
    
    if %errorlevel% == 0 (
        echo ✅ Setup dosyası başarıyla oluşturuldu!
        echo.
        
        REM Setup dosyası kontrolü
        if exist "installer\LoreDosyaIzleyici_Setup.exe" (
            for %%A in ("installer\LoreDosyaIzleyici_Setup.exe") do (
                set /a setupSizeInMB=%%~zA/1048576
            )
            echo 📁 Setup dosyası: installer\LoreDosyaIzleyici_Setup.exe
            echo 💾 Boyut: !setupSizeInMB! MB
            echo 📅 Oluşturma: %date% %time%
        )
    ) else (
        echo ❌ Setup dosyası oluşturulamadı!
        echo ⚠️  installer.iss dosyasını kontrol edin.
        pause
        exit /b 1
    )
) else (
    echo ❌ InnoSetup bulunamadı!
    echo 📥 InnoSetup indirmek için: https://jrsoftware.org/isdl.php
    echo ⚠️  InnoSetup'ı kurduktan sonra tekrar deneyin.
    pause
    exit /b 1
)
echo.

REM Final kontroller
echo [ADIM 4/4] Final kontroller yapılıyor...

REM Versiyon bilgileri göster
echo 📋 Versiyon bilgileri:
find "Version" appsettings.json 2>nul
find "MyAppVersion" installer.iss 2>nul
echo.

echo ===============================================
echo   🎉 İŞLEM BAŞARIYLA TAMAMLANDI!
echo ===============================================
echo.
echo 📦 PUBLISH SONUÇLARI:
echo    Dosya: LoreSetupAndDosyaOku.exe
echo    Konum: bin\Release\net8.0-windows\win-x64\publish\
if defined sizeInMB echo    Boyut: %sizeInMB% MB
echo.
echo 🔧 SETUP SONUÇLARI:
echo    Dosya: LoreDosyaIzleyici_Setup.exe
echo    Konum: installer\
if defined setupSizeInMB echo    Boyut: %setupSizeInMB% MB
echo.
echo 🎯 HAZZıRLIK DURUMU:
echo    ✅ Uygulama dağıtıma hazır
echo    ✅ Setup kuruluma hazır
echo    ✅ Test edilebilir
echo.

REM Kullanıcı seçenekleri
echo 🔍 Ne yapmak istiyorsunuz?
echo    [1] Setup dosyasını test et
echo    [2] Publish klasörünü aç
echo    [3] Setup klasörünü aç
echo    [4] Hiçbir şey (çık)
echo.
set /p choice=Seçiminizi yapın (1-4): 

if "%choice%"=="1" (
    echo.
    echo 🚀 Setup dosyası test ediliyor...
    start "" "installer\LoreDosyaIzleyici_Setup.exe"
    echo ✅ Setup dosyası açıldı!
) else if "%choice%"=="2" (
    echo.
    echo 📂 Publish klasörü açılıyor...
    start "" "bin\Release\net8.0-windows\win-x64\publish"
    echo ✅ Klasör açıldı!
) else if "%choice%"=="3" (
    echo.
    echo 📂 Setup klasörü açılıyor...
    start "" "installer"
    echo ✅ Klasör açıldı!
) else (
    echo.
    echo 👋 İşlem tamamlandı!
)

echo.
echo ⭐ Bir sonraki kullanım için bu dosyayı tekrar çalıştırabilirsiniz.
pause
