namespace Raknah.Contracts.Reservation
{
    public class PendingAndCanceledReservationResponse
    {
        public string CarNumber { get; set; } = string.Empty;
        public DateTime? StartTimeOfReservation { get; set; }
        public ReservationStatus Status { get; set; }
        public int ParkingSpotId { get; set; }
    }
}
