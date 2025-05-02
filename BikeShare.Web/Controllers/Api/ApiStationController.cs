using System.Text.Json;
using BikeShare.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BikeShare.Web.Controllers.Api;

[AllowAnonymous]
[ApiController]
[Route("api/stations")]
public class ApiStationController(StationService service) : ControllerBase
{
    /// <summary>
    /// Get all station information
    /// </summary>
    /// <returns>Station array</returns>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return new JsonResult(await service.GetAllStationInfo());
    }
    
    /// <summary>
    /// Get station information by ID
    /// </summary>
    /// <param name="id">Station ID</param>
    /// <returns>Station or 404-NotFound</returns>
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
    
    /// <summary>
    /// Get the number of bikes at a station
    /// </summary>
    /// <param name="id">Station ID</param>
    /// <returns>Bike count at the station</returns>
    [HttpGet("{id:int}/bikes")]
    public async Task<IActionResult> GetBikesAtStation(int id)
    {
        return Ok(await service.GetBikesAtStation(id));
    }
}