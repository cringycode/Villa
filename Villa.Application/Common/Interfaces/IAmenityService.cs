using Villa.Domain.Entities;

namespace Villa.Application.Common.Interfaces;

public interface IAmenityService
{
    IEnumerable<Amenity> GetAllAmenities();
    void CreateAmenity(Amenity amenity);
    void UpdateAmenity(Amenity amenity);
    Amenity GetAmenityById(int id);
    bool DeleteAmenity(int id);
}