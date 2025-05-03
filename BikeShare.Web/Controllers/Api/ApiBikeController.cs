using BikeShare.Web.Models;
using BikeShare.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BikeShare.Web.Controllers.Api;

[Authorize(Policy = "AdminOrApp")]
[ApiController]
[Route("api/bikes")]
public class ApiBikeController(BikeService service) : ControllerBase
{
    /// <summary>
    /// Get deatils af all bikes
    /// </summary>
    /// <returns>Bike array</returns>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return new JsonResult(await service.GetAllBikes());
    }
    
    /// <summary>
    /// Get details of a specific bike
    /// </summary>
    /// <param name="id">Bike ID</param>
    /// <returns>Bike details or 404-NotFound</returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetBike(int id)
    {
        var bike = await service.GetBike(id);
        if (bike == null)
        {
            return NotFound();
        }

        return new JsonResult(bike);
    }

    public class UpdateStatusRequest
    {
        public string Status { get; set; } = string.Empty;
        public int? StationId { get; set; } = -1;
    }
    /// <summary>
    /// Manually update the status of a bike (usually used for maintenance)
    /// </summary>
    /// <param name="id">Bike ID</param>
    /// <param name="request">[FromBody] {Status, StationId}</param>
    /// <returns>204-NoContent if update was successful or 400-BadRequest with error message</returns>
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            _ = await service.UpdateStatus(id, request.Status, request.StationId);
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    /// <summary>
    /// Get the status history of a bike
    /// </summary>
    /// <param name="id">Bike ID</param>
    /// <returns>StatusHistory array</returns>
    [HttpGet("{id:int}/status")]
    public async Task<IActionResult> GetStatusHistory(int id)
    {
        return new JsonResult(await service.GetStatusHistory(id));
    }
}