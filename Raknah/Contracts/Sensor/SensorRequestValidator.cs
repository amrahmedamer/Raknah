using FluentValidation;

namespace Raknah.Contracts.Sensor
{
    public class SensorRequestValidator : AbstractValidator<SensorRequest>
    {
        public SensorRequestValidator()
        {
            //RuleFor(x => x.SensorCode)
            //    .NotEmpty();

            //RuleFor(x => x.SensorStatus)
            //    .NotEmpty();
        }
    }
}
