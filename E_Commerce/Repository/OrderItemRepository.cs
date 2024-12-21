using E_Commerce.Data;
using E_Commerce.Models;
using E_Commerce.Repository.IRepository;

namespace E_Commerce.Repository
{
    public class OrderItemRepository :Repository<OrderItem> , IOrderItemRepository
    {
        public OrderItemRepository(ApplicationDbContext dbContext):base(dbContext)
        {
            
        }
    }
}
