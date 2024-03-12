using Villa.Application.Common.Interfaces;
using Villa.Domain.Entities;
using Villa.Infrastructure.Data;

namespace Villa.Infrastructure.Repos;

public class BookingRepo : Repo<Booking>, IBookingRepo
{
    private readonly AppDbContext _db;

    public BookingRepo(AppDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(Booking entity)
    {
        _db.Bookings.Update(entity);
    }
}