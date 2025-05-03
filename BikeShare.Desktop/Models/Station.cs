namespace BikeShare.Desktop.Models;

public partial class Station
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int LastMonthRentalsStarted { get; set; }
    public int LastMonthRentalsEnded { get; set; }
    public bool CanRent { get; set; }
}