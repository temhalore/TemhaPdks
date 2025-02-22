namespace LorePdks.COMMON.Models.ServiceResponse.Interfaces
{
    public interface IServiceResponse<T>
    {
        T data { get; set; }
        //int pageNumber { get; set; }
        //int itemsPerPage { get; set; }
        //int totalItems { get; set; }
    }
}