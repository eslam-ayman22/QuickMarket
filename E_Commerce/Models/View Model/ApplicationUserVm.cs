using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models.View_Model
{
    public class ApplicationUserVm
    {
        [Required]
        public string UserName { get; set; }


        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Adress { get; set; }
    }
}
