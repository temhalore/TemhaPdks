# PowerShell script: PublishAndCreateSetup.ps1
# UTF-8 ile kaydedin

# Log dosyasi ayarlari
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$logFile = Join-Path $scriptPath "publish_log.txt"

# Log fonksiyonu
function Write-Log {
    param([string]$message)
    Write-Host $message
    Add-Content -Path $logFile -Value "$(Get-Date) - $message"
}

# Baslangic
Clear-Host
"===============================================" | Out-File -FilePath $logFile
"  LORE DOSYA OKU - PUBLISH VE SETUP OLUSTURUCU" | Add-Content -Path $logFile
"  $(Get-Date)" | Add-Content -Path $logFile
"===============================================" | Add-Content -Path $logFile

Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "  LORE DOSYA OKU - PUBLISH VE SETUP OLUSTURUCU" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "BU KONSOL KAPANMAYACAK - ISLEM BITTIGINDE MANUEL OLARAK KAPATABILIRSINIZ" -ForegroundColor Yellow
Write-Host "LOG DOSYASI: $logFile" -ForegroundColor Gray
Write-Host ""

# Ana dizine gec
Set-Location "D:\Development\ozel\TemhaPdks\SetupAndDosyaOku\Lore.SetupAndDosyaOku"
Write-Log "Calisma dizini: $(Get-Location)"

# Versiyon bilgisi okuma
Write-Host "Versiyon bilgisi okunuyor..." -ForegroundColor Yellow
$currentVersion = "1.0.0"

if (Test-Path "appsettings.json") {
    $versionLine = Get-Content "appsettings.json" | Where-Object { $_ -match '"Version"' }
    if ($versionLine) {
        $versionMatch = $versionLine -match '"Version"\s*:\s*"([^"]+)"'
        if ($matches) {
            $currentVersion = $matches[1]
            Write-Log "Versiyon basariyla okundu: $currentVersion"
        }
    }
}

Write-Host "Mevcut versiyon: $currentVersion" -ForegroundColor Green
Write-Host ""

# Versiyon secimi
Write-Host "Versiyon guncelleme secenekleri:" -ForegroundColor Cyan
Write-Host "  [1] Mevcut versiyonu kullan ($currentVersion)" -ForegroundColor White
Write-Host "  [2] Yeni versiyon numarasi gir" -ForegroundColor White
Write-Host "  [3] Otomatik versiyon artir (son rakami +1)" -ForegroundColor White
Write-Host ""

$versionChoice = Read-Host "Seciminizi yapin (1-3)"
Write-Log "Kullanici secimi: $versionChoice"

