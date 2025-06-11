# Versiyon Senkronizasyon Kılavuzu

## Özet
PowerShell scripti (`PublishAndCreateSetup.ps1`) artık DataOku projesi ile API projesi arasında versiyon numaralarını otomatik olarak senkronize ediyor.

## Versiyon Senkronizasyonu Nasıl Çalışır?

### 1. Mevcut Durum
- **DataOku Projesi**: `SetupAndDosyaOku\Lore.SetupAndDosyaOku\appsettings.json` → `"Version": "1.0.4"`
- **API Projesi**: `LorePdks.API\appsettings.Development.json` → `"CurrentVersion": "1.0.4"`

### 2. PowerShell Script Seçenekleri

#### Option 1: Mevcut Versiyonu Kullan
- Hiçbir değişiklik yapmaz
- Sadece mevcut versiyon ile publish/setup oluşturur

#### Option 2: Yeni Versiyon Numarası Gir
- Kullanıcıdan yeni versiyon numarası ister
- **DataOku** `appsettings.json` dosyasını günceller
- **API** `appsettings.Development.json` dosyasındaki `DataOkuConsole:CurrentVersion`'ı günceller
- `installer.iss` dosyasını günceller

#### Option 3: Otomatik Versiyon Artır (ÖNERİLEN)
- Son rakamı otomatik olarak +1 artırır (örn: 1.0.3 → 1.0.4)
- **DataOku** `appsettings.json` dosyasını günceller
- **API** `appsettings.Development.json` dosyasındaki `DataOkuConsole:CurrentVersion`'ı günceller
- `installer.iss` dosyasını günceller

### 3. Güncellenen Dosyalar

#### DataOku Projesi
```json
{
  "AppSettings": {
    "Version": "1.0.4"  // ← Bu güncellenir
  }
}
```

#### API Projesi
```json
{
  "DataOkuConsole": {
    "CurrentVersion": "1.0.4",  // ← Bu güncellenir
    "SetupUrl": "https://localhost:44374/Api/DataOkuConsoleSetup/DownloadUpdate"
  }
}
```

#### Installer Configuration
```iss
#define MyAppVersion "1.0.4"  // ← Bu güncellenir
```

## Kullanım Adımları

### 1. PowerShell Scriptini Çalıştır
```powershell
cd "D:\Development\ozel\TemhaPdks\SetupAndDosyaOku\Lore.SetupAndDosyaOku"
powershell -ExecutionPolicy Bypass -File "PublishAndCreateSetup.ps1"
```

### 2. Option 3'ü Seç (Otomatik Versiyon Artır)
```
Versiyon guncelleme secenekleri:
  [1] Mevcut versiyonu kullan (1.0.4)
  [2] Yeni versiyon numarasi gir
  [3] Otomatik versiyon artir (son rakami +1)  ← BU SEÇENEĞİ SEÇ

Seciminizi yapin (1-3): 3
```

### 3. Onaylama
```
Versiyon artirildi: 1.0.4 -> 1.0.5
Bu degisikligi kaydetmek istiyor musunuz? (E/H): E  ← E'YE BAS
```

### 4. Sonuç
```
Versiyon guncelleniyor...
appsettings.json guncellendi
installer.iss guncellendi
API appsettings.Development.json DataOkuConsole:CurrentVersion guncellendi
API DataOkuConsole:CurrentVersion guncellendi: 1.0.5
Versiyon guncellendi: 1.0.5
```

## Faydalar

### ✅ Otomatik Senkronizasyon
- DataOku ve API projelerindeki versiyon numaraları her zaman senkronize
- Manuel güncelleme hatası riski elimine edildi

### ✅ Tek Tıkla İşlem
- Tek script ile hem DataOku hem API versiyonları güncellenir
- Publish ve setup oluşturma işlemleri otomatik

### ✅ Hata Önleme
- Client-Server arasında versiyon uyumsuzluğu önlenir
- Update kontrolü doğru çalışır

### ✅ Geliştirici Dostu
- Basit menü sistemi
- Detaylı log çıktıları
- Onaylama adımları

## Teknik Detaylar

### PowerShell Script Yapısı
1. **Versiyon Okuma**: DataOku `appsettings.json`'dan mevcut versiyon okunur
2. **Kullanıcı Seçimi**: 3 seçenek sunulur
3. **Dosya Güncellemeleri**: 
   - DataOku `appsettings.json`
   - API `appsettings.Development.json` 
   - `installer.iss`
4. **Build & Publish**: .NET publish işlemi
5. **Setup Oluşturma**: InnoSetup ile installer oluşturma

### API Endpoint Kontrolü
DataOku uygulaması şu endpoint'i kullanarak versiyon kontrolü yapar:
```
GET /Api/DataOkuConsoleSetup/CheckVersion
```

Bu endpoint `appsettings.Development.json`'daki `DataOkuConsole:CurrentVersion` değerini döner.

## Sorun Giderme

### Problem: API'deki versiyon güncellenmiyor
**Çözüm**: PowerShell scriptinin API dosya yolunu kontrol edin:
```powershell
$apiSettingsPath = "..\..\LorePdks.API\appsettings.Development.json"
```

### Problem: JSON comment hatası
**Uyarı**: Bu normal bir durumdur. API'deki JSON dosyasında comment'ler var ama PowerShell scripti düzgün çalışır.

### Problem: Setup dosyası oluşmuyor
**Kontrol**: InnoSetup'ın kurulu olduğunu doğrulayın:
```
C:\Program Files (x86)\Inno Setup 6\ISCC.exe
```

## Sonuç

Bu sistem ile DataOku ve API projelerindeki versiyon numaraları otomatik olarak senkronize ediliyor. Option 3 kullanarak tek tıkla versiyon artırımı ve tüm dosyaların güncellenmesi sağlanıyor.

**Önerilen Workflow:**
1. Geliştirme tamamlandıktan sonra
2. PowerShell scriptini çalıştır
3. Option 3'ü seç
4. Onaylama yap
5. Test et ve deploy et
