using System.ComponentModel.DataAnnotations;

namespace Everest.Identity.Models
{
    public class UpdatePasswordModel
    {
        [Required]
        [MinLength(6)]
        public string CurrentPassword { get; set; }

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }
    }
}
