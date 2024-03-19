using Microsoft.AspNetCore.Hosting;
using Villa.Application.Common.Interfaces;
using Villa.Application.Common.Utility;

namespace Villa.Application.Services.Implementation;

public class VillaService : IVillaService
{
    #region DI

    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public VillaService(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
        _unitOfWork = unitOfWork;
    }

    #endregion

    #region GET ALL VILLAS

    public IEnumerable<Domain.Entities.Villa> GetAllVillas()
    {
        return _unitOfWork.Villa.GetAll(includeProperties: "VillaAmenity");
    }

    #endregion

    #region GET VILLA

    public Domain.Entities.Villa GetVillaById(int id)
    {
        return _unitOfWork.Villa.Get(u => u.Id == id, includeProperties: "Villa");
    }

    #endregion

    #region CREATE

    public void CreateVilla(Domain.Entities.Villa villa)
    {
        if (villa.Image is not null)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(villa.Image.FileName);
            string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\VillaImage");

            using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
            villa.Image.CopyTo(fileStream);

            villa.ImageUrl = @"\images\VillaImage\" + fileName;
        }
        else
        {
            villa.ImageUrl = "https://placehold.co/600x400";
        }

        _unitOfWork.Villa.Add(villa);
        _unitOfWork.Save();
    }

    #endregion

    #region UPDATE

    public void UpdateVilla(Domain.Entities.Villa villa)
    {
        if (villa.Image is not null)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(villa.Image.FileName);
            string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"images\VillaImage");

            if (!string.IsNullOrEmpty(villa.ImageUrl))
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, villa.ImageUrl.TrimStart('\\'));

                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            using var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create);
            villa.Image.CopyTo(fileStream);

            villa.ImageUrl = @"\images\VillaImage\" + fileName;
        }

        _unitOfWork.Villa.Update(villa);
        _unitOfWork.Save();
    }

    #endregion

    #region DELETE

    public bool DeleteVilla(int id)
    {
        try
        {
            Domain.Entities.Villa? objFromDb = _unitOfWork.Villa.Get(u => u.Id == id);
            if (objFromDb is not null)
            {
                if (!string.IsNullOrEmpty(objFromDb.ImageUrl))
                {
                    var oldImagePath =
                        Path.Combine(_webHostEnvironment.WebRootPath, objFromDb.ImageUrl.TrimStart('\\'));

                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }
                }

                _unitOfWork.Villa.Remove(objFromDb);
                _unitOfWork.Save();
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    #endregion

    public IEnumerable<Domain.Entities.Villa> GetAvailabilityByDate(int nights, DateOnly checkInDate)
    {
        var villaList = _unitOfWork.Villa.GetAll(includeProperties: "VillaAmenity").ToList();
        var villaNumberList = _unitOfWork.VillaNumber.GetAll().ToList();
        var bookedVillas = _unitOfWork.Booking.GetAll
                (u => u.Status == SD.StatusApproved || u.Status == SD.StatusCheckedIn)
            .ToList();

        foreach (var villa in villaList)
        {
            int roomAvailable = SD.VillaRoomsAvailable_Count
                (villa.Id, villaNumberList, checkInDate, nights, bookedVillas);

            villa.IsAvailable = roomAvailable > 0 ? true : false;
        }

        return villaList;
    }

    public bool IsVillaAvailableByDate(int villaId, int nights, DateOnly checkInDate)
    {
        var villaNumbersList = _unitOfWork.VillaNumber.GetAll().ToList();
        var bookedVillas = _unitOfWork.Booking
            .GetAll(u => u.Status == SD.StatusApproved || u.Status == SD.StatusCheckedIn).ToList();

        int roomAvailable = SD.VillaRoomsAvailable_Count
            (villaId, villaNumbersList, checkInDate, nights, bookedVillas);

        return roomAvailable > 0;
    }
}