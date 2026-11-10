using System.ComponentModel.DataAnnotations;

namespace University_Cafeteria_Task8.Models
{
    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";
    }
}