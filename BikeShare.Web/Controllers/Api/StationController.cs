using System.Text.Json;
using BikeShare.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace BikeShare.Web.Controllers.Api;

[Controller]
[Route("api/stations")]
public class StationController(StationService stationService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var stations = await stationService.GetAllStationInfo();

        return Ok(JsonSerializer.Serialize(stations));
    }
}