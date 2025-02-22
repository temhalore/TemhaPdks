//using Microsoft.Extensions.Configuration;
//using Nest;
//using Newtonsoft.Json;
//using LorePdks.BAL.BaseManager.Elasticsearch.Interfaces;
//using LorePdks.BAL.BaseManager.Elasticsearch.Model;
//using LorePdks.COMMON.Aspects.Logging.Serilog;
//using LorePdks.COMMON.DTO.Base;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace LorePdks.BAL.BaseManager.Elasticsearch
//{
//    public class ElasticsearchManager : IElasticsearchManager
//    {

//        private readonly IConfiguration _configuration;
//        private readonly IElasticClient _client;

//        public ElasticsearchManager(IConfiguration configuration)
//        {
//            _configuration = configuration;
//            _client = CreateInstance();
//        }

//        private ElasticClient CreateInstance()
//        {
//            var logConfig = _configuration.GetSection("LogConfig").Get<LogingConfigurationModel>() ??
//                        throw new Exception("Null Options Message");

//            var settings = new ConnectionSettings(new Uri(logConfig.Serilog.Elasticsearch.Host));

//            settings.EnableApiVersioningHeader(); // enable ES 7.x compatibility on ES 8.x servers

//            if (!string.IsNullOrEmpty(logConfig.Serilog.Elasticsearch.Username) && !string.IsNullOrEmpty(logConfig.Serilog.Elasticsearch.Password))
//                settings.BasicAuthentication(logConfig.Serilog.Elasticsearch.Username, logConfig.Serilog.Elasticsearch.Password);

//            //settings.DisableDirectStreaming(true);

//            return new ElasticClient(settings);
//        }
//        public object GetLogsByAppTokenList(List<string> appTokenlist, LogRequestDTO logRequestDTO)
//        {
//            string patern = logRequestDTO.patern == "exception" ? "exception" : "logs";

//            var logConfig = _configuration.GetSection("LogConfig").Get<LogingConfigurationModel>() ??
//                       throw new Exception("Null Options Message");

//            string tokens = string.Join(',', appTokenlist);
//            BoolQueryDescriptor<LogsViewModel> boolQueryDescriptor = new BoolQueryDescriptor<LogsViewModel>();
//            boolQueryDescriptor = boolQueryDescriptor
//                                                .Must(qt => qt.Match(m => m
//                                                                     .Field("fields.spToken")
//                                                                     .Query(tokens)
//                                                                     ),
//                                                      ql => ql
//                                                        .Match(m => m
//                                                            .Field("level")
//                                                            .Query(logRequestDTO.levels)
//                                                            )
//                                                    )
//                                                .Filter(fi => fi
//                                                    .DateRange(r => r
//                                                        .Field(f => f.Timestamp)
//                                                        .GreaterThanOrEquals(logRequestDTO.beginDatetime)
//                                                    ));
//            if (!string.IsNullOrWhiteSpace(logRequestDTO.mesaj))
//            {
//                boolQueryDescriptor = boolQueryDescriptor.Should(
//                                                            x => x.MatchPhrase(y => y.Field("message").Query(logRequestDTO.mesaj))
//                                                           )
//                                                      .MinimumShouldMatch("1");
//            }

//            var response = _client.Search<LogsViewModel>(p => p.Index($"{logConfig.ProjectName}-{patern}-*")
//                                       .From(logRequestDTO.page * 25)
//                                       .Size(25)
//                                       .Query(q => q

//                                            .Bool(b => boolQueryDescriptor
//                                           )
//                                       )
//                                       .Sort(s => s.Descending(f => f.Timestamp)));




//            //if (response.ApiCall.ResponseBodyInBytes != null)
//            //{
//            //    var responseJson = System.Text.Encoding.UTF8.GetString(response.ApiCall.ResponseBodyInBytes);
//            //    responseJson += "";
//            //}
//            return response.Documents;

//        }

//        public object GetLogsByOysToken(LogRequestDTO logRequestDTO)
//        {
//            string patern = logRequestDTO.patern == "exception" ? "exception" : "logs";

//            var logConfig = _configuration.GetSection("LogConfig").Get<LogingConfigurationModel>() ??
//                      throw new Exception("Null Options Message");

