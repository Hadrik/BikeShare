using BikeShare.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace BikeShare.Web.Controllers.Api;

[Controller]
[Route("api/rentals")]
public class ApiRentalController(RentalService service) : ControllerBase
{
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
}