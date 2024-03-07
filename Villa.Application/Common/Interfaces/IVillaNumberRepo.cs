using Villa.Domain.Entities;

namespace Villa.Application.Common.Interfaces;

public interface IVillaNumberRepo : IRepo<VillaNumber>
{
    void Update(VillaNumber entity);
}