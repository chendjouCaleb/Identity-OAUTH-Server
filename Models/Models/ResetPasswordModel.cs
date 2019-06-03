using System.ComponentModel.DataAnnotations;

namespace Everest.Identity.Models
{


    public class ResetPasswordModel
    {

        [Required]
        public string Code { get; set; }

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }

    }
}
