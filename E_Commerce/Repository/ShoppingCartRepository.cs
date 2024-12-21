using E_Commerce.Data;
using E_Commerce.Models;
using E_Commerce.Repository.IRepository;

namespace E_Commerce.Repository
{
    public class ShoppingCartRepository:Repository<ShoppingCart> , IShoppingCartRepository
    {
        public ShoppingCartRepository(ApplicationDbContext dbContext):base(dbContext)
        {
            
        }
    }
}
