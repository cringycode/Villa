using Villa.Application.Common.Interfaces;
using Villa.Domain.Entities;
using Villa.Infrastructure.Data;

namespace Villa.Infrastructure.Repos;

public class AppUserRepo : Repo<AppUser>, IAppUserRepo
{
    private readonly AppDbContext _db;

    public AppUserRepo(AppDbContext db) : base(db)
    {
        _db = db;
    }
}