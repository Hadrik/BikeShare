using System.Security.Claims;
using BikeShare.Web.Models;
using BikeShare.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace BikeShare.Web.Controllers;

public class AccountController(RentalService rentalService, StationService stationService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
            return RedirectToAction("Login", "Auth");

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
        
        var xrental = await rentalService.GetExtendedRentalOfUser(userId);
        
        var rentalHistory = await rentalService.GetExtendedRentalHistoryOfUser(userId);
        
        return View(new AccountViewModel
        {
            Username = User.Identity?.Name,
            CurrentRental = xrental,
            PastRentals = rentalHistory
        });
    }
}