using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LorePdks.COMMON.DTO.Services.DataTable
{
    public class DataTableResponseDTO<T>
    {
        public List<T> data { get; set; }
        public long draw { get; set; }
        public long recordsFiltered { get; set; }
        public long recordsTotal { get; set; }
    }
    public class DataTableResponse
    {
        public List<dynamic> data { get; set; }
        public long recordsTotal { get; set; }
    }
}
