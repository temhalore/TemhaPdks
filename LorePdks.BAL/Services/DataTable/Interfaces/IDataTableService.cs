using LorePdks.COMMON.DTO.Services.DataTable;

namespace LorePdks.BAL.Services.DataTable.Interfaces
{
    public interface IDataTableService
    {
        DataTableResponseDTO<T> GetDataTableResponseByDtoQuery<T>(string baseQuery, DataTableRequestDTO<T> request, object queryParam = null);
        DataTableResponseDTO<T2> GetDataTableResponseByModelQuery<T1, T2>(string baseQuery, DataTableRequestDTO<T2> request, object queryParam = null, string countType = null);
        DataTableResponseDTO<T2> GetDataTableResponseByModel<T1, T2>(DataTableRequestDTO<T2> request, string whereQuery, object queryParam = null);
    }
}
