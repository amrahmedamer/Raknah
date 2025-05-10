using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Raknah.Extensions;
using System.Security.Claims;

namespace Raknah.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationServices _reservationServices;
        public ReservationController(IReservationServices reservationServices)
        {
            _reservationServices = reservationServices;
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservationAsync([FromBody] ReservationRequest request)
        {

            var result = await _reservationServices.CreateReservationAsync(request, User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpDelete("{reservationId}")]
        public async Task<IActionResult> CancelReservationAsync(int reservationId)
        {
            var result = await _reservationServices.CancelReservationAsync(reservationId);
            return result.IsSuccess ? Ok() : result.ToProblem();

        }
        [HttpPost("OpenGate/{reservationId}")]
        public async Task<IActionResult> OpenGateAsync([FromRoute] int reservationId)
        {

            var result = await _reservationServices.OpenGateAsync(reservationId);
            return result.IsSuccess ? Ok() : result.ToProblem();
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingReservations()
        {
            var result = await _reservationServices.GetPendingReservations(User.GetUserId());
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveReservations()
        {
            var result = await _reservationServices.GetActiveReservations(User.GetUserId());
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpGet("ActiveAndPending")]
        public async Task<IActionResult> GetActiveAndPendingReservations()
        {
            var result = await _reservationServices.GetPendingOrActiveReservations(User.GetUserId());
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();

        }

        [HttpGet("Completed")]
        public async Task<IActionResult> CompletedReservations()
        {
            var result = await _reservationServices.GetCompletedReservations(User.GetUserId());

            return result.IsFailure ? result.ToProblem() : Ok(result.Value);
        }

        [HttpGet("Canceled")]
        public async Task<IActionResult> CanceledReservations()
        {
            var result = await _reservationServices.GetCanceledReservations(User.GetUserId());
            return result.IsFailure ? result.ToProblem() : Ok(result.Value);

        }

        [HttpGet("CanceledAndCompleted")]
        public async Task<IActionResult> CanceledAndCompletedReservations()
        {
            var result = await _reservationServices.GetCompletedAndCanceledReservations(User.GetUserId());
            return result.IsFailure ? result.ToProblem() : Ok(result.Value);
        }

    }
}
