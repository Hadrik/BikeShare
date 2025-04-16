using BikeShare.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace BikeShare.Web.Controllers;

[Route("[controller]/{id:int}")]
public class StationController(StationService service) : Controller
{
    public async Task<IActionResult> Index(int id)
    {
        var station = await service.GetStation(id);
        if (station == null)
            return NotFound();
        
        var bikes = await service.GetBikesAtStation(id);
        ViewBag.Station = station;
        ViewBag.Bikes = bikes;
        
        return View();
    }
}