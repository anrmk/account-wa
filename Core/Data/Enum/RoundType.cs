using System.ComponentModel.DataAnnotations;

namespace Core.Data.Enum {
    public enum RoundType {
        [Display(Name = "No Round")]
        NoRound = 0,
        [Display(Name = "Round Up")]
        RoundUp = 1,
        [Display(Name = "Round Down")]
        RoundDown = 2
    }
}
