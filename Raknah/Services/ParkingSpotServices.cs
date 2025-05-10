using Microsoft.AspNetCore.Identity.UI.Services;
using Raknah.Consts.Errors;
using Raknah.Extensions;
using Raknah.Persistence;

namespace Raknah.Services
{
    public class ParkingSpotServices(ApplicationDbContext context,
        IEmailSender emailSender) : IParkingSpotServices
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IEmailSender _emailSender = emailSender;

        public async Task<Result<IEnumerable<ParkingSpot>>> GetParkingSpotsAsync()
        {
            IEnumerable<ParkingSpot> result = await _context.ParkingSpots.ToListAsync();

            return result is null ? Result.Failure<IEnumerable<ParkingSpot>>(ParkingSpotError.NotFoundParkingSpot) : Result.Success(result);
        }

        public async Task<Result> UpdateParkingSpotAsync(SensorRequest updateData)
        {

            var current = await _context.ParkingSpots
                .Include(x => x.Reservations)
                .ThenInclude(x => x.User)
                .SingleOrDefaultAsync(x => x.SensorCode == updateData.SensorCode);

            if (current is null)
                return Result.Failure(ParkingSpotError.NotFoundParkingSpot);

            if (current.Reservations is not null)
            {
                foreach (var reservation in current.Reservations.Where(r => r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Active))
                {

                    if (reservation.Status == ReservationStatus.Pending)
                    {
                        reservation.Status = ReservationStatus.Active;
                        reservation.StartTimeOfParking = DateTime.Now;
                    }
                    else
                    {
                        reservation.Status = ReservationStatus.Inactive;
                        reservation.EndTimeOfParking = DateTime.Now;

                        reservation.Duration = reservation.EndTimeOfParking!.Value - reservation.StartTimeOfParking!.Value;

                        await _emailSender.SendEmailAsync(
                     reservation.User!.Email!,
                         "Completed Reservation",
                         "CompletedReservation",
                         new Dictionary<string, string>
                         {
                            { "{{FullName}}", reservation.User!.FullName  },
                            { "{{CarNumber}}",  reservation.CarNumber},
                            { "{{ParkingSpotName}}", current.Name },
                            { "{{Duration}}", reservation.Duration.ToString()!},
                         });
                    }
                }

                current.SensorStatus = updateData.SensorStatus ? SensorStatus.Availiable : SensorStatus.Occupied;
                current.SpotStatus = updateData.SensorStatus ? SpotStatus.Availiable : SpotStatus.Occupied;


            }
            await _context.SaveChangesAsync();



            return Result.Success();
        }

    }
}
