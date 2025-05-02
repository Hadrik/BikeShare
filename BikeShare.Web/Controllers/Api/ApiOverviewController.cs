using BikeShare.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BikeShare.Web.Controllers.Api;

[ApiController]
[Route("api")]
public class ApiOverviewController(ReflectionService service) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Index()
    {
        var apiOverview = service.GetApiOverview();
        return new JsonResult(apiOverview);
    }
}