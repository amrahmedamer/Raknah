using Hangfire;
using Microsoft.AspNetCore.Identity.UI.Services;
using Raknah.Consts.Errors;
using Raknah.Extensions;
using Raknah.Persistence;
using System.Linq.Expressions;
using System.Text;

namespace Raknah.Services
{
    public class ResservationService(ApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor,
        HttpClient httpClient,
        IEmailSender emailSender
            ) : IReservationServices
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly HttpClient _httpClient = httpClient;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly int _timeoutInMinutes = 15;


        public async Task<Result> CancelReservationAsync(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.ParkingSpot)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null)
                return Result.Failure(ReservationError.NotFound);

            if (reservation.Status == ReservationStatus.Inactive)
                return Result.Failure(ReservationError.AlreadyCancelled);

            if (reservation.Status == ReservationStatus.Pending)
            {
                reservation.Status = ReservationStatus.Canceled;

                if (reservation.ParkingSpot != null)
                {
                    reservation.ParkingSpot.SpotStatus = SpotStatus.Availiable;
                }

                await _context.SaveChangesAsync();
                return Result.Success();
            }

            if (reservation.Status == ReservationStatus.Active)
            {
                reservation.EndTimeOfParking = DateTime.Now;
                reservation.Duration = reservation.EndTimeOfParking!.Value - reservation.StartTimeOfParking!.Value;
                reservation.Status = ReservationStatus.Canceled;

                if (reservation.ParkingSpot != null)
                {
                    reservation.ParkingSpot.SpotStatus = SpotStatus.Availiable;
                }

                await _context.SaveChangesAsync();
                return Result.Success();
            }

            return Result.Failure(ReservationError.UnexpectedStatus);
        }

        public async Task<Result<ReservationResponse>> CreateReservationAsync(ReservationRequest request, string userId)
        {
            var existing = await _context.Reservations
                .AnyAsync(r => r.CarNumber == request.CarNumber &&
                (r.Status == ReservationStatus.Active || r.Status == ReservationStatus.Pending));

            if (existing)
                return Result.Failure<ReservationResponse>(ReservationError.AlreadyExists);

            var spot = await _context.ParkingSpots.FindAsync(request.ParkingSpotId);

            if (spot == null)
                return Result.Failure<ReservationResponse>(ReservationError.InvalidParkingSpot);

            if (spot.SpotStatus != SpotStatus.Availiable)
                return Result.Failure<ReservationResponse>(ReservationError.AlreadyReserved);

            var reservation = request.Adapt<Reservation>();

            if (string.IsNullOrEmpty(userId))
                return Result.Failure<ReservationResponse>(ReservationError.NotAuthenticated);

            reservation.UserId = userId;
            reservation.StartTimeOfReservation = DateTime.Now;
            reservation.Status = ReservationStatus.Pending;

            _context.Reservations.Add(reservation);
            spot.SpotStatus = SpotStatus.Reserved;

            await _context.SaveChangesAsync();

            // Schedule a background job to cancel if not arrived in 15 minutes
            BackgroundJob.Schedule(
               () => ReminderReservation(reservation.Id),
               TimeSpan.FromSeconds(_timeoutInMinutes - 5)
            );

            BackgroundJob.Schedule(
                () => CancelReservationIfNotArrivedAsync(reservation.Id),
                TimeSpan.FromSeconds(_timeoutInMinutes)
            );

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            await _emailSender.SendEmailAsync(user!.Email!, "Reservation Successful", "ReservationSuccessful", new Dictionary<string, string>
            {
                { "{{FullName}}", user!.FullName  },
                { "{{CarNumber}}", reservation.CarNumber },
                { "{{ParkingSpotName}}", reservation.ParkingSpot!.Name },
                { "{{StartTimeOfReservation}}", reservation.StartTimeOfReservation.ToString()! }
            });

            return Result.Success(reservation.Adapt<ReservationResponse>());
        }

        public async Task<Result> OpenGateAsync(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.ParkingSpot)
                .FirstOrDefaultAsync(r => r.Id == reservationId && r.Status == ReservationStatus.Pending);

            if (reservation == null)
                return Result.Failure(ReservationError.NotFound);

            try
            {
                string esp32Url = "http://192.168.100.50/data";
                var jsonData = "{ \"message\": \"Open Door\" }";
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(esp32Url, content);

                if (!response.IsSuccessStatusCode)
                    return Result.Failure(ReservationError.EspFaliure);

                string responseBody = await response.Content.ReadAsStringAsync();

                if (responseBody.Contains("Gate opened", StringComparison.OrdinalIgnoreCase))
                {
                    // Update reservation status if needed here
                    return Result.Success();
                }
                else if (responseBody.Contains("No car", StringComparison.OrdinalIgnoreCase))
                {
                    return Result.Failure(ReservationError.NoCarDetected);
                }
                else
                {
                    return Result.Failure(ReservationError.ErrorFromGate);
                }
            }
            catch (Exception ex)
            {
                // يمكنك تسجيل الخطأ هنا ex.Message
                return Result.Failure(ReservationError.EspFaliure);
            }
        }

        public async Task CancelReservationIfNotArrivedAsync(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.ParkingSpot)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null || reservation.Status != ReservationStatus.Pending)
                return;

            var spot = await _context.ParkingSpots.FindAsync(reservation.ParkingSpotId);

            if (spot == null)
                return;

            // ✅ Check if the sensor detected a car (car has arrived)
            bool hasArrived = spot.SpotStatus == SpotStatus.Occupied; // Make sure this is updated by your sensor

            if (!hasArrived)
            {
                await CancelReservationAsync(reservationId);

                await _emailSender.SendEmailAsync(reservation.User!.Email!, "Reservation Cancelled", "CanceledReservation", new Dictionary<string, string>
                {
                    { "{{FullName}}",  reservation.User!.FullName},
                    { "{{CarNumber}}",  reservation.CarNumber},
                    { "{{ParkingSpotName}}", reservation.ParkingSpot!.Name },
                    { "{{StartTimeOfReservation}}", reservation.StartTimeOfReservation.ToString()! },
                });
            }
        }
        public async Task ReminderReservation(int reservationId)
        {
            var reservation = await _context.Reservations
               .Include(r => r.ParkingSpot)
               .Include(r => r.User)
               .FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null || reservation.Status != ReservationStatus.Pending)
                return;

            await _emailSender.SendEmailAsync(reservation.User!.Email!, "Reminder Reservation", "ReminderReservation", new Dictionary<string, string>
            {
                { "{{FullName}}",  reservation.User!.FullName},
                { "{{CarNumber}}",  reservation.CarNumber},
                { "{{ParkingSpotName}}", reservation.ParkingSpot!.Name },
                { "{{StartTimeOfReservation}}", reservation.StartTimeOfReservation.ToString()! },
            });
        }

        public async Task<Result<ActiveReservationResponse>> GetActiveReservations(string userId)
            => await GetStatusAsync<ActiveReservationResponse>(userId, x => x.Status == ReservationStatus.Active);

        public async Task<Result<PendingAndCanceledReservationResponse>> GetPendingReservations(string userId)
              => await GetStatusAsync<PendingAndCanceledReservationResponse>(userId, x => x.Status == ReservationStatus.Pending);

        public async Task<Result<List<CompletedAndCanceledReservationResponse>>> GetCompletedReservations(string userId)
            => await GetStatusAsync<List<CompletedAndCanceledReservationResponse>>(userId, x => x.Status == ReservationStatus.Inactive);

        public async Task<Result<List<PendingAndCanceledReservationResponse>>> GetCanceledReservations(string userId)
            => await GetStatusAsync<List<PendingAndCanceledReservationResponse>>(userId, x => x.Status == ReservationStatus.Canceled);

        public async Task<Result<List<CompletedAndCanceledReservationResponse>>> GetCompletedAndCanceledReservations(string userId)
            => await GetStatusAsync<List<CompletedAndCanceledReservationResponse>>(userId, x => x.Status == ReservationStatus.Canceled || x.Status == ReservationStatus.Inactive);
        public async Task<Result<List<PendingAndActiveReservationResponse>>> GetPendingOrActiveReservations(string userId)
          => await GetStatusAsync<List<PendingAndActiveReservationResponse>>(userId, x => x.Status == ReservationStatus.Pending || x.Status == ReservationStatus.Active);

        public async Task<Result<T>> GetStatusAsync<T>(string userId, Expression<Func<Reservation, bool>> status)
        {
            var statusReservation = _context.Reservations
               .Where(r => r.UserId == userId)
               .Where(status)
               .Include(r => r.ParkingSpot);

            object data;

            var isSingleResult = await _context.Reservations
            .AnyAsync(r => r.UserId == userId && r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Active);


            if (isSingleResult)
                data = await statusReservation.FirstOrDefaultAsync();
            else
                data = await statusReservation.ToListAsync();

            if (data is null || (data is List<Reservation> list && !list.Any()))
                return Result.Failure<T>(ReservationError.NotFound);

            return Result.Success(data.Adapt<T>());
        }


    }
}
