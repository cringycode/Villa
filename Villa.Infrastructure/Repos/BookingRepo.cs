using Villa.Application.Common.Interfaces;
using Villa.Application.Common.Utility;
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

    public void UpdateStatus(int bookingId, string bookingStatus, int villaNumber = 0)
    {
        var bookingFromDb = _db.Bookings.FirstOrDefault(x => x.Id == bookingId);
        if (bookingFromDb is not null)
        {
            bookingFromDb.Status = bookingStatus;
            if (bookingStatus == SD.StatusCheckedIn)
            {
                bookingFromDb.VillaNumber = villaNumber;
                bookingFromDb.ActualCheckInDate = DateTime.Now;
            }
            if (bookingStatus == SD.StatusCompleted)
            {
                bookingFromDb.ActualCheckOutDate = DateTime.Now;
            }
        }
    }

    public void UpdateStripePaymentId(int bookingId, string sessionId, string paymentIntentId)
    {
        var bookingFromDb = _db.Bookings.FirstOrDefault(x => x.Id == bookingId);
        if (bookingFromDb is not null)
        {
            if (!string.IsNullOrEmpty(sessionId))
            {
                bookingFromDb.StripeSessionId = sessionId;
            }

            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                bookingFromDb.StripePaymentIntentId = paymentIntentId;
                bookingFromDb.PaymentDate = DateTime.Now;
                bookingFromDb.IsPaymentSuccessful = true;
            }
        }
    }
}