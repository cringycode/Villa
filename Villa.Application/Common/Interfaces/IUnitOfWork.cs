namespace Villa.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IVillaRepo Villa { get; }
    IVillaNumberRepo VillaNumber { get; }
    void Save();
}