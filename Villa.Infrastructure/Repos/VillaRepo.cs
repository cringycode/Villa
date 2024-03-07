using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Villa.Application.Common.Interfaces;
using Villa.Infrastructure.Data;

namespace Villa.Infrastructure.Repos;

public class VillaRepo : Repo<Domain.Entities.Villa>, IVillaRepo
{
    private readonly AppDbContext _db;

    public VillaRepo(AppDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(Domain.Entities.Villa entity)
    {
        _db.Villas.Update(entity);
    }
}