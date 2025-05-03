namespace BikeShare.Web.Models;

public class AccountViewModel
{
    public string Username { get; set; }
    public ExtendedRental? CurrentRental { get; set; }
    public ExtendedRental[] PastRentals { get; set; }
}

public record ExtendedRental : Rental
{
    public string StartStationName { get; set; }
    public string? EndStationName { get; set; }
}