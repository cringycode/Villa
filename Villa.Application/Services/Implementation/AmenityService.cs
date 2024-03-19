using Villa.Application.Common.Interfaces;
using Villa.Domain.Entities;

namespace Villa.Application.Services.Implementation;

public class AmenityService(IUnitOfWork unitOfWork) : IAmenityService
{
    #region DI

    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    #endregion

    #region GET ALL

    public IEnumerable<Amenity> GetAllAmenities()
    {
        return _unitOfWork.Amenity.GetAll(includeProperties: "Villa");
    }

    #endregion

    #region GET

    public Amenity GetAmenityById(int id)
    {
        return _unitOfWork.Amenity.Get(u => u.Id == id, includeProperties: "Villa");
    }

    #endregion

    #region CREATE

    public void CreateAmenity(Amenity amenity)
    {
        ArgumentNullException.ThrowIfNull(amenity);

        _unitOfWork.Amenity.Add(amenity);
        _unitOfWork.Save();
    }

    #endregion

    #region UPDATE

    public void UpdateAmenity(Amenity amenity)
    {
        ArgumentNullException.ThrowIfNull(amenity);

        _unitOfWork.Amenity.Update(amenity);
        _unitOfWork.Save();
    }

    #endregion

    #region DELETE

    public bool DeleteAmenity(int id)
    {
        try
        {
            var amenity = _unitOfWork.Amenity.Get(u => u.Id == id);

            if (amenity is not null)
            {
                _unitOfWork.Amenity.Remove(amenity);
                _unitOfWork.Save();
                return true;
            }
            else
            {
                throw new InvalidOperationException($"Amenity with ID {id} not found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return false;
    }

    #endregion
}