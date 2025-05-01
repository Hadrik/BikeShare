using BikeShare.Web.Models;
using BikeShare.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BikeShare.Web.Controllers.Api;

[Controller]
[Route("api/bikes")]
public class ApiBikeController(BikeService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return new JsonResult(await service.GetAllBikes());
    }
    
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
    
    [Authorize("Admin")]
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            _ = await service.UpdateStatus(id, status);
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet("{id:int}/status")]
    public async Task<IActionResult> GetStatusHistory(int id)
    {
        return new JsonResult(await service.GetStatusHistory(id));
    }
}