using System.Security.Claims;
using BikeShare.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace BikeShare.Web.Controllers.Api;

[ApiController]
[Route("api/rentals")]
public class ApiRentalController(RentalService service) : ControllerBase
{
    /// <summary>
    /// Get active rental of a user from user ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Rental or null</returns>
    [HttpGet("active/{userId:int}")]
    public async Task<IActionResult> Active(int userId)
    {
        return new JsonResult(await service.GetRentalOfUser(userId));
    }

    /// <summary>
    /// Get active rental of the logged-in user
    /// </summary>
    /// <returns>Rental or null</returns>
    [HttpGet("active")]
    public async Task<IActionResult> Active()
    {
        int userId;
        try
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return BadRequest("User ID not found");
            userId = int.Parse(claim.Value);
        }
        catch (Exception e)
        {
            return BadRequest("User not found");
        }
        return new JsonResult(await service.GetRentalOfUser(userId));
    }
    
    public class StartRentalRequest
    {
        public int UserId { get; set; }
        public int StationId { get; set; }
    }
    
    /// <summary>
    /// Start a new rental
    /// </summary>
    /// <param name="request">{UserId, StationId}</param>
    /// <returns>201-Created or 400-BadRequest</returns>
    [HttpPost("start")]
    public async Task<IActionResult> Start(StartRentalRequest request)
    {
        try
        {
            var id = await service.StartRental(request.UserId, request.StationId);
            return CreatedAtAction(nameof(End), new { id }, null);
        } 
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Start a new rental for the logged-in user
    /// </summary>
    /// <param name="stationId">Start Station ID</param>
    /// <returns>201-Created or 400-BadRequest</returns>
    [HttpPost("start/{stationId:int}")]
    public async Task<IActionResult> Start(int stationId)
    {
        int userId;
        try
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return BadRequest("User ID not found");
            userId = int.Parse(claim.Value);
        }
        catch (Exception e)
        {
            return BadRequest("User not found");
        }
        
        return await Start(new StartRentalRequest {StationId = stationId, UserId = userId});
    }
    
    public class EndRentalRequest
    {
        public int RentalId { get; set; }
        public int StationId { get; set; }
    }
    
    /// <summary>
    /// End a rental
    /// </summary>
    /// <param name="request">{RentalId, StationId}</param>
    /// <returns>204-NoContent or 400-BadRequest with error message</returns>
    [HttpPost("end")]
    public async Task<IActionResult> End(EndRentalRequest request)
    {
        try
        {
            await service.FinishRental(request.RentalId, request.StationId);
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    /// <summary>
    /// End a rental for the logged-in user
    /// </summary>
    /// <param name="stationId">End Station ID</param>
    /// <returns>204-NoContent or 400-BadRequest with error message</returns>
    [HttpPost("end/{stationId:int}")]
    public async Task<IActionResult> End(int stationId)
    {
        int userId;
        try
        {
            userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
        catch (Exception e)
        {
            return BadRequest("User not found");
        }
        
        var rental = await service.GetRentalOfUser(userId);
        if (rental == null)
        {
            return BadRequest("User has no active rental");
        }
        
        return await End(new EndRentalRequest {RentalId = rental.Id!.Value, StationId = stationId});
    }
}