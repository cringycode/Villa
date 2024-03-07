using System.Linq.Expressions;

namespace Villa.Application.Common.Interfaces;

public interface IVillaRepo
{
    IEnumerable<Domain.Entities.Villa> GetAll
        (Expression<Func<Domain.Entities.Villa, bool>>? filter = null, string? includeProperties = null);

    Domain.Entities.Villa Get
        (Expression<Func<Domain.Entities.Villa, bool>> filter, string? includeProperties = null);

    void Add(Domain.Entities.Villa entity);
    void Update(Domain.Entities.Villa entity);
    void Remove(Domain.Entities.Villa entity);
    void Save();
}