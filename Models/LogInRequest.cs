using System.ComponentModel.DataAnnotations;

namespace OktaScimDemo.Models
{
    public class LogInRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
