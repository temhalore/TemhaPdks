namespace LorePdks.COMMON.DTO.Base
{

    public class TreeTableDTO<T>
    {
        public T data { get; set; }
        public bool expanded { get; set; }
        public List<TreeTableDTO<T>> children { get; set; }
    }
}