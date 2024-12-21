using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace E_Commerce.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set;}
        public string Description { get; set; }
        public string Img { get; set; }
        public double Price { get; set; }

        public decimal? Qty { get; set; }
        public int Rate { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]

        [ValidateNever]
        public Category category { get; set; }
    }
}
