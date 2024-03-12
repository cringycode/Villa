using Villa.Domain.Entities;

namespace Villa.Application.Common.Interfaces;

public interface IBookingRepo : IRepo<Booking>
{
    void Update(Booking entity);
}