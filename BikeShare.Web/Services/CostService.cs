namespace BikeShare.Web.Services;

public class CostService
{
    public int HourlyRate { get; set; } = 5;
    
    /// <summary>
    /// Calculate price based on <c>HourlyRate</c> and duration of rental.
    /// </summary>
    /// <returns><c>int</c> - total cost</returns>
    public int CalculateCost(DateTime start, DateTime end)
    {
        var duration = end - start;
        var hours = (int)Math.Ceiling(duration.TotalHours);
        return hours * HourlyRate;
    }
}