using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models.View_Model
{
    public class LoginVm
    {
        
        public int Id { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
       
        public bool RememberMe { get; set; }
    }
}
