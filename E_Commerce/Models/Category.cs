using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Img { get; set; }

        [ValidateNever]
        public List<Product> products { get; set; }

    }
}
