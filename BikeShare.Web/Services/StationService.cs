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
        var stations = await connection.GetListAsync<Station>();
        return stations.Where(s => s.Status != "Deleted").ToArray();
    }

    /// <summary>
    /// Gets a station by ID.
    /// </summary>
    /// <param name="id">Station ID</param>
    /// <returns>Station info</returns>
    public async Task<Station?> GetStation(int id)
    {
        using var connection = db.CreateConnection();
        var station = await connection.GetAsync<Station>(id);
        return station?.Status == "Deleted" ? null : station;
    }

    /// <summary>
    /// Creates a new station.
    /// </summary>
    /// <param name="name">Station name</param>
    /// <param name="latitude">Latitude</param>
    /// <param name="longitude">Longitude</param>
    public async Task CreateStation(string name, double latitude, double longitude)
    {
        using var connection = db.CreateConnection();
        var station = new Station
        {
            Name = name,
            Latitude = latitude,
            Longitude = longitude,
            Status = "Normal"
        };
        await connection.InsertAsync(station);
    }

    /// <summary>
    /// Deletes a station by ID.
    /// </summary>
    /// <param name="id">Station ID</param>
    /// <exception cref="KeyNotFoundException">Station with provided id not found</exception>
    public async Task DeleteStation(int id)
    {
        using var connection = db.CreateConnection();
        using var t = connection.BeginTransaction();

        try
        {
            var station = await connection.GetAsync<Station>(id, t);
            if (station == null)
            {
                throw new KeyNotFoundException($"Station with ID {id} not found.");
            }
            
            station.Status = "Deleted";
            await connection.UpdateAsync(station, t);
            
            t.Commit();
        }
        catch (Exception e)
        {
            t.Rollback();
            throw new Exception($"Failed to delete station with ID {id}: {e.Message}", e);
        }
    }

    /// <summary>
    /// Update station information by ID.
    /// </summary>
    /// <param name="id">Station ID</param>
    /// <param name="name">New name</param>
    /// <param name="latitude">New Latitude</param>
    /// <param name="longitude">New Longitude</param>
    /// <exception cref="KeyNotFoundException">Station with provided id not found</exception>
    /// <exception cref="InvalidOperationException">Cannot update deleted station</exception>
    public async Task UpdateStation(int id, string? name, double? latitude, double? longitude)
    {
        using var connection = db.CreateConnection();
        var station = await connection.GetAsync<Station>(id);
        if (station == null)
        {
            throw new KeyNotFoundException($"Station with ID {id} not found.");
        }
        
        if (station.Status == "Deleted")
        {
            throw new InvalidOperationException($"Station with ID {id} is deleted and cannot be updated.");
        }
        
        if (name != null)
        {
            station.Name = name;
        }
        if (latitude != null)
        {
            station.Latitude = latitude.Value;
        }
        if (longitude != null)
        {
            station.Longitude = longitude.Value;
        }
        
        await connection.UpdateAsync(station);
    }

    /// <summary>
    /// Gets the number of available bikes at a station.
    /// </summary>
    /// <param name="id">Station ID</param>
    /// <returns><c>int</c> number of bikes</returns>
    public async Task<int> GetBikesAtStation(int id)
    {
        using var connection = db.CreateConnection();
        var station = await connection.GetAsync<Station>(id);

        if (station?.Status == "Deleted")
            return 0;
        
        return await connection.QuerySingleAsync<int>(
            "SELECT COUNT(1) FROM Bikes WHERE station_id = @id AND status = @status",
            new { id, status = "Available" });
    }
}