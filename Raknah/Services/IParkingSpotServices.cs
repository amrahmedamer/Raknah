namespace Raknah.Services
{
    public interface IParkingSpotServices
    {
        Task<Result<IEnumerable<ParkingSpot>>> GetParkingSpotsAsync();
        Task<Result> UpdateParkingSpotAsync(SensorRequest parkingSpot);
    }
}
