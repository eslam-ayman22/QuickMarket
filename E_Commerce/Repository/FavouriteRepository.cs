using E_Commerce.Data;
using E_Commerce.Migrations;
using E_Commerce.Models;
using E_Commerce.Repository.IRepository;

namespace E_Commerce.Repository
{
    public class FavouriteRepository:Repository<Favourite>, IFavouriteRepository
    {
        public FavouriteRepository(ApplicationDbContext dbContext):base(dbContext)
        {
            
        }
    }
}
