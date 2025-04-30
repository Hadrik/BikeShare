using System.Text.Json;
using BikeShare.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BikeShare.Web.Controllers.Api;

[Controller]
[Route("api/stations")]
public class ApiStationController(StationService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return new JsonResult(await service.GetAllStationInfo());
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetStation(int id)
    {
        var station = await service.GetStation(id);
        if (station == null)
        {
            return NotFound();
        }

        return new JsonResult(station);
    }
    
    [HttpGet("{id:int}/bikes")]
    public async Task<IActionResult> GetBikesAtStation(int id)
    {
        return Ok(await service.GetBikesAtStation(id));
    }
}