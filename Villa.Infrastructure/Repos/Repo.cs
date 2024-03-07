using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Villa.Application.Common.Interfaces;
using Villa.Infrastructure.Data;

namespace Villa.Infrastructure.Repos;

public class Repo<T> : IRepo<T> where T : class
{
    private readonly AppDbContext _db;
    internal DbSet<T> dbSet;

    public Repo(AppDbContext db)
    {
        _db = db;
        dbSet = _db.Set<T>();
    }

    public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
    {
        IQueryable<T> query = dbSet;
        if (filter is not null)
        {
            query = query.Where(filter);
        }

        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var includeProp in includeProperties.Split
                         (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }

        return query.ToList();
    }

    public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null)
    {
        IQueryable<T> query = dbSet;
        if (filter is not null)
        {
            query = query.Where(filter);
        }

        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var includeProp in includeProperties.Split
                         (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }

        return query.FirstOrDefault();
    }

    public void Add(T entity)
    {
        dbSet.Add(entity);
    }

    public bool Any(Expression<Func<T, bool>> filter)
    {
        return dbSet.Any(filter);
    }

    public void Remove(T entity)
    {
        dbSet.Remove(entity);
    }
}