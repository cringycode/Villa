using Villa.Application.Common.Interfaces;
using Villa.Domain.Entities;
using Villa.Infrastructure.Data;

namespace Villa.Infrastructure.Repos;

public class AmenityRepo : Repo<Amenity>, IAmenityRepo
{
    private readonly AppDbContext _db;

    public AmenityRepo(AppDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(Amenity entity)
    {
        _db.Amenities.Update(entity);
    }
}