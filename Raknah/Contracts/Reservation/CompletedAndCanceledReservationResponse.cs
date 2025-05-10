namespace Raknah.Contracts.Reservation
{
    public class CompletedAndCanceledReservationResponse
    {
        public string CarNumber { get; set; } = string.Empty;
        public DateTime? StartTimeOfParking { get; set; }
        public DateTime? EndTimeOfParking { get; set; }
        public TimeSpan? Duration { get; set; }
        public ReservationStatus Status { get; set; }
        public int ParkingSpotId { get; set; }
    }
}
