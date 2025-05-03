using Dapper;

namespace BikeShare.Web.Services;

public class StatsService(DatabaseService db)
{
    public async Task<Tuple<int, int>> StationRentals(int stationId, TimeSpan period)
    {
        using var connection = db.CreateConnection();
        
        var endDate = DateTime.UtcNow;
        var startDate = endDate.Subtract(period);

        var started = await connection.ExecuteScalarAsync<int>(
        """
            SELECT COUNT(*)
            FROM Rentals
            WHERE start_station_id = @stationId
            AND end_timestamp IS NOT NULL
            AND datetime(end_timestamp) BETWEEN datetime(@startDate) AND datetime(@endDate)
            """,
            new { stationId, startDate, endDate });
        
        var ended = await connection.ExecuteScalarAsync<int>(
        """
            SELECT COUNT(*)
            FROM Rentals
            WHERE end_station_id = @stationId
            AND end_timestamp IS NOT NULL
            AND datetime(end_timestamp) BETWEEN datetime(@startDate) AND datetime(@endDate)
            """,
            new { stationId, startDate, endDate });
        
        return new Tuple<int, int>(started, ended);
    }
}