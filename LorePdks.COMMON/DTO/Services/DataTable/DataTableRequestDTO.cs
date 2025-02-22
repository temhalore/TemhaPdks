using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LorePdks.COMMON.DTO.Services.DataTable
{
    public class DataTableRequestDTO<T>
    {
        public List<DataTableColumns> columns { get; set; }
        public long start { get; set; }
        public long length { get; set; }
        public long draw { get; set; }
        public List<DataTableOrder> order { get; set; }
        public DataTableSearch search { get; set; }
        public T data { get; set; }
    }
    public class DataTableColumns
    {
        public string data { get; set; }
        public string name { get; set; }
        public bool orderable { get; set; }
        public bool searchable { get; set; }
    }
    public class DataTableOrder
    {
        public int column { get; set; }
        public string dir { get; set; }
    }
    public class DataTableSearch
    {
        public bool regex { get; set; }
        public string value { get; set; }
    }
}
