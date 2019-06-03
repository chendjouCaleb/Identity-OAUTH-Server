using System.ComponentModel.DataAnnotations;

namespace Everest.Identity.Models
{
    public class LoginModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Navigator { get; set; }

        [Required]
        public string OS { get; set; }

        [Required]
        public string RemoteAddress { get; set; }

        public bool Persisted { get; set; }
    }
}
