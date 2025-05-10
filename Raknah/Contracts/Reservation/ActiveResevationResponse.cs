namespace Raknah.Contracts.Reservation
{
    public class ActiveReservationResponse
    {
        public string CarNumber { get; set; } = string.Empty;
        public DateTime? StartTimeOfParking { get; set; }
        public ReservationStatus Status { get; set; }
        public int ParkingSpotId { get; set; }
    }
}
