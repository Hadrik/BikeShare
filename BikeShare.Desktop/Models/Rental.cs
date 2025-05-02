namespace BikeShare.Desktop.Models;

public class Rental
{
    public int? Id { get; set; }
    public int BikeId { get; set; }
    public int UserId { get; set; }
    public int StartStationId { get; set; }
    public int? EndStationId { get; set; }
    public DateTime StartTimestamp { get; set; }
    public DateTime? EndTimestamp { get; set; }
    public int? Cost { get; set; }
}