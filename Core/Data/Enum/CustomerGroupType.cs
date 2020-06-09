using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Core.Data.Enum {
    public enum CustomerGroupType {
        [Display(Name = "All customers", Description = "All customers")]
        All = 0,

        [Display(Name = "Only new customers", Description = "Only new customers")]
        OnlyNew = 1,

        [Display(Name = "Exclude new customers", Description ="Exclude new customers")]
        ExcludeNew = 2
    }
}
