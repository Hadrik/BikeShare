using BikeShare.Web.Models;
using BikeShare.Web.Services;
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
    
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await service.UpdateStatus(id, status);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
    
    [HttpGet("{id:int}/status")]
    public async Task<IActionResult> GetStatusHistory(int id)
    {
        return new JsonResult(await service.GetStatusHistory(id));
    }
}