//            var response = _client.Search<LogsViewModel>(p => p
//                                       .Index($"{logConfig.ProjectName}-{patern}-*")
//                                       .From(logRequestDTO.page * 25)
//                                       .Size(25)
//                                       //.Query(q => q
//                                       //        .DateRange(x => x.LessThanOrEquals(beginDatetime))
//                                       //       )
//                                       //.Query(q => q
//                                       //     .Bool(b => b
//                                       //         .Must(mu => mu
//                                       //             .MatchPhrase(m => m
//                                       //                 .Field(f => f.Message).Query("FormAppId"))
//                                       //             )
//                                       //         .Filter(fi => fi
//                                       //             .DateRange(r => r
//                                       //                 .Field(f => f.Timestamp)
//                                       //                 .LessThanOrEquals(new DateTime(2017, 1, 1)))
//                                       //             )
//                                       //         )
//                                       //     )
//                                       .Query(q => q

//                                            .Bool(b => b
//                                                .Should(
//                                                        x => x.MatchPhrase(y => y.Field("message").Query(logRequestDTO.mesaj))
//                                                       )
//                                                .MinimumShouldMatch("1")
//                                                .Must(qt => qt
//                                                       .Match(m => m
//                                                                    .Field("fields.spToken")
//                                                                    .Query(logRequestDTO.appToken)
//                                                                    ),
//                                                        qt => qt
//                                                        .Match(m => m
//                                                            .Field("level")
//                                                            .Query(logRequestDTO.levels)
//                                                            )
//                                                    )
//                                                .Filter(fi => fi
//                                                    .DateRange(r => r
//                                                        .Field(f => f.Timestamp)
//                                                        .LessThanOrEquals(logRequestDTO.beginDatetime)
//                                                    )
//                                       //.Filter(f=>f
//                                       //           .DateRange(x => x
//                                       //            .Field("@timestamp")
//                                       //            .LessThanOrEquals(beginDatetime)
//                                       //            )
//                                       //       )
//                                       // .Filter(f =>
//                                       //    f.DateRange(dt => dt
//                                       //        .Field(field => field.Timestamp)
//                                       //        .LessThanOrEquals(beginDatetime.AddDays(-1))
//                                       //        .TimeZone("+3:00")))
//                                       ))
//                                       )

//                                       .Sort(s => s.Descending(f => f.Timestamp)));

//            #region MyRegion
//            //.Query(q => q
//            //        .Match(m => m
//            //                .Field(f => f.Fields.oysToken)
//            //                .Query(oysToken)
//            //                )

//            //    )
//            //var response1 = await _client.Cat.IndicesAsync(c => c.AllIndices());
//            //var logIndexes = response1.Records.Select(a => a.Index).ToArray();

//            //var searchRequest = new SearchRequest<dynamic>(Nest.Indices.All)
//            //{
//            //    From = 0,
//            //    Size = 100,
//            //    Query = new MatchQuery
//            //    {
//            //        Field = Infer.Field<dynamic>(f => f),
//            //        Query = "Martijn"
//            //    }
//            //};



//            //.Query(q => q
//            //    .Exists(e => e
//            //        .Field("Elapsed")
//            //    )
//            //)

//            //.Query(q => q
//            //    .MultiMatch(m => m
//            //        .Fields(f => f
//            //            .Field("oysToken")
//            //        )
//            //        .Query("VjFuWTQ1MFlmTElTZEJBL1UzTWdnZmxFeS90bmR6TVc2Z1BwWHFRVmhYVT06")
//            //    )
//            //)

//            //.Query(q => q
//            //    .Term("oysToken", "VjFuWTQ1MFlmTElTZEJBL1UzTWdnZmxFeS90bmR6TVc2Z1BwWHFRVmhYVT06")

//            //    )
//            //);
//            // .Query(q => q
//            //    .Indices(i => i
//            //        .Indices(new[] { "INDEX_A", "INDEX_B" })
//            //        .Query(iq => iq.Term("FIELD", "VALUE"))
//            //        .NoMatchQuery(iq => iq.Term("FIELD", "VALUE"))
//            //    )
//            //)
//            //.Query(q => q.Bool(b => b.Must(bs => bs.Term(p => p.Field("fields.oysToken").Value("VjFuWTQ1MFlmTElTZEJBL1UzTWdnZmxFeS90bmR6TVc2Z1BwWHFRVmhYVT06"))))))

//            //.Query(q => q
//            //     .Match(m => m
//            //        .Field(f => f.oysToken)
//            //        .Query("")
//            //     )
//            //)

//            //.Source(f=>f.Includes(p2=>p2.Field(f2=>f2.message)))  
//            // .Query(q => q.MatchAll())

