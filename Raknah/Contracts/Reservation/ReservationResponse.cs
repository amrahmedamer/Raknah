namespace Raknah.Contracts.Reservation
{
    public class ReservationResponse
    {
        public string CarNumber { get; set; } = string.Empty;
        public int ParkingSpotId { get; set; }
        public DateTime StartTimeOfReservation { get; set; }
        public ReservationStatus Status { get; set; }
    }
}
