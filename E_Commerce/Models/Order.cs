using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.Models
{
    public class Order
    {
        public int OrderId { get; set; }
       
        public DateTime? orderDate { get; set; }
        public decimal? totalprice { get; set; }
      
        public int? OrderStatusID { get; set; }
       
        public string? ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser? ApplicationUser { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
