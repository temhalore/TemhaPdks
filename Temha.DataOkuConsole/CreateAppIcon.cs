using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace IconCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Icon boyutu
                int size = 64;
                
                // Bitmap oluştur
                using (Bitmap bitmap = new Bitmap(size, size))
                {
                    // Graphics object oluştur
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        // Arkaplanı doldur
                        g.Clear(Color.CornflowerBlue);
                        
                        // Harfleri çiz (T - Temha)
                        using (Font font = new Font("Arial", 32, FontStyle.Bold))
                        {
                            g.DrawString("T", font, Brushes.White, 16, 8);
                        }
                        
                        // Çerçeve çiz
                        using (Pen pen = new Pen(Color.White, 3))
                        {
                            g.DrawRectangle(pen, 3, 3, size - 6, size - 6);
                        }
                    }
                    
                    // Icon dosyası olarak kaydet
                    string iconPath = Path.Combine(
                        Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                        "app.ico");
                    
                    // Geçici olarak PNG olarak kaydet
                    string tempPngPath = Path.Combine(
                        Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                        "temp_icon.png");
                    
                    bitmap.Save(tempPngPath, ImageFormat.Png);
                    
                    // Icon olarak dönüştür
                    using (Icon icon = Icon.FromHandle(bitmap.GetHicon()))
                    {
                        using (FileStream fs = new FileStream(iconPath, FileMode.Create))
                        {
                            icon.Save(fs);
                        }
                    }
                    
                    Console.WriteLine($"Icon başarıyla oluşturuldu: {iconPath}");
                    
                    // Geçici dosyayı sil
                    try { File.Delete(tempPngPath); } catch { }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Icon oluşturulurken hata: {ex.Message}");
            }
        }
    }
}
