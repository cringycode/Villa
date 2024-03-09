namespace Villa.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IVillaRepo Villa { get; }
    IVillaNumberRepo VillaNumber { get; }
    IAmenityRepo Amenity { get; }
    void Save();
}