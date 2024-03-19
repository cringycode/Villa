namespace Villa.Application.Common.Interfaces;

public interface IVillaService
{
    IEnumerable<Domain.Entities.Villa> GetAllVillas();
    Domain.Entities.Villa GetVillaById(int id);
    void CreateVilla(Domain.Entities.Villa villa);
    void UpdateVilla(Domain.Entities.Villa villa);
    bool DeleteVilla(int id);
    IEnumerable<Domain.Entities.Villa> GetAvailabilityByDate(int nights, DateOnly checkInDate);
    bool IsVillaAvailableByDate(int villaId, int nights, DateOnly checkInDate);
}