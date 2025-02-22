using LorePdks.COMMON.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LorePdks.BAL.Services.Elasticsearch
{
    public interface IElasticsearchService
    {
        void DeleteLogs(DateTime beginDatetime, string levels, string patern, string message, int page = 0);
        object GetLogsByOysToken(LogRequestDTO logRequestDTO);
        object GetLogsByAppTokenList(List<string> appTokenlist, LogRequestDTO logRequestDTO);
    }
}
