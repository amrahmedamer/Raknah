namespace Raknah.Entity;

public class Reservation
{
    public int Id { get; set; }
    public string CarNumber { get; set; } = string.Empty;
    public DateTime? StartTimeOfReservation { get; set; }
    public DateTime? StartTimeOfParking { get; set; }
    public DateTime? EndTimeOfParking { get; set; }

    public TimeSpan? Duration { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

    public string UserId { get; set; } = null!;
    public ApplicationUser? User { get; set; }

    public int ParkingSpotId { get; set; }
    public ParkingSpot? ParkingSpot { get; set; }
}
