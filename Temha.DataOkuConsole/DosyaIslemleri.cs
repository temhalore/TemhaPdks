public static class DosyaIslemleri
{
    private static readonly int MaxRetryAttempts = 5;
    private static readonly int RetryDelayMs = 1000;
    private static readonly int FileCheckTimeoutMs = 30000;

    private static bool IsFileLocked(FileInfo file)
    {
        try
        {
            using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
            {
                stream.Close();
            }
            return false;
        }
        catch (IOException)
        {
            return true;
        }
    }

    private static bool DosyaIsleminiGerceklestir(string dosyaYolu, Action<string> islemDelegate)
    {
        var dosya = new FileInfo(dosyaYolu);
        var startTime = DateTime.Now;
        var attempt = 1;

        while (attempt <= MaxRetryAttempts)
        {
            try
            {
                if (dosya.Exists && IsFileLocked(dosya))
                {
                    var elapsedTime = DateTime.Now - startTime;
                    if (elapsedTime.TotalMilliseconds >= FileCheckTimeoutMs)
                    {
                        Console.WriteLine($"Dosya zaman aşımı: {dosyaYolu}");
                        return false;
                    }

                    Console.WriteLine($"Dosya meşgul: {dosyaYolu} - Deneme {attempt}/{MaxRetryAttempts}");
                    Thread.Sleep(RetryDelayMs);
                    attempt++;
                    continue;
                }

                islemDelegate(dosyaYolu);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Dosya işlemi hatası (Deneme {attempt}/{MaxRetryAttempts}): {ex.Message}");
                if (attempt == MaxRetryAttempts) throw;
                Thread.Sleep(RetryDelayMs);
                attempt++;
            }
        }
        return false;
    }

    public static void DosyaKlasorKontrol(string dosyaYolu)
    {
        try
        {
            if (string.IsNullOrEmpty(dosyaYolu))
                return;

            string klasorYolu = Path.GetDirectoryName(dosyaYolu);

            if (!Directory.Exists(klasorYolu))
            {
                Directory.CreateDirectory(klasorYolu);
                Console.WriteLine($"Klasör oluşturuldu: {klasorYolu}");
            }

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
            string yedekKlasor = Path.Combine(Path.GetDirectoryName(kaynak), "yedek");
            string yedekDosya = Path.Combine(yedekKlasor,
                $"{yedekEk}_{tarihEk}_{Path.GetFileName(kaynak)}");

            DosyaKopyala(kaynak, yedekDosya);
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
            DosyaIsleminiGerceklestir(dosyaYolu, (yol) =>
            {
                File.WriteAllText(yol, icerik);
            });
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
            DosyaIsleminiGerceklestir(dosyaYolu, (yol) =>
            {
                File.AppendAllText(yol, icerik + Environment.NewLine);
            });
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

            DosyaIsleminiGerceklestir(kaynak, (yol) =>
            {
                File.Copy(yol, hedef, true);
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Dosya kopyalama hatası: {ex.Message}");
        }
    }

    public static void DosyaSil(string dosyaYolu, bool yedekAl = true, string yedekOncesiEkIfade = "")
    {
        try
        {
            if (!File.Exists(dosyaYolu))
                return;

            if (yedekAl)
            {
                DosyaYedekAl(dosyaYolu, yedekOncesiEkIfade);
            }

            DosyaIsleminiGerceklestir(dosyaYolu, (yol) =>
            {
                File.Delete(yol);
            });
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

            string icerik = string.Empty;
            DosyaIsleminiGerceklestir(dosyaYolu, (yol) =>
            {
                icerik = File.ReadAllText(yol);
            });
            return icerik;
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

            string[] satirlar = null;
            DosyaIsleminiGerceklestir(dosyaYolu, (yol) =>
            {
                satirlar = File.ReadAllLines(yol);
            });
            return satirlar ?? new string[0];
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
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Dosya temizleme hatası: {ex.Message}");
        }
    }
}