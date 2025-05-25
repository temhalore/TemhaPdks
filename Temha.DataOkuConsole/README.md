# Temha Data Oku Console

Windows hizmeti (service) olarak çalışan veri okuma ve işleme uygulaması.

## Özellikler

- Windows servis olarak çalışabilme
- Sistem tepsisinde (system tray) simge gösterme
- Windows başlangıcında otomatik çalışma
- Belirli dosyaları izleyerek otomatik veri işleme
- Yapılandırılabilir dosya konumları ve parametreler

## Kurulum ve Kullanım

### Normal Kurulum

1. Uygulamayı çalıştırın
2. Kurulum menüsünden seçiminizi yapın:
   - **Normal mod (D)**: Uygulama olarak çalıştırır
   - **Windows servisi olarak kur (W)**: Windows servisi olarak kurar ve otomatik başlatır
   - **Var olan kurulumu sıfırla (S)**: Kurulumu temizleyerek sıfırlar

### Komut Satırı Kurulum

Uygulamayı aşağıdaki parametreler ile çalıştırabilirsiniz:

- **install**: Kurulum menüsünü başlatır
- **service-install**: Windows servisi olarak doğrudan kurar
- **service-uninstall**: Windows servisini kaldırır

```powershell
# Kurulum menüsünü başlat
.\Temha.DataOkuConsole.exe install

# Doğrudan Windows servisi olarak kur
.\Temha.DataOkuConsole.exe service-install

# Windows servisini kaldır
.\Temha.DataOkuConsole.exe service-uninstall
```

## Windows Servisi Kurulduktan Sonra

Windows servisi kurulumu tamamlandıktan sonra:

1. Servis otomatik olarak başlatılacaktır
2. Sistem tepsisinde bir simge görünecektir
3. Bu simgeye sağ tıklayarak:
   - Servis durumunu kontrol edebilirsiniz
   - Dosyaları manuel olarak işleyebilirsiniz
   - Servisi kapatabilirsiniz

## Yapılandırma

Uygulama, yapılandırma için `application.json` dosyasını kullanır. Bu dosya şu konumlarda aranır:

1. `C:\TemhaPdks\application.json`
2. Uygulama dizini içinde `application.json`

Örnek yapılandırma:

```json
{
  "AppSettings": {
    "FirmaKod": "FIRMA01",
    "IzlenecekDosya": "C:\\TemhaPdks\\data\\timerecords.txt",
    "IsDebugMode": false
  },
  "CoreSettings": {
    "HataliDosya": "C:\\TemhaPdks\\data\\hatalilar.txt"
  }
}
```

## Hata Ayıklama

Debug modunu etkinleştirmek için `application.json` dosyasındaki `IsDebugMode` değerini `true` olarak ayarlayın:

```json
"IsDebugMode": true
```

Loglar şu konumda kaydedilir: `[UygulamaKlasörü]\logs\service_log.txt`
