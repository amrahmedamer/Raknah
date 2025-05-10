namespace Raknah.Consts.Errors
{
    public static class ReservationError
    {
        public static readonly Error NotFound = new("Reservation.NotFound", "Reservation not found.", StatusCodes.Status404NotFound);
        public static readonly Error AlreadyInactive = new("Reservation.AlreadyInactive", "This reservation is already inactive.", StatusCodes.Status400BadRequest);
        public static readonly Error CancelFailed = new("Reservation.CancelFailed", "Reservation could not be canceled.", StatusCodes.Status400BadRequest);
        public static readonly Error AlreadyExists = new("Reservation.AlreadyExists", "This car already has an active or pending reservation.", StatusCodes.Status400BadRequest);
        public static readonly Error InvalidParkingSpot = new("Reservation.InvalidSpot", "Invalid Parking Spot.", StatusCodes.Status400BadRequest);
        public static readonly Error AlreadyReserved = new("ParkingSpot.AlreadyReserved", "This parking spot is already reserved.", StatusCodes.Status400BadRequest);
        public static readonly Error NotFounds = new("ParkingSpot.NotFound", "Parking spot not found.", StatusCodes.Status404NotFound);
        public static Error UnexpectedStatus => new("Reservation.UnexpectedStatus", "Reservation status is invalid.", StatusCodes.Status404NotFound);
        public static Error AlreadyCancelled => new("Reservation.AlreadyCancelled", "Reservation is already cancelled.", StatusCodes.Status404NotFound);
        public static Error NotAuthenticated => new("User.NotAuthenticated", "User is not authenticated.", StatusCodes.Status401Unauthorized);
        public static Error EspFaliure = new Error("ESP32 did not respond successfully.", "ESP32 did not respond successfully.", StatusCodes.Status404NotFound);
        public static Error NoCarDetected = new Error("No car detected in front of the gate.", "No car detected in front of the gate.", StatusCodes.Status404NotFound);
        public static Error ErrorFromGate = new Error("Unknown response from gate.", "Unknown response from gate.", StatusCodes.Status404NotFound);


    }
}
