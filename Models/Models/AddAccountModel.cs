using System;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Everest.Identity.Models
{
    public class AddAccountModel
    {
        [Required]
        [MinLength(length: 3)]
        public String Name { get; set; }

        [Required]
        [MinLength(length: 3)]
        public String Surname { get; set; }

        [EmailAddress]
        [Required]
        public String Email { get; set; }

        [Required]
        [MinLength(6)]
        public String Password { get; set; }
    }
}
