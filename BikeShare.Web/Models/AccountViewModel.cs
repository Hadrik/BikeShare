namespace BikeShare.Web.Models;

public class AccountViewModel
{
    public string Username { get; set; }
    public Rental? CurrentRental { get; set; }
    public Station? StartStation { get; set; }
}