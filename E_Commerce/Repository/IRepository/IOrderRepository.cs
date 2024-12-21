using System.Linq.Expressions;
using E_Commerce.Models;

namespace E_Commerce.Repository.IRepository
{
    public interface IOrderRepository :IRepository<Order>
    {
        IEnumerable<Order> Getorder(Expression<Func<Order, bool>> expression,
     params Func<IQueryable<Order>, IQueryable<Order>>[] includeproperties);
    }
}
