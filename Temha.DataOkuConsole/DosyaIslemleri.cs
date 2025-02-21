public static class DosyaIslemleri
{

    public static void DosyaKlasorKontrol(string dosyaYolu)
    {
        try
        {
            if (string.IsNullOrEmpty(dosyaYolu))
                return;

            string klasorYolu = Path.GetDirectoryName(dosyaYolu);

            // Klasör yoksa oluştur
            if (!Directory.Exists(klasorYolu))
            {
                Directory.CreateDirectory(klasorYolu);
                Console.WriteLine($"Klasör oluşturuldu: {klasorYolu}");
            }

            // Dosya yoksa oluştur
            if (!File.Exists(dosyaYolu))
            {
                using (File.Create(dosyaYolu)) { }
                Console.WriteLine($"Dosya oluşturuldu: {dosyaYolu}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Dosya/Klasör oluşturma hatası: {ex.Message}");
        }
    }

    public static void DosyaYedekAl(string kaynak, string yedekEk)
    {
        try
        {
            if (!File.Exists(kaynak))
                return;

            var simdi = DateTime.Now;
            string tarihEk = $"{simdi:yyyyMMdd-HHmmss}";

            string yedekDosya = Path.Combine(
                Path.GetDirectoryName(kaynak)+"\\yedek",//:TODO burada yeni bir clasör oluşturma olayı gerçekleşmeli yedek klasöründe olsun tüm yedekler
                $"{yedekEk}_{tarihEk}_{Path.GetFileName(kaynak)}");

            DosyaKopyala(kaynak, yedekDosya);

           // File.Copy(kaynak, yedekDosya, true);
            Thread.Sleep(100);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Yedek alma hatası: {ex.Message}");
        }
    }

    public static void DosyayaYaz(string dosyaYolu, string icerik)
    {
        try
        {
            DosyaKlasorKontrol(dosyaYolu);

            File.WriteAllText(dosyaYolu, icerik);

            Thread.Sleep(100);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Dosyaya yazma hatası: {ex.Message}");
        }
    }
    public static void DosyayaYazYeniSatir(string dosyaYolu, string icerik)
    {
        try
        {
            DosyaKlasorKontrol(dosyaYolu);
            File.AppendAllText(dosyaYolu, icerik + Environment.NewLine);

            Thread.Sleep(100);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Dosyaya yazma hatası: {ex.Message}");
        }
    }

    public static void DosyaKopyala(string kaynak, string hedef)
    {
        try
        {
            if (!File.Exists(kaynak))
                return;

            string hedefKlasor = Path.GetDirectoryName(hedef);
            if (!Directory.Exists(hedefKlasor))
            {
                Directory.CreateDirectory(hedefKlasor);
            }

            File.Copy(kaynak, hedef, true);
            Thread.Sleep(100);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Dosya kopyalama hatası: {ex.Message}");
        }
    }

    public static void DosyaSil(string dosyaYolu, bool yedekAl = true,string yedekOncesiEkIfade = "")
    {
        try
        {
            if (!File.Exists(dosyaYolu))
                return;

            if (yedekAl)
            {
               
                DosyaYedekAl(dosyaYolu, yedekOncesiEkIfade );
            }

            File.Delete(dosyaYolu);
            Thread.Sleep(100);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Dosya silme hatası: {ex.Message}");
        }
    }

    public static string DosyaOku(string dosyaYolu)
    {
        try
        {
            if (!File.Exists(dosyaYolu))
                return string.Empty;

            return File.ReadAllText(dosyaYolu);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Dosya okuma hatası: {ex.Message}");
            return string.Empty;
        }
    }

    public static string[] DosyaSatirlariniOku(string dosyaYolu)
    {
        try
        {
            if (!File.Exists(dosyaYolu))
                return new string[0];

            return File.ReadAllLines(dosyaYolu);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Dosya satırları okuma hatası: {ex.Message}");
            return new string[0];
        }
    }

    public static void DosyaTemizle(string dosyaYolu, bool yedekAl = true)
    {
        try
        {
            DosyayaYaz(dosyaYolu, "");
            Thread.Sleep(100);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Dosya temizleme hatası: {ex.Message}");
        }
    }


}