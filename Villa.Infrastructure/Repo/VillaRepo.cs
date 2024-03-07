using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Villa.Application.Common.Interfaces;
using Villa.Infrastructure.Data;

namespace Villa.Infrastructure.Repo;

public class VillaRepo : IVillaRepo
{
    private readonly AppDbContext _db;

    public VillaRepo(AppDbContext db)
    {
        _db = db;
    }

    public IEnumerable<Domain.Entities.Villa> GetAll(Expression<Func<Domain.Entities.Villa, bool>>? filter = null,
        string? includeProperties = null)
    {
        IQueryable<Domain.Entities.Villa> query = _db.Set<Domain.Entities.Villa>();
        if (filter is not null)
        {
            query = query.Where(filter);
        }
        
        if (!string.IsNullOrEmpty(includeProperties))
        {
            // Villa, VillaNumber
            foreach (var includeProp in includeProperties.Split
                         (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }
        
        return query.ToList();
    }

    public Domain.Entities.Villa Get(Expression<Func<Domain.Entities.Villa, bool>> filter,
        string? includeProperties = null)
    {
        IQueryable<Domain.Entities.Villa> query = _db.Set<Domain.Entities.Villa>();
        if (filter is not null)
        {
            query = query.Where(filter);
        }
        
        if (!string.IsNullOrEmpty(includeProperties))
        {
            // Villa, VillaNumber
            foreach (var includeProp in includeProperties.Split
                         (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }
        
        return query.FirstOrDefault();
    }

    public void Add(Domain.Entities.Villa entity)
    {
        _db.Add(entity);
    }

    public void Update(Domain.Entities.Villa entity)
    {
        _db.Villas.Update(entity);
    }

    public void Remove(Domain.Entities.Villa entity)
    {
        _db.Remove(entity);
    }

    public void Save()
    {
        _db.SaveChanges();
    }
}