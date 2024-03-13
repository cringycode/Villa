using Villa.Domain.Entities;

namespace Villa.Application.Common.Interfaces;

public interface IBookingRepo : IRepo<Booking>
{
    void Update(Booking entity);
    void UpdateStatus(int bookingId, string orderStatus);
    void UpdateStripePaymentId(int bookingId, string sessionId, string paymentIntentId);
}