//            //  .PostFilter(f => f.DateRange(r => r.Field(f2 => f2.Timestamp).GreaterThanOrEquals(DateTime.Now.AddDays(-1))))
//            //   )

//            #endregion

//            return response.Documents;

//        }

//        public async Task ChekIndex(string indexName)
//        {
//            var anyy = await _client.Indices.ExistsAsync(indexName);
//            if (anyy.Exists)
//                return;

//            var response = await _client.Indices.CreateAsync(indexName,
//                ci => ci
//                    .Index(indexName)
//                    //.ProductMapping()
//                    .Settings(s => s.NumberOfShards(3).NumberOfReplicas(1))
//                    );

//            return;

//        }

//        public void DeleteLogs(DateTime beginDatetime, string levels, string patern, string message, int pagesize)
//        {
//            var logConfig = _configuration.GetSection("LogConfig").Get<LogingConfigurationModel>() ??
//                      throw new Exception("Null Options Message");

//            var response = _client.DeleteByQuery<dynamic>(p => p
//                                       .Index($"{logConfig.ProjectName}-*")
//                                       .Size(pagesize)
//                                       .Query(q => q

//                                            .Bool(b => b
//                                            //.Should(
//                                            //        x => x.MatchPhrase(y => y.Field("message").Query(message))
//                                            //       )
//                                            //.MinimumShouldMatch("1")
//                                            .Must(
//                                                    //qt => qt.Match(m => m
//                                                    //                    .Field("fields.oysToken")
//                                                    //                    .Query(tokens)
//                                                    //                    ),
//                                                    ql => ql
//                                                    .Match(m => m
//                                                        .Field("fields.SourceContext")
//                                                        .Query("Microsoft.AspNetCore.Hosting.Diagnostics,Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor,Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker,Microsoft.AspNetCore.Routing.EndpointMiddleware,Microsoft.AspNetCore.Cors.Infrastructure.CorsService,Microsoft.Hosting.Lifetime,Microsoft.AspNetCore.StaticFiles.StaticFileMiddleware,Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker,Microsoft.AspNetCore.Mvc.ModelBinding.ParameterBinder,Microsoft.AspNetCore.Mvc.ModelBinding.Binders.BodyModelBinder,Microsoft.AspNetCore.Mvc.Formatters.SystemTextJsonInputFormatter,Microsoft.AspNetCore.HostFiltering.HostFilteringMiddleware,Microsoft.AspNetCore.HttpsPolicy.HstsMiddleware,Microsoft.AspNetCore.Routing.Matching.DfaMatcher,Microsoft.AspNetCore.Routing.EndpointRoutingMiddleware,Microsoft.AspNetCore.Mvc.Infrastructure.DefaultOutputFormatterSelector,Microsoft.AspNetCore.Mvc.ModelBinding.ModelBinderFactory,Microsoft.AspNetCore.Server.IIS.Core.IISHttpServer,Microsoft.Extensions.Hosting.Internal.Host,Microsoft.AspNetCore.Mvc.ModelBinding.ModelBinderFactory,Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware")
//                                                        )
//                                                )
//                                           //.Filter(
//                                           //    fi => fi.DateRange(r => r
//                                           //                        .Field(f => f.Timestamp)
//                                           //                        .LessThanOrEquals(beginDatetime)
//                                           //                      )
//                                           //    )

//                                           )
//                                       )
//                                       );

//            // bool isvalid = response.IsValid;

//        }


//        //public async Task InsertDocument(string indexName, Product product)
//        //{

//        //    var response = await _client.CreateAsync(product, q => q.Index(indexName));
//        //    if (response.ApiCall?.HttpStatusCode == 409)
//        //    {
//        //        await _client.UpdateAsync<Product>(response.Id, a => a.Index(indexName).Doc(product));
//        //    }

//        //}

//        //public async Task InsertDocuments(string indexName, List<Product> products)
//        //{
//        //    await _client.IndexManyAsync(products, index: indexName);
//        //}


//        //public async Task<Product> GetDocument(string indexName, int id)
//        //{
//        //    var response = await _client.GetAsync<Product>(id, q => q.Index(indexName));

//        //    return response.Source;

//        //}

//        //public async Task<List<Product>> GetDocuments(string indexName)
//        //{
//        //    var response = await _client.SearchAsync<Product>(q => q.Index(indexName).Scroll("5m"));
//        //    return response.Documents.ToList();
//        //}
//    }





//}
