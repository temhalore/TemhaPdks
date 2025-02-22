using System.Net;
using System.Text;
using System.Xml;

namespace LorePdks.COMMON.Helpers
{
    public static class HttpHelper
    {
        public static string authphrase = "F783ED3D7F5B48B390A3E1D2D247DF3C4ADA8B9CBCF747C0BD48A8796350E1AD";
        public static string Get(string url, Dictionary<string, string> headers)
        {

            try
            {

                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "GET";
                httpRequest.ContentType = "application/json";
                // httpRequest.Headers.Add("auth", CryptoHelper.GenerateSHA512(authphrase));

                foreach (var item in headers)
                {
                    if (string.IsNullOrEmpty(httpRequest.Headers.Get(item.Key)))
                        httpRequest.Headers.Add(item.Key, item.Value);
                }

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();

                StringBuilder sb = new StringBuilder();
                using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                        sb.Append(line);
                }

                return sb.ToString();
            }
            catch (WebException e)
            {
                string webHata = "";
                string str = $"WebException:";
                if (e.Response != null)
                {
                    StreamReader reader1 = new StreamReader(e.Response.GetResponseStream());
                    str = str + reader1.ReadLine();
                }
                else
                {
                    str = str + e.Message;
                }



                throw new Exception(e.Message + " - " + str);
            }
            catch (Exception EX)
            {
                string headerdanYakalananHAta = ((HttpWebResponse)((WebException)EX).Response).Headers["X-hesAccountServiceApp-error"];

                if (string.IsNullOrEmpty(headerdanYakalananHAta))
                {
                    if (((HttpWebResponse)((WebException)EX).Response).ContentType.Contains("application/problem+json"))
                    {
                        headerdanYakalananHAta = "gönderdiğiniz json nesnesinde tüm gerekli alanlar olduğundan ve kurallara uygun formatta olduğundan emin olunuz.";
                    }

                }

                throw new Exception(EX.Message + " - " + headerdanYakalananHAta);
            }
            //using (StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8))
            //{
            //    var data = reader.ReadToEnd();
            //    var responseData = JsonConvert.DeserializeObject<List<T>>(data);
            //    return responseData;
            //}
        }

        public static string Post(string url, Dictionary<string, string> headers, string data)
        {
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/json";

            foreach (var item in headers)
            {
                if (string.IsNullOrEmpty(httpRequest.Headers.Get(item.Key)))
                    httpRequest.Headers.Add(item.Key, item.Value);
            }

            byte[] bytedata = Encoding.UTF8.GetBytes(data);
            httpRequest.ContentLength = bytedata.Length;

            Stream requestStream = httpRequest.GetRequestStream();
            requestStream.Write(bytedata, 0, bytedata.Length);
            requestStream.Close();

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpRequest.GetResponse();
            Stream responseStream = httpWebResponse.GetResponseStream();

            StringBuilder sb = new StringBuilder();
            using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    sb.Append(line);
            }

