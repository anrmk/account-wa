using System.ComponentModel.DataAnnotations;

namespace Core.Data.Enum {
    public enum RoundType {
        [Display(Name = "No Round", Description = "No Round")]
        NoRound = 0,
        [Display(Name = "Round Up", Description = "Round Up")]
        RoundUp = 1,
        [Display(Name = "Round Down", Description = "Round Down")]
        RoundDown = 2
    }
}
