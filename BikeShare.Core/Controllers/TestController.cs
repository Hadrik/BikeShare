using BikeShare.Core.Utils;
using Microsoft.AspNetCore.Mvc;

namespace BikeShare.Core.Controllers;

public class TestController(DbConnection db) : ControllerBase
{
    [HttpGet("/")]
    public IActionResult Get()
    {
        return Ok("Hello World");
    }
}