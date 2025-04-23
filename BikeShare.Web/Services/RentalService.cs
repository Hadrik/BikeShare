using BikeShare.Web.Models;
using Dapper;

namespace BikeShare.Web.Services;

public class RentalService(DatabaseService db, BikeService bikeService)
{
    /// <summary>
    /// Starts a new rental. Assigns a random bike.
    /// </summary>
    /// <param name="userId">ID of user starting the rental</param>
    /// <param name="stationId">Station that the rental starts from</param>
    /// <exception cref="Exception">Error message</exception>
    /// <returns><c>int</c> - rental Id</returns>
    public async Task<int> StartRental(int userId, int stationId)
    {
        using var connection = db.CreateConnection();
        var now = DateTime.UtcNow;
        
        var bike = await connection.QuerySingleOrDefaultAsync<int>(
            "SELECT bike_id FROM Bikes WHERE station_id = @stationId AND status = @status ORDER BY RANDOM() LIMIT 1",
            new { stationId, status = "Available" });
        if (bike == 0)
            throw new Exception("No bikes available at this station");

        var rental = new Rental
        {
            UserId = userId,
            BikeId = bike,
            StartStationId = stationId,
            StartTimestamp = now,
        };

        using var t = connection.BeginTransaction();
        try
        {
            var rentalId = await connection.InsertAsync(rental, t);
            await bikeService.UpdateStatus(bike, "InUse", null, now, t);
            
            t.Commit();
            return rentalId!.Value;
        }
        catch (Exception ex)
        {
            t.Rollback();
            throw new Exception("Failed to start rental", ex);
        }
    }
    
    /// <summary>
    /// Finishes a rental. Updates the bike status and rental end time.
    /// </summary>
    /// <param name="rentalId">ID of rental to finalize</param>
    /// <param name="stationId">End station ID</param>
    /// <exception cref="KeyNotFoundException">Rental not found</exception>
    /// <exception cref="Exception">Rental not found</exception>
    public async Task FinishRental(int rentalId, int stationId)
    {
        var connection = db.CreateConnection();
        var t = connection.BeginTransaction();
        var now = DateTime.UtcNow;
        
        var rental = await connection.GetAsync<Rental>(rentalId, t);
        if (rental == null)
        {
            t.Rollback();
            throw new KeyNotFoundException("No rental found");
        }
        
        var bikeSuccess = await bikeService.UpdateStatus(rental.BikeId, "Available", stationId, now, t);
        if (!bikeSuccess)
        {
            t.Rollback();
            throw new Exception("Failed to update bike status");
        }
        
        rental.EndStationId = stationId;
        rental.EndTimestamp = now;
        
        var rows = await connection.UpdateAsync(rental, t);
        if (rows == 0)
        {
            t.Rollback();
            throw new Exception("Failed to finish rental");
        }
        
        t.Commit();
    }

    public async Task<int?> GetRentalOfUser(int userId)
    {
        var connection = db.CreateConnection();
        var rental = await connection.QuerySingleOrDefaultAsync<Rental>(
            "SELECT * FROM Rentals WHERE user_id = @userId AND end_timestamp IS NULL",
            new { userId });
        return rental?.Id;
    }
}