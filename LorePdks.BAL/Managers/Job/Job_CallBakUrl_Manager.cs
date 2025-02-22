using AutoMapper;
using AutoMapper.Execution;
using DapperExtensions.Predicate;
using Elasticsearch.Net;
using LorePdks.BAL.Managers.Common.Kod.Interfaces;
using LorePdks.BAL.Managers.Helper.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Nest;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using LorePdks.COMMON.Enums;
using LorePdks.DAL.Model;
using LorePdks.DAL.Repository;
using Quartz;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Transactions;
using System.Xml.Serialization;
using static Nest.MachineLearningUsage;
using static OfficeOpenXml.ExcelErrorValue;
using static Slapper.AutoMapper;

namespace LorePdks.BAL.Managers.Job
{
    [DisallowConcurrentExecution] // bu attribute olmazsa asenkron çalışır class üstünde bu olursa o job çağırsı önceki çağrı bitene kadar bekler
    public class Job_CallBakUrl_Manager : IJob
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IHelperManager _helperManager;
        private IMapper _mapper;
        private IKodManager _kodManager;
        private readonly ILogger<Job_CallBakUrl_Manager> _logger;



        public Job_CallBakUrl_Manager(IKodManager kodManager, IMapper mapper, IHttpContextAccessor httpContextAccessor, IHelperManager helperManager, ILogger<Job_CallBakUrl_Manager> logger)
        {
            //_modelDtoConverterHelper = modelDtoConverterHelper;
            _httpContextAccessor = httpContextAccessor;
            _helperManager = helperManager;
            _kodManager = kodManager;
            _logger = logger;

        }

        public Task Execute(IJobExecutionContext context)
        {
            string jobId = Guid.NewGuid().ToString();
            string metodName = "Job_CallBakUrl_Manager";
            Debug.WriteLine($"------------------job:{metodName}------- başladı ---------{jobId} :: {DateTime.Now.ToString()} ");
            ////Task.Delay(TimeSpan.FromSeconds(20));
            //Thread.Sleep(20000);

            int donguCount = 0;
            int hataCount = 0;
            try
            {
                var odemeRepo = new GenericRepository<T_Pos_Odeme>();
                int maxCallBackUrlCagriCount = 10;
                var cagriYapilacakList = odemeRepo.GetList("IS_UYGULAMA_CALL_BACK_URL_CAGRI_OK !=1 and (UYGULAMA_CALL_BACK_METOD_URL is not null and UYGULAMA_CALL_BACK_METOD_URL !='') and CALL_BACK_URL_CAGRI_COUNT<=@maxCallBackUrlCagriCount", new { maxCallBackUrlCagriCount });

                _logger.LogInformation($"{metodName} çalıştı.jobId:{jobId} cagriYapilacakList.count{cagriYapilacakList.Count} deneme yapılacak ");

                foreach (var odeme in cagriYapilacakList)
                {
                    donguCount++;
                    //zaman alacaktır ama bi sorgu daha at anlık bak
                    var odemeDb = odemeRepo.Get(odeme.ID);
                    if (!string.IsNullOrEmpty(odemeDb.UYGULAMA_CALL_BACK_METOD_URL) && !odemeDb.IS_UYGULAMA_CALL_BACK_URL_CAGRI_OK)
                    {
                        //_odemeManager.CallBackMetodCagirisYap<Job_CallBakUrl_Manager>(odemeDb.ODEME_KEY, odemeDb.UYGULAMA_CALL_BACK_METOD_URL, _logger);
                    }
                }

            }
            // hata yı loglar ama dönme
            catch (Exception ex)
            {
                hataCount++;
                _logger.LogError($"{metodName} jobId:{jobId} hata:{ex.Message} innerMessage:{ex.InnerException} ");

                //throw;
            }

            _logger.LogInformation($"{metodName} bitti.jobId:{jobId} donguCount:{donguCount} hataCount:{hataCount} ");

            Debug.WriteLine($"------------------job:{metodName}--------------------{jobId} :: {DateTime.Now.ToString()}");
            return Task.CompletedTask;

        }
    }
}