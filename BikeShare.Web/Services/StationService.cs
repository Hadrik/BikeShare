using BikeShare.Web.Models;
using Dapper;

namespace BikeShare.Web.Services;

public class StationService(DatabaseService db)
{
    /// <summary>
    /// Gets all stations in db.
    /// </summary>
    /// <returns>Array of all stations</returns>
    public async Task<Station[]> GetAllStationInfo()
    {
        using var connection = db.CreateConnection();
        return (await connection.GetListAsync<Station>()).ToArray();
    }

    /// <summary>
    /// Gets a station by ID.
    /// </summary>
    /// <param name="id">Station ID</param>
    /// <returns>Station info</returns>
    public async Task<Station?> GetStation(int id)
    {
        using var connection = db.CreateConnection();
        return await connection.GetAsync<Station>(id);
    }

    /// <summary>
    /// Gets the number of available bikes at a station.
    /// </summary>
    /// <param name="id">Station ID</param>
    /// <returns><c>int</c> number of bikes</returns>
    public async Task<int> GetBikesAtStation(int id)
    {
        using var connection = db.CreateConnection();
        return await connection.QuerySingleAsync<int>(
            "SELECT COUNT(1) FROM Bikes WHERE station_id = @id AND status = @status",
            new { id, status = "Available" });
    }
}