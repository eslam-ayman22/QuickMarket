using System.Linq.Expressions;
using E_Commerce.Data;
using E_Commerce.Models;
using E_Commerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Repository
{
    public class OrderRepository :Repository<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<Order>? dbset;
        public OrderRepository(ApplicationDbContext context):base(context)
        {
            this.context = context;
            this.dbset = context.Set<Order>();
        }
        public IEnumerable<Order> Getorder(Expression<Func<Order, bool>> expression, params Func<IQueryable<Order>, IQueryable<Order>>[] includeproperties)
        {
            IQueryable<Order> query = dbset;

            // Handle null expression by returning all records
            if (expression != null)
            {
                query = query.Where(expression);
            }

            // Apply Include and ThenInclude
            foreach (var include in includeproperties)
            {
                query = include(query);
            }

            return query.ToList();
        }

    }
}