            return sb.ToString();
        }

        public static string Patch(string url, Dictionary<string, string> headers, string data)
        {
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "PATCH";
            httpRequest.ContentType = "application/json";

            foreach (var item in headers)
            {
                if (string.IsNullOrEmpty(httpRequest.Headers.Get(item.Key)))
                    httpRequest.Headers.Add(item.Key, item.Value);
            }

            byte[] bytedata = Encoding.UTF8.GetBytes(data);
            httpRequest.ContentLength = bytedata.Length;

            Stream requestStream = httpRequest.GetRequestStream();
            requestStream.Write(bytedata, 0, bytedata.Length);
            requestStream.Close();

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpRequest.GetResponse();
            Stream responseStream = httpWebResponse.GetResponseStream();

            StringBuilder sb = new StringBuilder();
            using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    sb.Append(line);
            }

            return sb.ToString();
        }
        public static string Put(string url, Dictionary<string, string> headers, string data)
        {
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "PUT";
            httpRequest.ContentType = "application/json";

            foreach (var item in headers)
            {
                if (string.IsNullOrEmpty(httpRequest.Headers.Get(item.Key)))
                    httpRequest.Headers.Add(item.Key, item.Value);
            }

            byte[] bytedata = Encoding.UTF8.GetBytes(data);
            httpRequest.ContentLength = bytedata.Length;

            Stream requestStream = httpRequest.GetRequestStream();
            requestStream.Write(bytedata, 0, bytedata.Length);
            requestStream.Close();

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpRequest.GetResponse();
            Stream responseStream = httpWebResponse.GetResponseStream();

            StringBuilder sb = new StringBuilder();
            using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    sb.Append(line);
            }

            return sb.ToString();
        }


        private static readonly Encoding encoding = Encoding.UTF8;

        public static HttpWebResponse MultipartFormPost(string postUrl, string userAgent, Dictionary<string, object> postParameters, Dictionary<string, string> headers)
        {
            string formDataBoundary = string.Format("----------{0:N}", Guid.NewGuid());
            string contentType = "multipart/form-data; boundary=" + formDataBoundary;

            byte[] formData = GetMultipartFormData(postParameters, formDataBoundary);

            return PostForm(postUrl, userAgent, contentType, formData, headers);
        }
        private static HttpWebResponse PostForm(string postUrl, string userAgent, string contentType, byte[] formData, Dictionary<string, string> headers)
        {
            HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;

            if (request == null)
            {
                throw new NullReferenceException("request is not a http request");
            }

            // Set up the request properties.
            request.Method = "POST";
            request.ContentType = contentType;
            request.UserAgent = userAgent;
            request.CookieContainer = new CookieContainer();
            request.ContentLength = formData.Length;

            // You could add authentication here as well if needed:
            // request.PreAuthenticate = true;
            // request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;

            //Add header if needed
            foreach (var item in headers)
            {
                if (string.IsNullOrEmpty(request.Headers.Get(item.Key)))
                    request.Headers.Add(item.Key, item.Value);
            }

            // Send the form data to the request.
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
            }

            return request.GetResponse() as HttpWebResponse;

        }

        private static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new MemoryStream();
            bool needsCLRF = false;

            foreach (var param in postParameters)
            {

                if (needsCLRF)
                    formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

                needsCLRF = true;

                if (param.Value is FileParameter) // to check if parameter if of file type
                {
                    FileParameter fileToUpload = (FileParameter)param.Value;

                    // Add just the first part of this param, since we will write the file data directly to the Stream
                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                        boundary,
                        param.Key,
                        fileToUpload.FileName ?? param.Key,
                        fileToUpload.ContentType ?? "application/octet-stream");

                    formDataStream.Write(encoding.GetBytes(header), 0, encoding.GetByteCount(header));
                    // Write the file data directly to the Stream, rather than serializing it to a string.
                    formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                }
                else
                {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                        boundary,
                        param.Key,
                        param.Value);
                    formDataStream.Write(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));
                }
            }

            // Add the end of the request.  Start with a newline
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            return formData;
        }

        public class FileParameter
        {
            public byte[] File { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
            public FileParameter(byte[] file) : this(file, null) { }
            public FileParameter(byte[] file, string filename) : this(file, filename, null) { }
            public FileParameter(byte[] file, string filename, string contenttype)
            {
                File = file;
                FileName = filename;
                ContentType = contenttype;
            }
        }


        public static string PostSoapXml(string url, string action, string xmlData)
        {
            //Making Web Request    
            HttpWebRequest Req = (HttpWebRequest)WebRequest.Create(url);
            //SOAPAction    
            Req.Headers.Add(@"SOAPAction:http://tempuri.org/" + action);
            //Content_type    
            Req.ContentType = "text/xml;charset=\"utf-8\"";
            Req.Accept = "text/xml";
            //HTTP method    
            Req.Method = "POST";
            //return HttpWebRequest    

            XmlDocument SOAPReqBody = new XmlDocument();
            //SOAP Body Request    
            SOAPReqBody.LoadXml(xmlData);

            using (Stream stream = Req.GetRequestStream())
            {
                SOAPReqBody.Save(stream);
            }
            string ServiceResult = "";
            //Geting response from request    
            using (WebResponse Serviceres = Req.GetResponse())
            {
                using (StreamReader rd = new StreamReader(Serviceres.GetResponseStream()))
                {
                    //reading stream    
                    ServiceResult = rd.ReadToEnd();
                }
            }
            return ServiceResult;
        }
    }
}
