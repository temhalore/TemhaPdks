using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace LorePdks.COMMON.Helpers
{
    public static class SerializerHelper
    {
        public static T DeserializeObject<T>(string stringObject) where T : class
        {
            var data = JsonConvert.DeserializeObject<T>(stringObject);
            return data;
        }

        public static string SeserializeObject(object entity)
        {
            var data = JsonConvert.SerializeObject(entity);
            return data;
        }

        public static string SerializeXml<T>(T entity) where T : class
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var serializer = new XmlSerializer(typeof(T));
            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                OmitXmlDeclaration = true,

            };
            var builder = new StringBuilder();
            using (var writer = XmlWriter.Create(builder, settings))
            {
                serializer.Serialize(writer, entity, ns);
            }
            return builder.ToString();
        }

        public static T Deserialize<T>(string input) where T : class
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));

            using (StringReader sr = new StringReader(input))
            {
                return (T)ser.Deserialize(sr);
            }
        }

    }
}
