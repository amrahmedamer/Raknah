namespace Raknah.Consts.Errors
{
    public static class ParkingSpotError
    {
        public static Error NotFoundParkingSpot = new Error("NotFoundParkingSpot", "Parking spot not found.", StatusCodes.Status404NotFound);
    }
}
