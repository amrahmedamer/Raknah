using FluentValidation;

namespace Raknah.Contracts.Reservation
{
    public class ReservationRequestValidation : AbstractValidator<ReservationRequest>
    {
        public ReservationRequestValidation()
        {




            RuleFor(car => car.CarNumber)
            .NotEmpty().WithMessage("Car number is required.")
            .Matches(@"^\d{4}[A-Za-z]{3}$")
            .WithMessage("Car number must be exactly 4 digits followed by 3 letters.");

            RuleFor(x => x.ParkingSpotId)
              .NotEmpty();

        }


    }
}
