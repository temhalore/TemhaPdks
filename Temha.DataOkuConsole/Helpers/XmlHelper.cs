using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LorePdks.COMMON.Helpers
{
    public static class XmlHelper
    {

        /// <summary>
        /// json stringi soap stringe dönüştürmeye yarar
        /// </summary>
        /// <param name="xmlBobyData"></param>
        /// <returns></returns>
        public static string getSoapRequestData(string xmlBobyData)
        {
            string xmlData = string.Format(@"<?xml version=""1.0"" encoding=""utf-8""?>
                            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                              <soap:Body>
                               {0}
                              </soap:Body>
                            </soap:Envelope>", xmlBobyData);
            return xmlData;

        }

        /// <summary>
        /// xml stringteki tag verilen tag name in içeriğini yani innertext ini verir
        /// </summary>
        /// <param name="xmlData"></param>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public static string XmlElementValueByTagName(this string xmlData, string tagName)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlData);
            var xmlElement = xmlDocument.GetElementsByTagName(tagName);
            return xmlElement.Count > 0 ? xmlElement[0].InnerText : "";

        }

    }
}
