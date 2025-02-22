using System.Collections.Generic;
using System.Text.Json.Serialization;
using LorePdks.COMMON.DTO.Security.Page;
using LorePdks.COMMON.DTO.Base;

namespace LorePdks.COMMON.DTO.Security.Menu
{
    public class MenuDTO : BaseDTO
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public string title { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public PageDTO pageDto { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public string target { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public string icon { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public string href { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public int hasSubMenu { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public long orderNo { get; set; }
        //public long parentId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public MenuDTO parentMenuDto { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public string tree { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public List<MenuDTO> subMenuListDto { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public bool isExpended { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public bool isRoleMenu { get; set; }
    }
}
