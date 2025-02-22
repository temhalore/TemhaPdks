//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using LorePdks.DAL.Model;
//using LorePdks.DAL.Repository;

//namespace LorePdks.BAL.Managers.Security
//{
//    public class LogManager
//    {
//        public async Task RequestLog(string correlationId, string requestInfo, byte[] message, string ip, string token, HttpRequestMessage request)
//        {
//            var repoRestlog = new GenericRepository<t_core_restlog>();
//            //request.Properties["MS_HttpContext"] = null;
//            //request.Properties["MS_RequestContext"] = null;
//            //request.Properties["MS_HttpConfiguration"] = null;
//            //request.Properties["MS_CorsRequestContextKey"] = null;

//            t_core_restlog restlog = new t_core_restlog
//            {
//                CorrelationId = correlationId,
//                Info = requestInfo,
//                Ip = ip,
//                Message = Encoding.UTF8.GetString(message),
//                //Rest= JsonConvert.SerializeObject(request),
//                Token = token,
//                RestType = 1
//            };

//            await Task.Run(() => repoRestlog.Insert(restlog));
//        }
//        public async Task ResponseLog(string correlationId, string requestInfo, byte[] message, string ip, string token, HttpResponseMessage response)
//        {
//            var repoRestlog = new GenericRepository<t_core_restlog>();


//            //response.RequestMessage.Properties["MS_RequestContext"] = null;
//            //response.RequestMessage.Properties["MS_HttpConfiguration"] = null;
//            //response.RequestMessage.Properties["MS_CorsRequestContextKey"] = null;
//            //response.RequestMessage.Properties["MS_DisposableRequestResources"] = null;
//            //response.RequestMessage.Properties["MS_HttpActionDescriptor"] = null;

//            t_core_restlog restlog = new t_core_restlog
//            {
//                CorrelationId = correlationId,
//                Info = requestInfo,
//                Ip = ip,
//                Message = Encoding.UTF8.GetString(message),
//                //Rest= JsonConvert.SerializeObject(request),
//                Token = token,
//                RestType = 2
//            };

//            await Task.Run(() => repoRestlog.Insert(restlog));
//        }
//        public void ReqLog(string correlationId, string requestInfo, byte[] message, string ip, string token, HttpRequestMessage request)
//        {
//            string fName = "req_" + correlationId + ".txt";
//            string fileName = @"C:\restLog\" + fName;
//            try
//            {
//                if (File.Exists(fileName))
//                {
//                    File.Delete(fileName);
//                }

//                t_core_restlog restlog = new t_core_restlog
//                {
//                    CorrelationId = correlationId,
//                    Info = requestInfo,
//                    Ip = ip,
//                    Message = Encoding.UTF8.GetString(message),
//                    //Rest= JsonConvert.SerializeObject(request),
//                    Token = token,
//                    RestType = 1
//                };

//                using (StreamWriter sw = File.CreateText(fileName))
//                {
//                    sw.WriteLine("New file created: {0}", DateTime.Now.ToString());
//                    sw.WriteLine("corrId:{0}", correlationId);
//                    sw.WriteLine("Info:{0}", requestInfo);
//                    sw.WriteLine("IP:{0} ", ip);
//                    sw.WriteLine("MESSAGE:{0} ", Encoding.UTF8.GetString(message));
//                    sw.WriteLine("TOKEN:{0} ", token);
//                    sw.WriteLine("REST_TYPE:{0} ", 1);
//                    sw.WriteLine("json:{0} ", JsonConvert.SerializeObject(restlog));
//                    sw.WriteLine("header:{0} ", JsonConvert.SerializeObject(request.Headers));
//                    sw.WriteLine("content:{0} ", JsonConvert.SerializeObject(request.Content));
//                    sw.WriteLine("method:{0} ", JsonConvert.SerializeObject(request.Method));
//                    sw.WriteLine("requestUri:{0} ", JsonConvert.SerializeObject(request.RequestUri));
//                    //sw.WriteLine("prop:{0} ", JsonConvert.SerializeObject(request.Properties));
//                    sw.WriteLine("Done! ");
//                }
//            }
//            catch (Exception)
//            {
//            }
//        }
//        public void RespLog(string correlationId, string requestInfo, string ip, string token)//, byte[] message,HttpResponseMessage response
//        {
//            string fName = "resp_" + correlationId + ".txt";
//            string fileName = @"C:\restLog\" + fName;
//            try
//            {
//                if (File.Exists(fileName))
//                {
//                    File.Delete(fileName);
//                }

