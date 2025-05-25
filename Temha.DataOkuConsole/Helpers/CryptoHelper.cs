
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace LorePdks.COMMON.Helpers
{
    public class CryptoHelper
    {
        //private static readonly string passphrase = "D11BB480FCC14E0783D5CAC42075C27649028F1D9D56498E949D83E8BE5EFB0A";
        private static string passphrase = "EDBECAE71CD278C"; //"EDBECAE749BD439EB5D640A7E1CD278C"
        private static string passphraseFoto = "EDBECAE749BD439EB5D640A7E1CD278C";
        private static readonly SymmetricAlgorithm _algorithm = new RijndaelManaged();



        static CryptoHelper()
        {
            GetKey(passphrase);
            //passphrase = Guid.NewGuid().ToString("N").ToUpper();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string EncryptFotoString(string message, bool urlEncode = true)
        {
            //long userID = 0;
            //var principal = System.Threading.Thread.CurrentPrincipal;
            //if (principal != null && principal.Identity != null && !String.IsNullOrWhiteSpace(principal.Identity.Name))
            //    userID = Convert.ToInt64(principal.Identity.Name);

            byte[] results;

            UTF8Encoding utf8 = new UTF8Encoding();
            MD5CryptoServiceProvider hashProvider = new MD5CryptoServiceProvider();

            byte[] tdesKey = hashProvider.ComputeHash(utf8.GetBytes(passphraseFoto));

            TripleDESCryptoServiceProvider tdesAlgorithm = new TripleDESCryptoServiceProvider();
            tdesAlgorithm.Key = tdesKey;
            tdesAlgorithm.Mode = CipherMode.ECB;
            tdesAlgorithm.Padding = PaddingMode.PKCS7;


            byte[] dataToEncrypt = utf8.GetBytes(message);

            try
            {
                ICryptoTransform encryptor = tdesAlgorithm.CreateEncryptor();
                results = encryptor.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
            }
            finally
            {
                tdesAlgorithm.Clear();
                hashProvider.Clear();
            }

            var val = Convert.ToBase64String(results);
            if (urlEncode)
                return HttpUtility.UrlEncode(val);
            else
                return val;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static string DecryptFotoString(string message, bool urlDecode = true)
        {
            //long userID = 0;
            //var principal = System.Threading.Thread.CurrentPrincipal;
            //if (principal != null && principal.Identity != null && !String.IsNullOrWhiteSpace(principal.Identity.Name))
            //    userID = Convert.ToInt64(principal.Identity.Name);

            if (string.IsNullOrEmpty(message)) return message;

            if (urlDecode)
                message = HttpUtility.UrlDecode(message);

            byte[] results;

            UTF8Encoding utf8 = new UTF8Encoding();
            MD5CryptoServiceProvider hashProvider = new MD5CryptoServiceProvider();

            byte[] tdesKey = hashProvider.ComputeHash(utf8.GetBytes(passphraseFoto));
            TripleDESCryptoServiceProvider tdesAlgorithm = new TripleDESCryptoServiceProvider();

            tdesAlgorithm.Key = tdesKey;
            tdesAlgorithm.Mode = CipherMode.ECB;
            tdesAlgorithm.Padding = PaddingMode.PKCS7;

            message = message.Replace(' ', '+');
            message = message + new string('=', (4 - message.Length % 4) % 4);

            byte[] dataToDecrypt = Convert.FromBase64String(message);

            try
            {
                ICryptoTransform decryptor = tdesAlgorithm.CreateDecryptor();
                results = decryptor.TransformFinalBlock(dataToDecrypt, 0, dataToDecrypt.Length);
            }
            catch
            {
                throw new Exception("Decription Exception");
            }
            finally
            {
                tdesAlgorithm.Clear();
                hashProvider.Clear();
            }

            return utf8.GetString(results, 0, results.Length);
        }






        //oystoken ve ip adresi ile şifreler
        public static string EncryptString(string message, bool urlEncode = false, bool ipAdresVeTokenKat = true)
        {

            byte[] results;
            // şimdilik kapattık eski hale getirdik ae 22.09.22
            //var aaa = IPAddressHelper.GetIPAddress();
            //////if (ipAdresVeTokenKat)
            //////{
            //////    //var ipAdres = RequestHelper.GetIPAddress();
            //////    var oysToken = RequestHelper.GetOysTokenOgrenciBilgisiString();

            //////    //if (ipAdres.Length >= 3)
            //////    //    ipAdres = ipAdres.Substring(0, 3);
            //////    message = message + ":" + oysToken;// + ":" + ipAdres;
            //////}


            UTF8Encoding utf8 = new UTF8Encoding();
            MD5CryptoServiceProvider hashProvider = new MD5CryptoServiceProvider();

            byte[] tdesKey = hashProvider.ComputeHash(utf8.GetBytes(passphrase));

            TripleDESCryptoServiceProvider tdesAlgorithm = new TripleDESCryptoServiceProvider();
            tdesAlgorithm.Key = tdesKey;
            tdesAlgorithm.Mode = CipherMode.ECB;
            tdesAlgorithm.Padding = PaddingMode.PKCS7;

            byte[] dataToEncrypt = utf8.GetBytes(message);

            try
            {
                ICryptoTransform encryptor = tdesAlgorithm.CreateEncryptor();
                results = encryptor.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
            }
            finally
            {
                tdesAlgorithm.Clear();
                hashProvider.Clear();
            }

            var val = Convert.ToBase64String(results);
            //System.Web.HttpServerUtility.UrlTokenEncode(results);

            //if (urlEncode)
            //    return val;
            //else
            return ToHexString(val);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static string DecryptString(string message, bool urlDecode = false, bool ipAdresVeTokenKat = true)
        {
            //if (urlDecode)
            //    message = HttpUtility.UrlDecode(message);




            message = FromHexString(message);

            byte[] results;

            UTF8Encoding utf8 = new UTF8Encoding();
            MD5CryptoServiceProvider hashProvider = new MD5CryptoServiceProvider();

            byte[] tdesKey = hashProvider.ComputeHash(utf8.GetBytes(passphrase));
            TripleDESCryptoServiceProvider tdesAlgorithm = new TripleDESCryptoServiceProvider();

            tdesAlgorithm.Key = tdesKey;
            tdesAlgorithm.Mode = CipherMode.ECB;
            tdesAlgorithm.Padding = PaddingMode.PKCS7;

            byte[] dataToDecrypt = Convert.FromBase64String(message);
            //System.Web.HttpServerUtility.UrlTokenDecode(message);

            try
            {
                ICryptoTransform decryptor = tdesAlgorithm.CreateDecryptor();
                results = decryptor.TransformFinalBlock(dataToDecrypt, 0, dataToDecrypt.Length);
            }
            catch (Exception ex)
            {
                throw new Exception("Decription Exception: " + ex.Message);
            }
            finally
            {
                tdesAlgorithm.Clear();
                hashProvider.Clear();
            }


            var messageCozulen = utf8.GetString(results, 0, results.Length);

            var sonuc = messageCozulen;
            // kontrollllll
            // şimdilik kapattık eski hale getirdik ae 22.09.22
            //////if (ipAdresVeTokenKat)
            //////{

            //////    //var ipAdres = RequestHelper.GetIPAddress();
            //////    //if (ipAdres.Length > 3)
            //////    //    ipAdres = ipAdres.Substring(0, 3);
            //////    var oysToken = RequestHelper.GetOysTokenOgrenciBilgisiString();              

            //////    var mesajSplitHali = messageCozulen.Split(":");

            //////    if (mesajSplitHali.Count() != 2)
            //////    {
            //////        throw new Exception("Çöüzmlemede bir hata oluştu.Hata:0 | " + messageCozulen + " | " + oysToken);
            //////    }

            //////    if (oysToken != "" && !mesajSplitHali[1].ToString().Contains(oysToken))
            //////    {
            //////        throw new Exception("Başkasının işlemini yapmaya çalışıyor gibi görünüyorsunuz.Hata:1 | " + messageCozulen +" | "+ oysToken);
            //////    }
            //////    //if (!mesajSplitHali[2].ToString().Contains(ipAdres))
            //////    //{
            //////    //    throw new Exception("Başkasının işlemini yapmaya çalışıyor gibi görünüyorsunuz.Hata:2");
            //////    //}

            //////    sonuc = mesajSplitHali[0].ToString();
            //////}

            return sonuc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CalculateMD5Hash(string input)
        {
            MD5 md5 = MD5.Create();

            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("X2"));

            return sb.ToString().ToUpper();
        }

        public static string ToHexString(string str)
        {
            var sb = new StringBuilder();

            var bytes = Encoding.Unicode.GetBytes(str);
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString(); // returns: "48656C6C6F20776F726C64" for "Hello world"
        }

        public static string FromHexString(string hexString)
        {
            try
            {
                var bytes = new byte[hexString.Length / 2];
                for (var i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                }

                return Encoding.Unicode.GetString(bytes); // returns: "Hello world" for "48656C6C6F20776F726C64"

            }
            catch (Exception ex)
            {

                throw;
                //throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, ex.Message);
            }
        }

        public static byte[] EncryptData(byte[] data)
        {
            ICryptoTransform encryptor = _algorithm.CreateEncryptor();

            byte[] cryptoData = encryptor.TransformFinalBlock(data, 0, data.Length);

            return cryptoData;
        }

        public static byte[] DecryptData(byte[] cryptoData)
        {
            ICryptoTransform decryptor = _algorithm.CreateDecryptor();

            byte[] data = decryptor.TransformFinalBlock(cryptoData, 0, cryptoData.Length);

            return data;
        }

        private static void GetKey(string password)
        {
            byte[] salt = new byte[8];

            byte[] passwordBytes = Encoding.ASCII.GetBytes(password);

            int length = Math.Min(passwordBytes.Length, salt.Length);

            for (int i = 0; i < length; i++)
                salt[i] = passwordBytes[i];

            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt);

            _algorithm.Key = key.GetBytes(_algorithm.KeySize / 8);
            _algorithm.IV = key.GetBytes(_algorithm.BlockSize / 8);

        }

        public static string GenerateSHA512(string text, string salt = null)
        {
            if (string.IsNullOrEmpty(salt))
                salt = Guid.NewGuid().ToString("N");

            // sha512 hash
            SHA512Managed sha512 = new SHA512Managed();
            byte[] passBytes = Encoding.UTF8.GetBytes(string.Concat(text, salt));
            byte[] hashBytes = sha512.ComputeHash(passBytes);

            // hash add salt
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
            byte[] hashWithSaltBytes = new byte[hashBytes.Length + saltBytes.Length];

            for (int i = 0; i < hashBytes.Length; i++)
                hashWithSaltBytes[i] = hashBytes[i];

            for (int i = 0; i < saltBytes.Length; i++)
                hashWithSaltBytes[hashBytes.Length + i] = saltBytes[i];

            return Convert.ToBase64String(hashWithSaltBytes);
        }

        public static bool ValidateSHA512(string text, string compareHash)
        {
            try
            {
                int hashSizeInBytes = 64;
                byte[] hashWithSaltBytes = Convert.FromBase64String(compareHash);

                if (hashWithSaltBytes.Length < hashSizeInBytes)
                    return false;

                byte[] saltBytes = new byte[hashWithSaltBytes.Length - hashSizeInBytes];

                for (int i = 0; i < saltBytes.Length; i++)
                    saltBytes[i] = hashWithSaltBytes[hashSizeInBytes + i];

                var salt = Encoding.UTF8.GetString(saltBytes);
                string expectedHash = GenerateSHA512(text, salt);

                return compareHash == expectedHash;
            }
            catch (Exception)
            {
                return false;
            }

        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


    }
}
