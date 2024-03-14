using Villa.Domain.Entities;

namespace Villa.Application.Common.Utility;

public class SD
{
    public const string RoleCustomer = "Customer";
    public const string RoleAdmin = "Admin";
    public const string StatusPending = "Pending";
    public const string StatusApproved = "Approved";
    public const string StatusCheckedIn = "CheckedIn";
    public const string StatusCompleted = "Completed";
    public const string StatusCancelled = "Cancelled";
    public const string StatusRefunded = "Refunded";

    public static int VillaRoomsAvailable_Count(int villaId,
        List<VillaNumber> villaNumberList, DateOnly checkInDate, int nights,
        List<Booking> bookings)
    {
        var roomsInVilla = villaNumberList.Where(x => x.VillaId == villaId).Count();

        int finalAvailableRoomForAllNights = int.MaxValue;

        for (int i = 0; i < nights; i++)
        {
            var bookedVillaIdsForNight = GetOverlappingBookingsForNight(bookings, checkInDate.AddDays(i), villaId);

            if (bookedVillaIdsForNight.Count == roomsInVilla)
            {
                return 0; // No rooms available for this night
            }

            var totalAvailableRooms = roomsInVilla - bookedVillaIdsForNight.Count;
            finalAvailableRoomForAllNights = Math.Min(finalAvailableRoomForAllNights, totalAvailableRooms);
        }

        return finalAvailableRoomForAllNights;
    }

    private static List<int> GetOverlappingBookingsForNight(List<Booking> bookings, DateOnly checkInDate, int villaId)
    {
        return bookings.Where(u => u.CheckInDate <= checkInDate && u.CheckOutDate > checkInDate
                                                                && u.VillaId == villaId).Select(b => b.Id).ToList();
    }
}