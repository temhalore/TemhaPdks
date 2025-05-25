# Temha Data Oku Console Setup İndirici

Bu program, Temha Data Oku Console uygulamasını indirip otomatik olarak Windows servisi şeklinde kuran bir yardımcı uygulamadır.

## Özellikler

- API'den güncel uygulama versiyonunu kontrol etme
- Uygulamayı otomatik indirme
- Arşiv dosyasını açma ve kurulum yapma
- Uygulama yapılandırma dosyasını oluşturma
- **Otomatik Windows servisi kurulumu ve başlatma**
- Servis başlatma durumunu doğrulama

## Kullanım

1. Firma kodunuzu girin ve "Kontrol Et" butonuna basın
2. Geçerli bir firma kodu girildiğinde, uygulama en güncel sürümü indirecektir
3. İzlenecek dosya yolunu ve diğer ayarları yapılandırın
4. İndirme ve kurulum otomatik olarak gerçekleşecektir
5. Kurulum tamamlandığında uygulama **Windows servisi olarak** otomatik kurulacaktır
6. Servis başarıyla kurulduğunda otomatik olarak başlatılacak ve durumu doğrulanacaktır
7. Herhangi bir sorun olması durumunda, detaylı hata mesajları görüntülenecektir

## Teknik Notlar

- Kurulum işlemi sırasında `service-install` parametresi kullanılarak DataOkuConsole uygulaması Windows servisi olarak kurulur
- Yönetici hakları gerektirir (servis kurulum ve yönetimi için)
- Servis kurulduktan sonra Windows Servisler yönetim konsolundan izlenebilir ve yönetilebilir
- Kurulum sonrası servis durumu otomatik olarak kontrol edilir ve kullanıcıya bildirilir

### Komut Satırı Parametreleri

DataOkuConsole aşağıdaki komut satırı parametrelerini destekler:

```
service-install   : Windows servisi olarak kur
service-uninstall : Windows servisini kaldır
install           : Kurulum menüsü
```
