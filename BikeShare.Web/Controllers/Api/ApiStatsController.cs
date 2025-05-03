using BikeShare.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BikeShare.Web.Controllers.Api;

[Authorize(Policy = "AdminOrApp")]
[ApiController]
[Route("api/stats")]
public class ApiStatsController(StatsService service)
{
    /// <summary>
    /// Get the number of rentals started and ended at a specific station in the last month.
    /// </summary>
    /// <param name="stationId">Station ID</param>
    /// <returns>{started, ended}</returns>
    [HttpGet("station/{stationId:int}/last-month")]
    public async Task<IActionResult> RentalsByStationLastMonth(int stationId)
    {
        var period = TimeSpan.FromDays(30);
        var rentals = await service.StationRentals(stationId, period);

        return new JsonResult(new
        {
            started = rentals.Item1,
            ended = rentals.Item2
        });
    }
}