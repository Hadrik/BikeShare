using System.Text.Json;
using BikeShare.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace BikeShare.Web.Controllers.Api;

[Controller]
[Route("api/stations")]
public class ApiStationController(StationService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var stations = await service.GetAllStationInfo();

        return Ok(JsonSerializer.Serialize(stations));
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetStation(int id)
    {
        var station = await service.GetStation(id);
        if (station == null)
        {
            return NotFound();
        }

        return Ok(JsonSerializer.Serialize(station));
    }
    
    [HttpGet("{id:int}/bikes")]
    public async Task<IActionResult> GetBikesAtStation(int id)
    {
        var bikes = await service.GetBikesAtStation(id);

        return Ok(bikes);
    }
}