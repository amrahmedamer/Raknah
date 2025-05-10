using Microsoft.AspNetCore.Mvc;
using Raknah.Contracts.Spot;
namespace Raknah.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ParkingSpotsController(IParkingSpotServices parkingSpotServices) : ControllerBase
{
    private readonly IParkingSpotServices _parkingSpotServices = parkingSpotServices;

    [HttpPost]
    public async Task<IActionResult> UpdateSensorData([FromBody] SensorRequest data)
    {

        var result = await _parkingSpotServices.UpdateParkingSpotAsync(data);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpGet]
    public async Task<IActionResult> GetParkingSpots()
    {
        var result = await _parkingSpotServices.GetParkingSpotsAsync();
        return result.IsSuccess
                       ? Ok(result.Value.Adapt<IEnumerable<SpotResponse>>())
                       : result.ToProblem();
    }

}
