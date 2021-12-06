using System.ComponentModel.DataAnnotations;

namespace OktaScimDemo.Models
{
    public class UpdateUserRequest
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
    }
}
