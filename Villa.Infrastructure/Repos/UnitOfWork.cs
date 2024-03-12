using Villa.Application.Common.Interfaces;
using Villa.Infrastructure.Data;

namespace Villa.Infrastructure.Repos;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;
    public IVillaRepo Villa { get; private set; }
    public IVillaNumberRepo VillaNumber { get; private set; }
    public IAmenityRepo Amenity { get; private set; }
    public IBookingRepo Booking { get; private set; }
    public IAppUserRepo AppUser { get; }

    public UnitOfWork(AppDbContext db, IAppUserRepo appUser)
    {
        _db = db;
        Villa = new VillaRepo(_db);
        VillaNumber = new VillaNumberRepo(_db);
        Amenity = new AmenityRepo(_db);
        Booking = new BookingRepo(_db);
        AppUser = new AppUserRepo(_db);
    }

    public void Save()
    {
        _db.SaveChanges();
    }
}