//                t_core_restlog restlog = new t_core_restlog
//                {
//                    CorrelationId = correlationId,
//                    Info = requestInfo,
//                    Ip = ip,
//                    //Message = Encoding.UTF8.GetString(message), buraya requestdrolar gelir
//                    //Rest= JsonConvert.SerializeObject(request),
//                    Token = token,
//                    RestType = 2
//                };

//                using (StreamWriter sw = File.CreateText(fileName))
//                {
//                    sw.WriteLine("New file created: {0}", DateTime.Now.ToString());
//                    sw.WriteLine("corrId:{0}", correlationId);
//                    sw.WriteLine("Info:{0}", requestInfo);
//                    sw.WriteLine("IP:{0} ", ip);
//                    //sw.WriteLine("MESSAGE:{0} ", Encoding.UTF8.GetString(message));
//                    sw.WriteLine("TOKEN:{0} ", token);
//                    sw.WriteLine("REST_TYPE:{0} ", 2);
//                    sw.WriteLine("json:{0} ", JsonConvert.SerializeObject(restlog));
//                    sw.WriteLine("Done! ");
//                }
//            }
//            catch (Exception)
//            {
//            }
//        }
//        //public void ExeceptionLog(HttpActionExecutedContext actionExecutedContext)
//        //{
//        //    var name= Guid.NewGuid().ToString("N").ToUpper();
//        //    string fName = "exception_" + name+ ".txt";
//        //    string fileName = @"C:\restLog\" + fName;
//        //    try
//        //    {
//        //        if (File.Exists(fileName))
//        //        {
//        //            File.Delete(fileName);
//        //        }

//        //        using (StreamWriter sw = File.CreateText(fileName))
//        //        {
//        //            try
//        //            {
//        //                sw.WriteLine("New file created: {0}", DateTime.Now.ToString());                        
//        //                if (actionExecutedContext != null)
//        //                {
//        //                    if (actionExecutedContext.Request != null)
//        //                    {
//        //                        sw.WriteLine("header:{0} ", JsonConvert.SerializeObject(actionExecutedContext.Request.Headers));
//        //                        sw.WriteLine("content:{0} ", JsonConvert.SerializeObject(actionExecutedContext.Request.Content));
//        //                        sw.WriteLine("method:{0} ", JsonConvert.SerializeObject(actionExecutedContext.Request.Method));
//        //                        sw.WriteLine("requestUri:{0} ", JsonConvert.SerializeObject(actionExecutedContext.Request.RequestUri));
//        //                    }
//        //                }
//        //                if (actionExecutedContext != null)
//        //                {
//        //                    if (actionExecutedContext.Response != null)
//        //                    {
//        //                        sw.WriteLine("header:{0} ", JsonConvert.SerializeObject(actionExecutedContext.Response.Headers));
//        //                        sw.WriteLine("content:{0} ", JsonConvert.SerializeObject(actionExecutedContext.Response.Content));
//        //                        sw.WriteLine("method:{0} ", JsonConvert.SerializeObject(actionExecutedContext.Response.StatusCode));
//        //                        sw.WriteLine("requestUri:{0} ", JsonConvert.SerializeObject(actionExecutedContext.Response.IsSuccessStatusCode));
//        //                    }
//        //                }
//        //                if (actionExecutedContext != null)
//        //                {
//        //                    sw.WriteLine("Exception:" + JsonConvert.SerializeObject(actionExecutedContext.Exception));
//        //                    if (actionExecutedContext.ActionContext != null)
//        //                    {
//        //                        sw.WriteLine("ModelState:" + JsonConvert.SerializeObject(actionExecutedContext.ActionContext.ModelState));
//        //                        sw.WriteLine("ActionArguments:" + JsonConvert.SerializeObject(actionExecutedContext.ActionContext.ActionArguments));
//        //                        sw.WriteLine("ActionDescriptor:" + JsonConvert.SerializeObject(actionExecutedContext.ActionContext.ActionDescriptor));
//        //                        sw.WriteLine("ControllerContext:" + JsonConvert.SerializeObject(actionExecutedContext.ActionContext.ControllerContext));                                
//        //                    }
//        //                }

//        //                //sw.WriteLine("prop:{0} ", JsonConvert.SerializeObject(request.Properties));
//        //                sw.WriteLine("Done! ");
//        //            }
//        //            catch (Exception ex)
//        //            {
//        //                sw.WriteLine("hata:"+ JsonConvert.SerializeObject(ex));
//        //            }                    
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {

//        //    }
//        //}
//        public void SaveLog(List<t_core_restlog> logList)
//        {
//            var repo = new GenericRepository<t_core_restlog>();

//            repo.InsertAll(logList);
//        }
//    }
//}
