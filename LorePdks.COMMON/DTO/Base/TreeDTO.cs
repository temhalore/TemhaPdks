namespace LorePdks.COMMON.DTO.Base
{

    public class TreeDTO
    {
        public string label { get; set; }
        public dynamic data { get; set; }
        public string expandedIcon { get; set; } = "pi pi-folder-open";
        public string collapsedIcon { get; set; } = "pi pi-folder-open";
        public List<TreeDTO> children { get; set; }
    }
}