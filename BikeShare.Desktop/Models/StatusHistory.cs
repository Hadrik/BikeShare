namespace BikeShare.Desktop.Models;

public class StatusHistory
{
    public int Id { get; set; }
    public int BikeId { get; set; }
    public int? StationId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime TimeStamp { get; set; }
}