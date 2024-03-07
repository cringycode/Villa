namespace Villa.Application.Common.Interfaces;

public interface IVillaRepo : IRepo<Domain.Entities.Villa>
{
    void Update(Domain.Entities.Villa entity);
}