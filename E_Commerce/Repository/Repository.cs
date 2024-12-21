using System.Linq.Expressions;
using E_Commerce.Data;
using E_Commerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace E_Commerce.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext dbContext;
        private readonly DbSet<T> dbset;
        public Repository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            dbset = dbContext.Set<T>();
        }

        public void commit()
        {
            dbContext.SaveChanges();
        }
        public void Add(T entity)
        {
            dbset.Add(entity);
            dbContext.SaveChanges();
        }

        public void AddRange(IEnumerable<T> values)
        {
            dbset.AddRange(values);
            dbContext.SaveChanges();
        }

        public void Delete(T entity)
        {
            dbset.Remove(entity);
            dbContext.SaveChanges();
        }

        public void DeleteRange(IEnumerable<T> values)
        {
            dbset.RemoveRange(values);
            dbContext.SaveChanges();
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includeproperties)
        {
            IQueryable<T> query = dbset;
            if(expression != null)
            {
                query = query.Where(expression);
            }
            foreach(var property in includeproperties)
            {
                query = query.Include(property);
            }
            return query.ToList();
        }

        public T GetOne(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includeproperties)
        {
            return Get(expression, includeproperties).FirstOrDefault();
        }

        public void Update(T entity)
        {
            dbset.Update(entity);
            dbContext.SaveChanges();
        }

        public void UpdateRange(IEnumerable<T> values)
        {
            dbset.UpdateRange(values);
            dbContext.SaveChanges();
        }
    }
}
