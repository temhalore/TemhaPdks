@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion
echo ===============================================
echo   LORE DOSYA OKU - PUBLISH VE SETUP OLUŞTURUCU
echo ===============================================
echo.

REM Ana dizine geç
cd /d "d:\Development\ozel\TemhaPdks\SetupAndDosyaOku\Lore.SetupAndDosyaOku"

REM Mevcut versiyon bilgilerini oku
echo 📋 Mevcut versiyon bilgileri kontrol ediliyor...
echo.

REM appsettings.json'dan mevcut versiyonu oku
for /f "tokens=2 delims=:" %%a in ('find "Version" appsettings.json 2^>nul') do (
    set "current_version=%%a"
    REM Tırnak işaretlerini ve virgülleri temizle
    set "current_version=!current_version:"=!"
    set "current_version=!current_version:,=!"
    set "current_version=!current_version: =!"
)

if "!current_version!"=="" (
    echo ⚠️  appsettings.json'da versiyon bulunamadı, varsayılan versiyon kullanılıyor: 1.0.0
    set "current_version=1.0.0"
)

echo 📌 Mevcut versiyon: !current_version!
echo.

REM Versiyon güncelleme seçeneği sun
echo 🔄 Versiyon güncelleme seçenekleri:
echo    [1] Mevcut versiyonu kullan (!current_version!)
echo    [2] Yeni versiyon numarası gir
echo    [3] Otomatik versiyon artır (son rakamı +1)
echo.
set /p version_choice=Seçiminizi yapın (1-3): 

if "!version_choice!"=="2" (
    echo.
    set /p new_version=🔢 Yeni versiyon numarasını girin (örnek: 1.0.1): 
    echo.
    echo 📝 Versiyon !current_version! -> !new_version! olarak değiştirilecek.
    echo ⚠️  Bu değişiklik appsettings.json ve installer.iss dosyalarını güncelleyecek.
    echo.
    set /p confirm=Devam etmek istiyor musunuz? (E/H): 
    
    if /i "!confirm!"=="E" (
        call :UpdateVersion "!new_version!"
        if !errorlevel! equ 0 (
            echo ✅ Versiyon başarıyla güncellendi: !new_version!
            set "current_version=!new_version!"
        ) else (
            echo ❌ Versiyon güncellemesi başarısız!
            pause
            exit /b 1
        )
    ) else (
        echo ℹ️  Versiyon güncellenmedi, mevcut versiyon kullanılacak.
    )
    
) else if "!version_choice!"=="3" (
    echo.
    REM Otomatik versiyon artırma
    call :AutoIncrementVersion "!current_version!"
    if !errorlevel! equ 0 (
        echo ✅ Versiyon otomatik olarak artırıldı: !current_version! -> !new_version!
        set "current_version=!new_version!"
        
        echo.
        set /p confirm=Bu değişikliği kaydetmek istiyor musunuz? (E/H): 
        if /i "!confirm!"=="E" (
            call :UpdateVersion "!new_version!"
            if !errorlevel! equ 0 (
                echo ✅ Versiyon dosyalarda güncellendi!
            ) else (
                echo ❌ Versiyon güncelleme başarısız!
                pause
                exit /b 1
            )
        ) else (
            echo ℹ️  Versiyon güncellenmedi, mevcut versiyon kullanılacak.
            set "current_version=!current_version!"
        )
    ) else (
        echo ❌ Otomatik versiyon artırma başarısız!
        pause
        exit /b 1
    )
) else (
    echo ℹ️  Mevcut versiyon kullanılacak: !current_version!
)

echo.
echo 🎯 Kullanılacak versiyon: !current_version!
echo.
echo 🚀 Publish ve setup oluşturma işlemi başlatılıyor...

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
goto :eof

REM ===============================================
REM   VERSİYON GÜNCELLEMESİ FONKSİYONLARI
REM ===============================================

:UpdateVersion
setlocal
set "new_ver=%~1"

echo.
echo 📝 Versiyon güncelleme başlıyor...

REM appsettings.json güncelle
echo    - appsettings.json güncelleniyor...
powershell -Command "(Get-Content 'appsettings.json') -replace '\"Version\": \".*\"', '\"Version\": \"%new_ver%\"' | Set-Content 'appsettings.json'"
if !errorlevel! neq 0 (
    echo ❌ appsettings.json güncellenemedi!
    exit /b 1
)

REM installer.iss güncelle
echo    - installer.iss güncelleniyor...
powershell -Command "(Get-Content 'installer.iss') -replace '#define MyAppVersion \".*\"', '#define MyAppVersion \"%new_ver%\"' | Set-Content 'installer.iss'"
if !errorlevel! neq 0 (
    echo ❌ installer.iss güncellenemedi!
    exit /b 1
)

echo ✅ Versiyon dosyaları güncellendi!
exit /b 0

:AutoIncrementVersion
setlocal
set "current_ver=%~1"

REM Versiyon formatını ayır (örn: 1.0.0 -> 1, 0, 0)
for /f "tokens=1,2,3 delims=." %%a in ("%current_ver%") do (
    set "major=%%a"
    set "minor=%%b"  
    set "patch=%%c"
)

REM Son rakamı artır
if "!patch!"=="" set "patch=0"
set /a "patch=!patch!+1"

REM Yeni versiyonu oluştur
set "new_version=!major!.!minor!.!patch!"

REM Global değişkene ata
endlocal & set "new_version=%new_version%"
exit /b 0
