namespace Raknah.Entity;

public class ParkingSpot
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public SpotStatus SpotStatus { get; set; } = SpotStatus.Availiable;
    public SensorStatus SensorStatus { get; set; } = SensorStatus.Availiable;
    public string SensorCode { get; set; } = default!;
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
