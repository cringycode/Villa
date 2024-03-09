using Villa.Application.Common.Interfaces;
using Villa.Infrastructure.Data;

namespace Villa.Infrastructure.Repos;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;
    public IVillaRepo Villa { get; private set; }
    public IVillaNumberRepo VillaNumber { get; private set; }
    public IAmenityRepo Amenity { get; private set; }

    public UnitOfWork(AppDbContext db)
    {
        _db = db;
        Villa = new VillaRepo(_db);
        VillaNumber = new VillaNumberRepo(_db);
        Amenity = new AmenityRepo(_db);
    }

    public void Save()
    {
        _db.SaveChanges();
    }
}