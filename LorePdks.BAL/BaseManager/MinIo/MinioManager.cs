//using Minio;
//using Minio.DataModel;
//using Minio.Exceptions;
//using LorePdks.BAL.BaseManager.MinIo.Interfaces;
//using LorePdks.COMMON.Enums;
//using LorePdks.COMMON.Extensions;
//using LorePdks.COMMON.Models;

//namespace LorePdks.BAL.BaseManager.MinIo
//{
//    public class MinioManager : IMinioManager
//    {
//        //private static MinioClient minio = new MinioClient().WithEndpoint("minio.istanbul.edu.tr", "9000").WithCredentials("minioadmin", "EDFTR45FGTRer").Build();

//        string _minIoEndPoint = "oysminio.istanbul.edu.tr";
//        string _minIoAccessKey = "iuoysminioadmin";
//        string _minIoSecurityKey = "WMQ5SVfW72NI5iBOj2sq1q+VrOtejZBsilrSNtlW1m0+";

//        public void UploadData(Stream data, string bucketName, string directory, string fileName, string ext, long length)
//        {
//            var minioClient = new MinioClient()
//                .WithEndpoint(_minIoEndPoint)
//                .WithCredentials(_minIoAccessKey, _minIoSecurityKey)
//                .WithSSL()
//                .Build();

//            //var minioClient = new MinioClient()
//            //    .WithEndpoint("minio.istanbul.edu.tr", 9000)
//            //    .WithCredentials("iuminioadmin", "+6c7MvGt3IiblTaikWLfyIfvrYqTFv+nqVoLagxeH3s=")
//            //    .WithSSL()
//            //    .Build();

//            Upload(minioClient, bucketName, directory, data, fileName, ext).ConfigureAwait(false);
//        }

//        [Obsolete]
//        private async static Task Upload(MinioClient minio, string bucketName, string directory, Stream data, string fileName, string ext)
//        {
//            string contentType = "";

//            switch (ext.ToLower())
//            {
//                case "pdf":
//                    contentType = "application/pdf";
//                    break;
//                case "doc":
//                    contentType = "application/msword";
//                    break;
//                case "docx":
//                    contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
//                    break;
//                case "csv":
//                    contentType = "text/csv";
//                    break;
//                case "jpeg":
//                    contentType = "image/jpeg";
//                    break;
//                case "jpg":
//                    contentType = "image/jpeg";
//                    break;
//                case "png":
//                    contentType = "image/png";
//                    break;
//                case "ppt":
//                    contentType = "application/vnd.ms-powerpoint";
//                    break;
//                case "pptx":
//                    contentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
//                    break;
//                case "txt":
//                    contentType = "text/plain";
//                    break;
//                case "xls":
//                    contentType = "application/vnd.ms-excel";
//                    break;
//                case "xlsx":
//                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
//                    break;
//                case "zip":
//                    contentType = "application/zip";
//                    break;
//                case "mp4":
//                    contentType = "video/mpeg";
//                    break;
//                default:
//                    //throw new OSSException(MessageCode.ERROR_600, "Yüklemeye çalıştığınız dosya formatı desteklenmemektedir.");
//                    break;
//            }
//            string stt = "";
//            try
//            {

//                // Make a bucket on the server, if not already present.
//                stt = "1";
//                bool found = false;

//                found = returnBucketStatus(minio, bucketName, 0);

//                if (!found)
//                {
//                    stt = "2";
//                    //await kalktı getaweter result eklendi ae 10.05.2022
//                    minio.MakeBucketAsync(bucketName).ConfigureAwait(false).GetAwaiter().GetResult();
//                }

//                stt = "3";
//                // Upload a file to bucket.                
//                minio.PutObjectAsync(bucketName, directory + "/" + fileName + "." + ext, data, data.Length, contentType).ConfigureAwait(false).GetAwaiter().GetResult();

//                // var result = task.Status;

//            }
//            catch (MinioException e)
//            {
//                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, "Dosya Yükleme sırasında hata oluştu lütfen tekrar deneyin.Hata:1");
//            }
//            catch (Exception ex)
//            {
//                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, "Dosya Yükleme sırasında hata oluştu lütfen tekrar deneyin.Hata:2");
//            }