switch ($versionChoice) {
    "1" {
        Write-Host "Mevcut versiyon kullanilacak: $currentVersion" -ForegroundColor Green
    }
    "2" {
        Write-Host ""
        $newVersion = Read-Host "Yeni versiyon numarasini girin (ornek: 1.2.3)"
        
        if ([string]::IsNullOrEmpty($newVersion)) {
            Write-Host "Versiyon bos olamaz!" -ForegroundColor Red
            Write-Host "3 saniye bekleyip ana menuye donuluyor..." -ForegroundColor Yellow
            Start-Sleep -Seconds 3
            exit
        }
        
        Write-Host ""
        Write-Host "Versiyon $currentVersion -> $newVersion olarak degistirilecek." -ForegroundColor Yellow
        Write-Host ""
        $confirm = Read-Host "Devam etmek istiyor musunuz? (E/H)"
        
        if ($confirm -eq "E" -or $confirm -eq "e") {
            Write-Host "Versiyon guncelleniyor..." -ForegroundColor Yellow
            
            # appsettings.json guncelleme
            if (Test-Path "appsettings.json") {
                $content = Get-Content "appsettings.json" -Raw
                $content = $content -replace '"Version"\s*:\s*"[^"]+"', "`"Version`": `"$newVersion`""
                Set-Content "appsettings.json" $content
                Write-Log "appsettings.json guncellendi"
            }
            
            # installer.iss guncelleme - Bos versiyon degerini guncelle
            if (Test-Path "installer.iss") {
                $content = Get-Content "installer.iss" -Raw
                $content = $content -replace '#define MyAppVersion\s+"[^"]*"', "#define MyAppVersion `"$newVersion`""
                Set-Content "installer.iss" $content
                Write-Log "installer.iss guncellendi"
            }
            
            Write-Host "Versiyon guncellendi: $newVersion" -ForegroundColor Green
            $currentVersion = $newVersion
        }
        else {
            Write-Host "Versiyon guncellenmedi." -ForegroundColor Yellow
        }
    }
    "3" {
        Write-Host ""
        Write-Host "Otomatik versiyon artirma..." -ForegroundColor Yellow
        
        $versionParts = $currentVersion -split '\.'
        $major = $versionParts[0]
        $minor = $versionParts[1]
        $patch = $versionParts[2]
        
        if ([string]::IsNullOrEmpty($patch)) { $patch = 0 }
        $patch = [int]$patch + 1
        $newVersion = "$major.$minor.$patch"
        
        Write-Host "Versiyon artirildi: $currentVersion -> $newVersion" -ForegroundColor Green
        Write-Host ""
        $confirm = Read-Host "Bu degisikligi kaydetmek istiyor musunuz? (E/H)"
        
        if ($confirm -eq "E" -or $confirm -eq "e") {
            Write-Host "Versiyon guncelleniyor..." -ForegroundColor Yellow
            
            # appsettings.json guncelleme
            if (Test-Path "appsettings.json") {
                $content = Get-Content "appsettings.json" -Raw
                $content = $content -replace '"Version"\s*:\s*"[^"]+"', "`"Version`": `"$newVersion`""
                Set-Content "appsettings.json" $content
                Write-Log "appsettings.json guncellendi"
            }
            
            # installer.iss guncelleme - Bos versiyon degerini guncelle
            if (Test-Path "installer.iss") {
                $content = Get-Content "installer.iss" -Raw
                $content = $content -replace '#define MyAppVersion\s+"[^"]*"', "#define MyAppVersion `"$newVersion`""
                Set-Content "installer.iss" $content
                Write-Log "installer.iss guncellendi"
            }
            
            Write-Host "Versiyon guncellendi: $newVersion" -ForegroundColor Green
            $currentVersion = $newVersion
        }
        else {
            Write-Host "Versiyon guncellenmedi." -ForegroundColor Yellow
        }
    }
    default {
        Write-Host "Gecersiz secim! Mevcut versiyon kullanilacak." -ForegroundColor Red
    }
}

# Versiyon bos olmamali, InnoSetup icin
if ([string]::IsNullOrEmpty($currentVersion) -or $currentVersion -eq "") {
    $currentVersion = "1.0.0"
    Write-Host "Versiyon bos olamaz! Varsayilan versiyon kullanilacak: $currentVersion" -ForegroundColor Yellow
    
    # installer.iss guncelleme - Bos versiyon degerini guncelle
    if (Test-Path "installer.iss") {
        $content = Get-Content "installer.iss" -Raw
        $content = $content -replace '#define MyAppVersion\s+"[^"]*"', "#define MyAppVersion `"$currentVersion`""
        Set-Content "installer.iss" $content
        Write-Log "installer.iss guncellendi (bos versiyon duzeltildi)"
    }
}

# Build islemi
Write-Host ""
Write-Host "Kullanilacak versiyon: $currentVersion" -ForegroundColor Cyan
Write-Host ""
Write-Host "Publish ve setup olusturma islemi baslatiliyor..." -ForegroundColor Yellow
Write-Host "KONSOL KAPANMAYACAK - HATA CIKSA BILE!" -ForegroundColor Red
Write-Host ""

# Build temizleme
Write-Host "[ADIM 1/4] Eski build dosyalari temizleniyor..." -ForegroundColor Cyan
Write-Log "Eski build dosyalari temizleniyor..."

if (Test-Path "bin\Release") {
    Write-Host "Eski build klasoru bulundu, siliniyor..." -ForegroundColor Yellow
    Remove-Item -Path "bin\Release" -Recurse -Force -ErrorAction SilentlyContinue
    Write-Host "Eski dosyalar temizlendi" -ForegroundColor Green
}
else {
    Write-Host "Temizlenecek dosya yok" -ForegroundColor Green
}
Write-Host ""

# Publish islemi
Write-Host "[ADIM 2/4] Proje publish ediliyor..." -ForegroundColor Cyan
Write-Host "Bu islem 30-60 saniye surebilir..." -ForegroundColor Yellow
Write-Host "HATA CIKSA BILE KONSOL KAPANMAYACAK!" -ForegroundColor Red
Write-Host ""

Write-Host "Publish komutu baslatiliyor..." -ForegroundColor Yellow
Write-Host "Calistirilan komut:" -ForegroundColor Gray
Write-Host "  dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=false" -ForegroundColor Gray
Write-Host ""

Write-Log "dotnet publish komutu basliyor..."
$publishOutput = & dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=false 2>&1
$publishResult = $LASTEXITCODE
$publishOutput | ForEach-Object { Write-Host $_; Write-Log $_ }

Write-Host ""
Write-Host "Publish islemi tamamlandi. Exit Code: $publishResult" -ForegroundColor Yellow

if ($publishResult -eq 0) {
    Write-Host "Publish islemi BASARILI!" -ForegroundColor Green
    
    $exePath = "bin\Release\net8.0-windows\win-x64\publish\LoreSetupAndDosyaOku.exe"
    if (Test-Path $exePath) {
        $fileInfo = Get-Item $exePath
        $sizeInMB = [math]::Round($fileInfo.Length / 1MB, 2)
        
        Write-Host "Dosya: LoreSetupAndDosyaOku.exe" -ForegroundColor White
        Write-Host "Boyut: $sizeInMB MB" -ForegroundColor White
        Write-Host "Konum: bin\Release\net8.0-windows\win-x64\publish\" -ForegroundColor White
        Write-Log "Publish basarili: $exePath ($sizeInMB MB)"
    }
    else {
        Write-Host "Publish basarili ama EXE dosyasi bulunamadi!" -ForegroundColor Red
        Write-Host "Publish klasoru kontrol ediliyor..." -ForegroundColor Yellow
        Get-ChildItem "bin\Release\net8.0-windows\win-x64\publish\" | ForEach-Object { Write-Log $_.FullName }
    }
}
else {
    Write-Host "Publish islemi BASARISIZ! Hata kodu: $publishResult" -ForegroundColor Red
    Write-Host "ANCAK KONSOL KAPANMAYACAK - HATALARINIZI GOREBILIRSINIZ" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Hata analizi icin lutfen yukaridaki ciktiyi inceleyin" -ForegroundColor Yellow
    Write-Host ""
    $retryPublish = Read-Host "Tekrar denemek istiyor musunuz? (E/H)"
    if ($retryPublish -eq "E" -or $retryPublish -eq "e") {
        Write-Host "Publish islemi tekrar deneniyor..." -ForegroundColor Yellow
        # Tekrar publish islemi
    }
    else {
        Write-Host "Setup adimina geciliyor (publish olmadan)" -ForegroundColor Yellow
    }
}
Write-Host ""

# Setup olusturma
Write-Host "[ADIM 3/4] Setup dosyasi olusturuluyor..." -ForegroundColor Cyan
Write-Log "Setup dosyasi olusturuluyor..."
Write-Host "InnoSetup kontrolu..." -ForegroundColor Yellow

$innoSetupPath = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
if (Test-Path $innoSetupPath) {
    Write-Host "InnoSetup bulundu" -ForegroundColor Green
    
    $setupPath = "installer\LoreDosyaIzleyici_Setup.exe"
    if (Test-Path $setupPath) {
        Write-Host "Eski setup dosyasi siliniyor..." -ForegroundColor Yellow
        Remove-Item -Path $setupPath -Force -ErrorAction SilentlyContinue
    }
    
    # Son bir kez daha installer.iss dosyasini kontrol et
    if (Test-Path "installer.iss") {
        $issContent = Get-Content "installer.iss" -Raw
        if ($issContent -match '#define MyAppVersion\s+""') {
            Write-Host "UYARI: installer.iss dosyasinda versiyon hala bos!" -ForegroundColor Red
            Write-Host "Versiyon otomatik olarak ayarlaniyor: $currentVersion" -ForegroundColor Yellow
            
            $issContent = $issContent -replace '#define MyAppVersion\s+"[^"]*"', "#define MyAppVersion `"$currentVersion`""
            Set-Content "installer.iss" $issContent
            Write-Log "installer.iss guncellendi (bos versiyon duzeltildi)"
        }
    }
    
    Write-Host "InnoSetup komutu calistiriliyor..." -ForegroundColor Yellow
    Write-Host "  `"$innoSetupPath`" installer.iss" -ForegroundColor Gray
    Write-Host ""
    
    Write-Log "InnoSetup komutu basliyor..."
    $setupOutput = & $innoSetupPath installer.iss 2>&1
    $setupResult = $LASTEXITCODE
    $setupOutput | ForEach-Object { Write-Host $_; Write-Log $_ }
    
    Write-Host ""
    Write-Host "Setup olusturma tamamlandi. Exit Code: $setupResult" -ForegroundColor Yellow
    
    if ($setupResult -eq 0) {
        Write-Host "Setup dosyasi BASARIYLA olusturuldu!" -ForegroundColor Green
        
        if (Test-Path $setupPath) {
            $fileInfo = Get-Item $setupPath
            $setupSizeInMB = [math]::Round($fileInfo.Length / 1MB, 2)
            
            Write-Host "Setup: LoreDosyaIzleyici_Setup.exe" -ForegroundColor White
            Write-Host "Boyut: $setupSizeInMB MB" -ForegroundColor White
            Write-Host "Konum: installer\" -ForegroundColor White
            Write-Host "Olusturma: $(Get-Date)" -ForegroundColor White
            Write-Log "Setup basarili: $setupPath ($setupSizeInMB MB)"
        }
        else {
            Write-Host "Setup komutu basarili ama dosya bulunamadi!" -ForegroundColor Red
            Write-Host "installer klasoru kontrol ediliyor..." -ForegroundColor Yellow
            Get-ChildItem "installer\" | ForEach-Object { Write-Log $_.FullName }
        }
    }
    else {
        Write-Host "Setup dosyasi olusturulamadi! Hata kodu: $setupResult" -ForegroundColor Red
        Write-Host "installer.iss dosyasini kontrol edin." -ForegroundColor Yellow
        Write-Host "ANCAK KONSOL KAPANMAYACAK - HATALARINIZI GOREBILIRSINIZ" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Hata analizi icin lutfen yukaridaki ciktiyi inceleyin" -ForegroundColor Yellow
        Write-Host ""
        $retrySetup = Read-Host "Tekrar denemek istiyor musunuz? (E/H)"
        if ($retrySetup -eq "E" -or $retrySetup -eq "e") {
            Write-Host "Setup islemi tekrar deneniyor..." -ForegroundColor Yellow
            # Tekrar setup islemi
        }
        else {
            Write-Host "Final adimina geciliyor (setup olmadan)" -ForegroundColor Yellow
        }
    }
}
else {
    Write-Host "InnoSetup bulunamadi!" -ForegroundColor Red
    Write-Host "InnoSetup indirmek icin: https://jrsoftware.org/isdl.php" -ForegroundColor Yellow
    Write-Host "Setup olusturulamadi, sadece publish yapildi." -ForegroundColor Yellow
    Write-Host "KONSOL KAPANMAYACAK - DEVAM EDEBILIRSINIZ" -ForegroundColor Yellow
}
Write-Host ""

# Final kontroller
Write-Host "[ADIM 4/4] Final kontroller yapiliyor..." -ForegroundColor Cyan
Write-Host "Versiyon bilgileri kontrol ediliyor:" -ForegroundColor Yellow

if (Test-Path "appsettings.json") {
    Get-Content "appsettings.json" | Select-String "Version" | ForEach-Object { Write-Log $_; Write-Host $_ }
}
if (Test-Path "installer.iss") {
    Get-Content "installer.iss" | Select-String "MyAppVersion" | ForEach-Object { Write-Log $_; Write-Host $_ }
}

Write-Host ""
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "  ISLEM TAMAMLANDI!" -ForegroundColor Green
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host ""

# Dosya durumlarini kontrol et
Write-Host "SONUC RAPORU:" -ForegroundColor Cyan
$exePath = "bin\Release\net8.0-windows\win-x64\publish\LoreSetupAndDosyaOku.exe"
if (Test-Path $exePath) {
    Write-Host "PUBLISH: $exePath - BASARILI" -ForegroundColor Green
}
else {
    Write-Host "PUBLISH: Dosya bulunamadi!" -ForegroundColor Red
}

$setupPath = "installer\LoreDosyaIzleyici_Setup.exe"
if (Test-Path $setupPath) {
    Write-Host "SETUP: $setupPath - BASARILI" -ForegroundColor Green
}
else {
    Write-Host "SETUP: Dosya bulunamadi!" -ForegroundColor Red
}

Write-Host "VERSIYON: $currentVersion" -ForegroundColor Cyan
Write-Host ""

# Final menu
function Show-FinalMenu {
    Write-Host "===============================================" -ForegroundColor Cyan
    Write-Host "  SONRAKI ADIMLAR" -ForegroundColor Cyan
    Write-Host "===============================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Ne yapmak istiyorsunuz?" -ForegroundColor Yellow
    Write-Host "  [1] Setup dosyasini test et" -ForegroundColor White
    Write-Host "  [2] Publish klasorunu ac" -ForegroundColor White
    Write-Host "  [3] Setup klasorunu ac" -ForegroundColor White
    Write-Host "  [4] Log dosyasini ac" -ForegroundColor White
    Write-Host "  [5] Script'i yeniden baslat" -ForegroundColor White
    Write-Host "  [6] Konsolu acik birak (manuel kapatim)" -ForegroundColor White
    Write-Host "  [7] Cik" -ForegroundColor White
    Write-Host ""
    $choice = Read-Host "Seciminizi yapin (1-7)"
    Write-Log "Kullanici secimi: $choice"
    
    switch ($choice) {
        "1" {
            if (Test-Path $setupPath) {
                Write-Host ""
                Write-Host "Setup test ediliyor..." -ForegroundColor Yellow
                Start-Process $setupPath
                Write-Host "Setup acildi!" -ForegroundColor Green
            }
            else {
                Write-Host "Setup dosyasi bulunamadi!" -ForegroundColor Red
            }
            return $true
        }
        "2" {
            Write-Host ""
            Write-Host "Publish klasoru aciliyor..." -ForegroundColor Yellow
            Start-Process "bin\Release\net8.0-windows\win-x64\publish"
            Write-Host "Klasor acildi!" -ForegroundColor Green
            return $true
        }
        "3" {
            Write-Host ""
            Write-Host "Setup klasoru aciliyor..." -ForegroundColor Yellow
            Start-Process "installer"
            Write-Host "Klasor acildi!" -ForegroundColor Green
            return $true
        }
        "4" {
            Write-Host ""
            Write-Host "Log dosyasi aciliyor..." -ForegroundColor Yellow
            Start-Process "notepad" -ArgumentList $logFile
            Write-Host "Log dosyasi acildi!" -ForegroundColor Green
            return $true
        }
        "5" {
            Write-Host ""
            Write-Host "Script yeniden baslatiliyor..." -ForegroundColor Yellow
            Write-Host ""
            # PowerShell'de script'i yeniden baslatmak icin
            $scriptPath = $MyInvocation.MyCommand.Path
            & $scriptPath
            exit
        }
        "6" {
            Write-Host ""
            Write-Host "Konsol acik kalacak - istediginiz zaman kapatabilirsiniz!" -ForegroundColor Green
            Write-Host "Baska komutlar calistirabilir veya dosyalari inceleyebilirsiniz." -ForegroundColor Yellow
            Write-Host ""
            Write-Host "[SERBEST KOMUT MODU - KONSOL ACIK KALACAK]" -ForegroundColor Cyan
            Write-Host "Cikmak icin 'exit' yazin veya pencereyi kapatin." -ForegroundColor Yellow
            
            # PowerShell'de yeni bir konsol acmak
            Start-Process powershell
            return $false
        }
        "7" {
            Write-Host ""
            Write-Host "Islem tamamlandi!" -ForegroundColor Green
            return $false
        }
        default {
            Write-Host "Gecersiz secim! Tekrar deneyin." -ForegroundColor Red
            return $true
        }
    }
}

$continueMenu = $true
while ($continueMenu) {
    $continueMenu = Show-FinalMenu
    
    if ($continueMenu) {
        Write-Host ""
        $continueChoice = Read-Host "Baska bir islem yapmak istiyor musunuz? (E/H)"
        if ($continueChoice -ne "E" -and $continueChoice -ne "e") {
            $continueMenu = $false
            Write-Host ""
            Write-Host "Islem tamamlandi!" -ForegroundColor Green
        }
    }
}

# Script sonu
Write-Host ""
Write-Host "Script tamamlandi! Kapatmak icin bir tusa basin..." -ForegroundColor Cyan
Write-Host "Log dosyasi: $logFile" -ForegroundColor Gray

Add-Content -Path $logFile -Value ""
Add-Content -Path $logFile -Value "==============================================="
Add-Content -Path $logFile -Value "Log Bitis: $(Get-Date)"
Add-Content -Path $logFile -Value "==============================================="

Write-Host ""
$keepOpen = Read-Host "Son secenek: Konsolu acik birakmak istiyor musunuz? (E/H)"
if ($keepOpen -eq "E" -or $keepOpen -eq "e") {
    Write-Host ""
    Write-Host "Konsol acik kalacak - istediginiz zaman kapatin!" -ForegroundColor Green
    Write-Host ""
    
    # PowerShell'de beklemek icin
    $null = Read-Host "Cikmak icin Enter tusuna basin..."
}
else {
    Write-Host ""
    Write-Host "Kapatmak icin bir tusa basin..." -ForegroundColor Yellow
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
}