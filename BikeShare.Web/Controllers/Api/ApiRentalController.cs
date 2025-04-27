using System.Security.Claims;
using BikeShare.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace BikeShare.Web.Controllers.Api;

[Controller]
[Route("api/rentals")]
public class ApiRentalController(RentalService service) : ControllerBase
{
    [HttpGet("{userId:int}")]
    public async Task<IActionResult> Active(int userId)
    {
        return Ok(await service.GetRentalOfUser(userId));
    }
    
    public class StartRentalRequest
    {
        public int UserId { get; set; }
        public int StationId { get; set; }
    }
    
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

    [HttpPost("start/{id:int}")]
    public async Task<IActionResult> Start(int id)
    {
        int userId;
        try
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) throw new Exception("User ID not found");
            userId = int.Parse(claim.Value);
        }
        catch (Exception e)
        {
            return BadRequest("User not found");
        }
        
        return await Start(new StartRentalRequest {StationId = id, UserId = userId});
    }
    
    public class EndRentalRequest
    {
        public int RentalId { get; set; }
        public int StationId { get; set; }
    }
    
    [HttpPost("end")]
    public async Task<IActionResult> End(EndRentalRequest request)
    {
        try
        {
            await service.FinishRental(request.RentalId, request.StationId);
            return NoContent();
        }
        catch (KeyNotFoundException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return Problem(e.Message);
        }
    }
    
    [HttpPost("end/{id:int}")]
    public async Task<IActionResult> End(int id)
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
        
        var rentalId = await service.GetRentalOfUser(userId);
        if (rentalId == null)
        {
            return BadRequest("User has no active rental");
        }
        
        return await End(new EndRentalRequest {RentalId = rentalId.Value, StationId = id});
    }
}