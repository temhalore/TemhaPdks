
using AutoMapper;
using LorePdks.BAL.Services.DataTable.Interfaces;
using LorePdks.COMMON.DTO.Services.DataTable;
using LorePdks.DAL.Model;
using LorePdks.DAL.Repository;
using System.Diagnostics.PerformanceData;

namespace LorePdks.BAL.Services.DataTable
{
    public class DataTableService : IDataTableService
    {
        private readonly IMapper _mapper;
        public DataTableService(IMapper mapper)
        {
            _mapper = mapper;
        }
        public DataTableResponseDTO<T> GetDataTableResponseByDtoQuery<T>(string baseQuery, DataTableRequestDTO<T> request, object queryParam = null)
        {
            GenericRepository<T_Pos_Kod> _repoRepo = new GenericRepository<T_Pos_Kod>();

            var response = new DataTableResponseDTO<T>();

            //var repo = new GenericRepository<t_core_repo>();

            string query = baseQuery;
            var columnList = new List<string>();
            if (request.search != null && !string.IsNullOrEmpty(request.search.value))
            {

                foreach (var item in request.columns)
                {
                    if (item.searchable)
                    {
                        //   columnList.Add(item.name + " LIKE '%" + request.search.value.TrimEnd() + "%'");
                        columnList.Add(item.name + " LIKE @param");
                    }
                }


                if (columnList.Count > 0)
                {
                    query += " AND (" + string.Join(" OR ", columnList.ToArray()) + ") ";
                }

            }
            var param = "";
            if (request.search.value != null)
            {
                param = "%" + request.search.value.TrimEnd() + "%";
            }

            string queryCount = $"SELECT COUNT(*) as COUNT " + query.Substring(query.IndexOf("FROM"), query.Length - query.IndexOf("FROM"));

            long recordsTotal = Convert.ToInt64(_repoRepo.QueryDyn(queryCount, queryParam ?? new { param }).ToList()[0].COUNT);

            //int recordsTotal = recordsTotal1[0].COUNT;

            columnList = new List<string>();
            if (request.order.Count > 0)
            {

                foreach (var item in request.order)
                {
                    columnList.Add(request.columns[item.column].name + " " + item.dir);
                }
                query += " ORDER BY " + string.Join(',', columnList.ToArray());
            }
            if (request.length > 0)
                query += " OFFSET " + request.start + " ROWS FETCH NEXT " + request.length + " ROWS ONLY";

            List<T> tableData = _repoRepo.Query<T>(query, queryParam ?? new { param }).ToList();

            //foreach (var item in tableData)
            //{
            //    if (item.id != null)
            //    {
            //        item.eid = CryptoHelper.EncryptString(item.id.ToString());
            //    }
            //}

            response.recordsFiltered = recordsTotal;
            response.data = tableData;
            response.recordsTotal = recordsTotal;
            response.draw = request.draw;
            return response;
        }
        public DataTableResponseDTO<T2> GetDataTableResponseByModelQuery<T1, T2>(string baseQuery, DataTableRequestDTO<T2> request, object queryParam = null, string countType = null)
        {
            GenericRepository<T_Pos_Kod> _repoRepo = new GenericRepository<T_Pos_Kod>();
            var response = new DataTableResponseDTO<T2>();
            if (countType == null)
            {
                countType = "*";
            }
            //var repo = new GenericRepository<t_core_repo>();

            string query = baseQuery;
            var columnList = new List<string>();
            if (request.search != null && !string.IsNullOrEmpty(request.search.value))
            {

                foreach (var item in request.columns)
                {
                    if (item.searchable)
                    {
                        //   columnList.Add(item.name + " LIKE '%" + request.search.value.TrimEnd() + "%'");
                        columnList.Add("CONVERT(NVARCHAR," + item.name + ") LIKE @param");
                    }
                }


                if (columnList.Count > 0)
                {
                    query += " AND (" + string.Join(" OR ", columnList.ToArray()) + ") ";
                }

            }
            var param = "";
            if (request.search.value != null)
            {
                param = "%" + request.search.value.TrimEnd() + "%";
            }
            query = query.Replace("from", "FROM");
            string queryCount = $"SELECT COUNT({countType}) AS COUNT " + query.Substring(query.IndexOf("FROM"), query.Length - query.IndexOf("FROM"));

            long recordsTotal = Convert.ToInt64(_repoRepo.QueryDyn(queryCount, queryParam ?? new { param }).ToList()[0].COUNT);
            //int recordsTotal = recordsTotal1[0].COUNT;

            columnList = new List<string>();
            if (request.order.Count > 0)
            {

                foreach (var item in request.order)
                {
                    columnList.Add(request.columns[item.column].name + " " + item.dir);
                }
                query += " ORDER BY " + string.Join(',', columnList.ToArray());
            }
            else
            {
                query += " ORDER BY 1";
            }
            if (request.length > 0)
                query += " OFFSET " + request.start + " ROWS FETCH NEXT " + request.length + " ROWS ONLY";
            var tableData = _repoRepo.Query<T1>(query, queryParam ?? new { param });
            List<T2> dataTableDto = _mapper.Map<List<T2>>(tableData);
            response.recordsFiltered = recordsTotal;
            response.data = dataTableDto;
            response.recordsTotal = recordsTotal;
            response.draw = request.draw;
            return response;
        }
        public DataTableResponseDTO<T2> GetDataTableResponseByModel<T1, T2>(DataTableRequestDTO<T2> request, string whereQuery = null, object queryParam = null)
        {
            GenericRepository<T_Pos_Kod> _repoRepo = new GenericRepository<T_Pos_Kod>();

            var response = new DataTableResponseDTO<T2>();

            var type = typeof(T1);
            string _tableName = type.Name;//.CustomAttributes.Where(x => x.AttributeType.Name == "TableAttribute").FirstOrDefault().ConstructorArguments.FirstOrDefault().Value.ToString();

            string query = @$"SELECT * FROM {_tableName} ";
            if (!string.IsNullOrEmpty(whereQuery))
            {
                whereQuery = $@"WHERE 1=1 AND IsDeleted = 0 AND {whereQuery}";
            }
            else
            {
                whereQuery = $@"WHERE 1=1 AND IsDeleted = 0";
            }
            query = query + whereQuery;

            var columnList = new List<string>();
            if (request.search != null && !string.IsNullOrEmpty(request.search.value))
            {

                foreach (var item in request.columns)
                {
                    if (item.searchable)
                    {
                        //   columnList.Add(item.name + " LIKE '%" + request.search.value.TrimEnd() + "%'");
                        columnList.Add(item.name + " LIKE @param");
                    }
                }


                if (columnList.Count > 0)
                {
                    query += " AND (" + string.Join(" OR ", columnList.ToArray()) + ") ";
                }

            }
            var param = "";
            //!= null && request.search.value != ""
            if (request.search.value.Length > 0)
            {
                param = "%" + request.search.value.TrimEnd() + "%";
            }

            string queryCount = "SELECT COUNT(*) AS COUNT " + query.Substring(query.IndexOf("FROM"), query.Length - query.IndexOf("FROM"));

            long recordsTotal = Convert.ToInt64(_repoRepo.QueryDyn(queryCount, queryParam ?? new { param }).ToList()[0].COUNT);
            //int recordsTotal = recordsTotal1[0].COUNT;

            columnList = new List<string>();
            if (request.order.Count > 0)
            {

                foreach (var item in request.order)
                {
                    columnList.Add(request.columns[item.column].name + " " + item.dir);
                }
                query += " ORDER BY " + string.Join(',', columnList.ToArray());
            }
            if (request.length > 0)
                query += " OFFSET " + request.start + " ROWS FETCH NEXT " + request.length + " ROWS ONLY";

            var tableData = _repoRepo.Query<T1>(query, queryParam ?? new { param });
            List<T2> dataTableDto = _mapper.Map<List<T2>>(tableData);
            response.recordsFiltered = recordsTotal;
            response.data = dataTableDto;
            response.recordsTotal = recordsTotal;
            response.draw = request.draw;
            return response;
        }
    }
}
