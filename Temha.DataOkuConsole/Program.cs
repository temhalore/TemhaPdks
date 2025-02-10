using System;
using System.IO;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("Lütfen okumak istediğiniz txt dosyasının yolunu giriniz:");
            string kaynakDosyaYolu = Console.ReadLine();

            if (File.Exists(kaynakDosyaYolu))
            {
                // Kopya dosya için yol oluşturma
                string kopyaDosyaYolu = Path.Combine(
                    Path.GetDirectoryName(kaynakDosyaYolu),
                    $"kopya_{Path.GetFileName(kaynakDosyaYolu)}");

                // Hatalı satırlar için dosya yolu
                string hataliSatirlarDosyaYolu = Path.Combine(
                    Path.GetDirectoryName(kaynakDosyaYolu),
                    "hatalilar.txt");

                // Dosyanın kopyasını oluştur
                File.Copy(kaynakDosyaYolu, kopyaDosyaYolu, true);

                // Ana dosyayı temizle
                File.WriteAllText(kaynakDosyaYolu, string.Empty);

                int satirSayisi = 0;
                int hataliSatirSayisi = 0;

                Console.WriteLine("\nDosya içeriği:\n");

                // Kopya dosyayı satır satır oku
                using (StreamReader sr = new StreamReader(kopyaDosyaYolu))
                {
                    string satir;
                    while ((satir = sr.ReadLine()) != null)
                    {
                        satirSayisi++;
                        try
                        {
                            //burada http post metodunu firma için çağıracağız.App settingsten alacağız firma bilgisini
                        }
                        catch (Exception ex)
                        {
                            hataliSatirSayisi++;
                            // Hatalı satırı hatalilar.txt dosyasına yaz
                            using (StreamWriter sw = new StreamWriter(hataliSatirlarDosyaYolu, true))
                            {
                                sw.WriteLine($"Satır {satirSayisi}: {satir} - Hata: {ex.Message}");
                            }
                            Console.WriteLine($"Hata - Satır {satirSayisi}: {ex.Message}");
                        }
                    }
                }

                // İşlem bittikten sonra kopya dosyayı sil
                File.Delete(kopyaDosyaYolu);

                Console.WriteLine($"\nToplam {satirSayisi} satır okundu.");
                Console.WriteLine($"Toplam {hataliSatirSayisi} hatalı satır bulundu.");

                if (hataliSatirSayisi > 0)
                {
                    Console.WriteLine($"Hatalı satırlar '{hataliSatirlarDosyaYolu}' dosyasına kaydedildi.");
                }
            }
            else
            {
                Console.WriteLine("Belirtilen dosya bulunamadı!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Bir hata oluştu: {ex.Message}");
        }

        Console.WriteLine("\nProgramı kapatmak için bir tuşa basınız...");
        Console.ReadKey();
    }
}