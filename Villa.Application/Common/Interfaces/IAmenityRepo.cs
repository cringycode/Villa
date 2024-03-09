using Villa.Domain.Entities;

namespace Villa.Application.Common.Interfaces;

public interface IAmenityRepo : IRepo<Amenity>
{
    void Update(Amenity entity);
}