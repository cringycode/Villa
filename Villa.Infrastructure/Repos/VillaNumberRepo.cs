using Villa.Application.Common.Interfaces;
using Villa.Domain.Entities;
using Villa.Infrastructure.Data;

namespace Villa.Infrastructure.Repos;

public class VillaNumberRepo : Repo<VillaNumber>, IVillaNumberRepo
{
    private readonly AppDbContext _db;

    public VillaNumberRepo(AppDbContext db) : base(db)
    {
        _db = db;
    }

    public void Update(VillaNumber entity)
    {
        _db.VillaNumbers.Update(entity);
    }
}