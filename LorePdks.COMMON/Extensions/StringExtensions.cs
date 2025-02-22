
using HtmlAgilityPack;
using LorePdks.COMMON.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace LorePdks.COMMON.Extensions
{
    public static class StringExtensions
    {
        public static string ToASCIINumber(this string value)
        {

            var result = "";
            foreach (var c in value)
            {
                if (char.IsNumber(c))
                    result += c;
                else if (char.IsLetter(c))
                    result += ((int)c).ToString();
            }
            return result;
        }
        public static string toStringByFormat(this DateTime? value, string format = "")
        {
            var result = "";
            var defaultFormat = "yyyy-MM-ddTHH:mm:ss";
            if (value != null)
            {
                result = value != null ? value?.ToString(defaultFormat) : "";
                if (string.IsNullOrEmpty(format)) format = defaultFormat;

                if (!string.IsNullOrEmpty(format))
                {
                    try
                    {
                        result = value != null ? value?.ToString(format) : "";
                    }
                    catch (Exception)
                    {
                        result = value != null ? value?.ToString(defaultFormat) : "";

                    }
                }


            }
            else
            {
                result = "";
            }


            return result;
        }

        public static string ToQueryString(NameValueCollection queryData)
        {
            var array = (from key in queryData.AllKeys
                         from value in queryData.GetValues(key)
                         select string.Format(CultureInfo.InvariantCulture, "{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value)))
                .ToArray();
            return "?" + string.Join("&", array);
        }

        public static string ToUpperFirsts(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            input = input.Trim();
            return string.Join(" ", input.Split(' ').Select(d => d.First().ToString().ToUpper() + d.ToLower().Substring(1)));
        }

        public static string ToCamelCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var val = input.ToUpperFirsts().Replace(" ", "");

            char[] letters = val.ToCharArray();
            letters[0] = char.ToLower(letters[0]);
            return new string(letters).ToTurkishChars();
        }

        public static string ToTurkishChars(this string text)
        {
            string[] olds = { "Ğ", "ğ", "Ü", "ü", "Ş", "ş", "İ", "ı", "Ö", "ö", "Ç", "ç" };
            string[] news = { "G", "g", "U", "u", "S", "s", "I", "i", "O", "o", "C", "c" };

            for (int i = 0; i < olds.Length; i++)
                text = text.Replace(olds[i], news[i]);

            return text;
        }

        public static string ToUrlSafe(this string text)
        {
            //String[] olds = { "Ğ", "ğ", "Ü", "ü", "Ş", "ş", "İ", "ı", "Ö", "ö", "Ç", "ç", " "  };
            //String[] news = { "g", "g", "u", "u", "s", "s", "i", "i", "o", "o", "c", "c", "-" };

            //for (int i = 0; i < olds.Length; i++)
            //    text = text.Replace(olds[i], news[i]);

            //text = text.ToLower();

            //text = Uri.UnescapeDataString(text);
            //text = text.Replace(".", "");

            //return text;

            return text;
        }

        /// <summary>
        /// Produces optional, URL-friendly version of a title, "like-this-one". 
        /// hand-tuned for speed, reflects performance refactoring contributed
        /// by John Gietzen (user otac0n) 
        /// </summary>
        public static string URLFriendly(this string value)
        {
            if (value == null)
                return "";

            var toLower = true;

            var normalised = value.Normalize(NormalizationForm.FormKD);

            const int maxlen = 8000;
            int len = normalised.Length;
            bool prevDash = false;
            var sb = new StringBuilder(len);
            char c;

            for (int i = 0; i < len; i++)
            {
                c = normalised[i];
                if (c >= 'a' && c <= 'z' || c >= '0' && c <= '9')
                {
                    if (prevDash)
                    {
                        sb.Append('-');
                        prevDash = false;
                    }
                    sb.Append(c);
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    if (prevDash)
                    {
                        sb.Append('-');
                        prevDash = false;
                    }
                    // Tricky way to convert to lowercase
                    if (toLower)
                        sb.Append((char)(c | 32));
                    else
                        sb.Append(c);
                }
                else if (c == ' ' || c == ',' || c == '.' || c == '/' || c == '\\' || c == '-' || c == '_' || c == '=')
                {
                    if (!prevDash && sb.Length > 0)
                    {
                        prevDash = true;
                    }
                }
                else
                {
                    string swap = ConvertEdgeCases(c, toLower);

                    if (swap != null)
                    {
                        if (prevDash)
                        {
                            sb.Append('-');
                            prevDash = false;
                        }
                        sb.Append(swap);
                    }
                }

                if (sb.Length == maxlen)
                    break;
            }
            return sb.ToString();
        }

        public static string[] SplitToChunks(this string source, int maxLength)
        {
            return source
                .Where((x, i) => i % maxLength == 0)
                .Select(
                    (x, i) => new string(source
                        .Skip(i * maxLength)
                        .Take(maxLength)
                        .ToArray()))
                .ToArray();
        }

        static string ConvertEdgeCases(char c, bool toLower)
        {
            string swap = null;
            switch (c)
            {
                case 'ı':
                    swap = "i";
                    break;
                case 'ł':
                    swap = "l";
                    break;
                case 'Ł':
                    swap = toLower ? "l" : "L";
                    break;
                case 'đ':
                    swap = "d";
                    break;
                case 'ß':
                    swap = "ss";
                    break;
                case 'ø':
                    swap = "o";
                    break;
                case 'Þ':
                    swap = "th";
                    break;
            }
            return swap;
        }

        //public static string ToInnerText(this string text)
        //{
        //    if (String.IsNullOrEmpty(text))
        //        return text;

        //    HtmlDocument doc = new HtmlDocument();
        //    doc.LoadHtml(text);
        //    var inner = doc.DocumentNode.InnerText;
        //    return WebUtility.HtmlDecode(inner);
        //}

        static public string WordEllipsis(string text, int maxLength, string ellipsis = "...")
        {
            string result;

            if (text.Length <= maxLength)
            {
                result = text;
            }
            else if (maxLength <= ellipsis.Length)
            {
                result = ellipsis.Substring(0, maxLength);
            }
            else
            {
                result = text.Substring(0, maxLength - ellipsis.Length);
                var lastWordPosition = result.LastIndexOf(' ');

                if (lastWordPosition < 0)
                {
                    lastWordPosition = 0;
                }
                result = result.Substring(0, lastWordPosition).Trim(new[] { '.', ',', '!', '?' }) + ellipsis;
            }

            return result;
        }

        static public string WordGizle(string text, int gosterBastan, int gosterSondan, string gizliKarakter = "******")
        {
            string result;

            if (text.Length <= 0) result = text;
            if (text.Length <= gosterBastan && gosterBastan > 0) gosterBastan = 1;
            if (text.Length <= gosterSondan && gosterSondan > 0) gosterSondan = 1;

            result = text.Substring(0, gosterBastan) + gizliKarakter + text.Substring(text.Length - gosterSondan, gosterSondan);

            return result;
        }



        public static string ToSqlLikeFriendly(this string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var removeBadChar = new List<string> { "%", "'", "&", "|", "\\", "!", "$", "@", "#" };
                foreach (var ch in removeBadChar)
                    text = text.Replace(ch, "");
                return $"%{text.ToLower()}%";
            }
            return "%%";
        }

        public static string RemoveDiacritics(this string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                    if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                    {
                        stringBuilder.Append(c);
                    }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string ToInnerText(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(text);
            var inner = doc.DocumentNode.InnerText;
            return WebUtility.HtmlDecode(inner);
        }

        //string içindeki sayıları verir
        public static string getStringInNumbers(string s)
        {
            string donus = "";
            foreach (char c in s)
            {
                if (char.IsDigit(c))
                    donus += c;
            }
            return donus;
        }

        public static string ToUpperFirstLetter(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            source = source.ToLower();
            // convert to char array of the string
            char[] letters = source.ToCharArray();
            // upper case the first char
            letters[0] = char.ToUpper(letters[0]);
            // return the array made of the new char array
            return new string(letters);
        }

        public static string ToUpperTurkce(string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;

            return source.ToUpper(new CultureInfo("tr-TR", false));
        }


        public static string convertStringForSimilarity(string str)
        {

            if (string.IsNullOrEmpty(str))
            {
                return "";
            }

            //her boşluktan sonrasını kelime olarak kabul eder ve ayır her kelimeyi büyük harfe çevirir ve birleştirir
            string[] stringList = str.Split(' ');
            StringBuilder sb = new StringBuilder();
            foreach (var s in stringList)
            {
                sb.Append(ToUpperTurkce(s.Trim()));
            }

            str = Convert.ToString(sb.ToString());

            return str;
        }

        ////iki metin arasındaki benzerlik oranını verir
        //public static Double getBenzerlikOrani(String metin1, String metin2)
        //{
        //    NormalizedLevenshtein smlr = new NormalizedLevenshtein();
        //    Double benzerlik = 0.0;
        //    benzerlik = smlr.Similarity(convertStringForSimilarity(metin1), convertStringForSimilarity(metin2));
        //    return benzerlik;
        //}

        ////verilen oranı geçen benzerlik değeri varsa benzer true olarak döner
        //public static bool getBenzerMi(string metin1, string metin2, Double limit)
        //{
        //    bool benzer = false;
        //    NormalizedLevenshtein smlr = new NormalizedLevenshtein();
        //    Double benzerlik = 0.0;
        //    benzerlik = smlr.Similarity(convertStringForSimilarity(metin1), convertStringForSimilarity(metin2));

        //    if (benzerlik >= limit)
        //    {
        //        benzer = true;
        //    }
        //    return benzer;
        //}

        ////verilen oranı geçen benzerlik değeri varsa benzer true olarak döner
        //public static bool getBenzerMi(List<string> stringList, string text, double limit)
        //{
        //    if (stringList == null || stringList.Count == 0)
        //    {
        //        return false;
        //    }

        //    foreach (var kontrolItem in stringList)
        //    {
        //        //kontrol edilecek olanı listeden ayır
        //        foreach (var item in stringList.Where(x => x != kontrolItem).ToList())
        //        {
        //            if (getBenzerMi(item, kontrolItem, Convert.ToDouble(limit)))
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    return false;
        //}

        public static int ToDecryptInt(this string value)
        {
            try
            {
                return Convert.ToInt32(CryptoHelper.DecryptString(value));
            }
            catch (FormatException)
            {
                return 0;
            }
        }
        public static int ToInt32(this string value)
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch (FormatException)
            {
                return 0;
            }
        }
        /// <summary>
        /// Mesaj datasından Base64 image alanını silmek için
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string RemoveMessageBase64ImageData(this string message)
        {
            try
            {
                var ind = message.IndexOf("data:image/jpg");
                if (ind > 0)
                    return message.Replace(message.Substring(ind, message.Substring(ind).IndexOf("==") + 2), "");
                if (message.Length > 3999)
                    return message.Substring(0, 3999);
            }
            catch (Exception)
            {
                return message;
            }

            return message;
        }
        public static string Decode(this string input)
        {
            byte[] decbuff = Convert.FromBase64String(input.Replace(",", "=").Replace("-", "+").Replace("/", "_"));
            return Encoding.UTF8.GetString(decbuff);
        }

        public static string Encode(this string input)
        {
            byte[] encbuff = Encoding.UTF8.GetBytes(input ?? "");
            return Convert.ToBase64String(encbuff).Replace("=", ",").Replace("+", "-").Replace("_", "/");
        }









    }
}
