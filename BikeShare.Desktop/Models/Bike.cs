namespace BikeShare.Desktop.Models;

public class Bike
{
    public int Id { get; set; }
    public int StationId { get; set; }
    public string Status { get; set; }
    public DateTime LastStatusChange { get; set; }
}