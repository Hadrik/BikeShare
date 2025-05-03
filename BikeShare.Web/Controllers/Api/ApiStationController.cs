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

    public class StationRequest
    {
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    /// <summary>
    /// Update station information by ID
    /// </summary>
    /// <param name="id">Station ID</param>
    /// <param name="request">[FromBody] {Name, Latitude, Longitude}</param>
    /// <returns>200-Ok or 404-NotFount for invalid ID or 400-BadRequest with error message</returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateStation(int id, [FromBody] StationRequest request)
    {
        try
        {
            await service.UpdateStation(id, request.Name, request.Latitude, request.Longitude);
            return Ok();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    /// <summary>
    /// Create a new station
    /// </summary>
    /// <param name="request">[FromBody] {Name, Latitude, Longitude}</param>
    /// <returns>200-Ok or 400-BadRequest with error message</returns>
    [HttpPost]
    public async Task<IActionResult> AddStation([FromBody] StationRequest request)
    {
        try
        {
            await service.CreateStation(request.Name, request.Latitude, request.Longitude);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    /// <summary>
    /// Delete a station by ID
    /// </summary>
    /// <param name="id">Station ID</param>
    /// <returns>200-Ok or 404-NotFound for invalid station ID</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteStation(int id)
    {
        try
        {
            await service.DeleteStation(id);
            return Ok();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
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