//            //kontrol için eklendi dosyagerçekten yüklenip yüklenmediğini kontrol ediyor bulunamazsa yüklenmemiştir ae 10.05.2022
//            try
//            {
//                ObjectStat objectStat = minio.StatObjectAsync(bucketName, directory + "/" + fileName + "." + ext).GetAwaiter().GetResult();


//            }
//            catch (Exception ex)
//            {

//                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, $"Dosya Yükleme sırasında hata oluştu lütfen tekrar deneyin.Hata:3");
//            }

//        }

//        //TO DO: Geçici çözüm olarak eklendi, BucketExistsAsync çağırıldığında SSLden dolayı connection kurulamadı hatası veriyordu. Ü.A.
//        //Daha iyi bir çözüm bakılmalı.
//        public static bool returnBucketStatus(MinioClient client, string bucketName, int tryCount)
//        {
//            bool found = false;

//            try
//            {
//                found = client.BucketExistsAsync(bucketName).ConfigureAwait(false).GetAwaiter().GetResult();
//            }
//            catch (Exception ex)
//            {
//                if (tryCount == 5)
//                    throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, $"Dosya Yükleme sırasında hata oluştu lütfen tekrar deneyin.Hata:7");

//                System.Threading.Thread.Sleep(50);
//                tryCount++;
//                return returnBucketStatus(client, bucketName, tryCount);
//            }

//            return found;
//        }

//        public string GetDataUrl(string fileName, string bucketName, string ext, int sure)
//        {
//            var minioClient = new MinioClient()
//                .WithEndpoint(_minIoEndPoint)
//                .WithCredentials(_minIoAccessKey, _minIoSecurityKey)
//                .WithSSL()
//                .Build();
//            var data = GetUrl(minioClient, bucketName, fileName, sure, ext).Result;

//            return data;
//        }

//        private async Task<string> GetUrl(MinioClient minio, string bucketName, string fileName, int sure, string ext)
//        {
//            string contentType = "";

//            if (!string.IsNullOrEmpty(ext))
//            {
//                switch (ext.ToLower())
//                {
//                    case "pdf":
//                        contentType = "application/pdf";
//                        break;
//                    case "doc":
//                        contentType = "application/msword";
//                        break;
//                    case "docx":
//                        contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
//                        break;
//                    case "csv":
//                        contentType = "text/csv";
//                        break;
//                    case "jpeg":
//                        contentType = "image/jpeg";
//                        break;
//                    case "jpg":
//                        contentType = "image/jpeg";
//                        break;
//                    case "png":
//                        contentType = "image/png";
//                        break;
//                    case "ppt":
//                        contentType = "application/vnd.ms-powerpoint";
//                        break;
//                    case "pptx":
//                        contentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
//                        break;
//                    case "txt":
//                        contentType = "text/plain";
//                        break;
//                    case "xls":
//                        contentType = "application/vnd.ms-excel";
//                        break;
//                    case "xlsx":
//                        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
//                        break;
//                    case "zip":
//                        contentType = "application/zip";
//                        break;
//                    case "mp4":
//                        contentType = "video/mpeg";
//                        break;
//                    default:
//                        break;
//                        //throw new OSSException(MessageCode.ERROR_600, "Yüklemeye çalıştığınız dosya formatı desteklenmemektedir.");
//                }
//            }

//            try
//            {
//                if (!string.IsNullOrEmpty(ext))
//                {
//                    var reqParams = new Dictionary<string, string> { { "response-content-type", contentType } };
//                    var presignedUrl = minio.PresignedGetObjectAsync(bucketName, fileName, sure, reqParams).ConfigureAwait(false).GetAwaiter().GetResult();
//                    //var dd= minio.GetObjectAsync(bucketName, fileName,fileName);                   
//                    return presignedUrl;
//                }
//                else
//                {
//                    var presignedUrl = minio.PresignedGetObjectAsync(bucketName, fileName, sure, null).ConfigureAwait(false).GetAwaiter().GetResult();
//                    return presignedUrl;
//                }
//            }
//            catch (Exception ex)
//            {
//                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, "Dosya görüntülenemedi.");
//            }
//        }
//    }
//}
