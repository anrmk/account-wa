using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels {
    public class CustomerTagViewModel {
        public long Id { get; set; }

        [Required]
        [Display(Name = "Tag Name")]
        [MaxLength(128)]
        public string Name { get; set; }
    }
}