using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LorePdks.COMMON.Extensions
{
    public static class GeneralExtensions
    {
        /// <summary>
        /// bir objeyi generic şekilde clonlamaya yarar referanstan kuratarmakgerektiğinde kullanılabilir - Ahmet     asdasdas
        /// c
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Clone<T>(this T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public static List<T> ShuffleList<T>(List<T> list)
        {
            List<T> randomizedList = new List<T>();
            Random rnd = new Random();
            while (list.Count > 0)
            {
                int index = rnd.Next(0, list.Count); //pick a random item from the master list
                randomizedList.Add(list[index]); //place it at the end of the randomized list
                list.RemoveAt(index);
            }
            return randomizedList;
        }

        /// <summary>
        /// listeyi istenilen sayıda birden çok listeye böler
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="locations"></param>
        /// <param name="nSize"></param>
        /// <returns></returns>
        public static IEnumerable<List<T>> splitList<T>(this List<T> locations, int nSize = 999)
        {
            for (int i = 0; i < locations.Count; i += nSize)
            {
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
            }
        }


        /// <summary>
        /// listi aralara vilgur koyarak stringe çevirir
        /// // model gönderme düz listler içindir model gönderirsen deserilaze ve serilazsi işlemlerini kendin yap
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ListToString(List<string> list, string ayrac = ",")
        {
            return string.Join(ayrac, list);
        }

        /// <summary>
        /// stringi belirtilen ayraç ile default vilgurdur  liste çevirir
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<string> StringToList(string metin, string ayrac = ",")
        {

            var listArry = metin.Split(ayrac);

            List<string> returnList = listArry.ToList();

            return returnList;
        }



        private const string _alg = "HmacSHA256"; //bu algoritma name bu değişmeycek
        private const string _salt = "sNl2024posSaE";

        private const int _tokenExpMin = 5;


        public static string getTokenUret(string tokenUretimStr)
        {
            Guid guid = Guid.NewGuid();
            string guidStr = Convert.ToString(guid);

            string hash = string.Join(":", new string[] { tokenUretimStr, Convert.ToString(guidStr) });
            string hashLeft = "";
            string hashRight = "";
            using (HMAC hmac = HMAC.Create(_alg))
            {
                hmac.Key = Encoding.UTF8.GetBytes(guidStr);
                hmac.ComputeHash(Encoding.UTF8.GetBytes(hash));
                hashLeft = Convert.ToBase64String(hmac.Hash);
                // hashRight = string.Join(":", new string[] { userName, guidStr });
            }

            string token = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Join(":", hashLeft, hashRight)));

            return token;


        }

        public static string GetHashedPassword(string password)
        {
            string key = string.Join(":", new string[] { password, _salt });
            using (HMAC hmac = HMAC.Create(_alg))
            {
                // Hash the key.
                hmac.Key = Encoding.UTF8.GetBytes(_salt);
                hmac.ComputeHash(Encoding.UTF8.GetBytes(key));
                return Convert.ToBase64String(hmac.Hash);
            }
        }



        /// <summary>
        /// //////
        /// </summary>
        /// <param name="karakterSayisi"></param>
        /// <returns></returns>
        public static string getSifreUret(int karakterSayisi)
        {
            Random rastgele = new Random();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < karakterSayisi; i++)
            {
                int secim = rastgele.Next(1, 4);

                int ascii = 65;


                switch (secim)
                {
                    case 1:
                        //büyük harf  65, 90
                        ascii = rastgele.Next(65, 91);//91 dahil değildir
                        break;
                    case 2:
                        //küçük harf 97, 122
                        ascii = rastgele.Next(97, 123);
                        break;
                    case 3:
                        //rakam 48, 57
                        ascii = rastgele.Next(48, 58);
                        break;

                    default:
                        break;
                }

                char karakter = Convert.ToChar(ascii);
                sb.Append(karakter);

            }
            return sb.ToString();
        }


    }
}
