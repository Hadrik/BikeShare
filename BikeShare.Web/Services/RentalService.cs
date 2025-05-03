using BikeShare.Web.Models;
using Dapper;

namespace BikeShare.Web.Services;

public class RentalService(DatabaseService db, BikeService bikeService, CostService costService)
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

        var active = await connection.QuerySingleOrDefaultAsync<int>(
            "SELECT rental_id FROM Rentals WHERE user_id = @userId AND end_timestamp IS NULL",
            new { userId });
        if (active != 0)
            throw new Exception("User already has an active rental");

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
    /// Finishes a rental. Updates the bike status, rental end time and cost.
    /// </summary>
    /// <param name="rentalId">ID of rental to finalize</param>
    /// <param name="stationId">End station ID</param>
    /// <exception cref="KeyNotFoundException">Rental not found</exception>
    /// <exception cref="Exception">Rental not found</exception>
    public async Task FinishRental(int rentalId, int stationId)
    {
        using var connection = db.CreateConnection();
        using var t = connection.BeginTransaction();
        var now = DateTime.UtcNow;

        var rental = await connection.GetAsync<Rental>(rentalId, t);
        if (rental == null)
        {
            t.Rollback();
            throw new KeyNotFoundException("No rental found");
        }

        try
        {
            await bikeService.UpdateStatus(rental.BikeId, "Available", stationId, now, t);
        }
        catch (Exception ex)
        {
            t.Rollback();
            throw new Exception("Failed to update bike status", ex);
        }

        rental.EndStationId = stationId;
        rental.EndTimestamp = now;
        rental.Cost = costService.CalculateCost(rental.StartTimestamp, rental.EndTimestamp.Value);

        var rows = await connection.UpdateAsync(rental, t);
        if (rows == 0)
        {
            t.Rollback();
            throw new Exception("Failed to finish rental");
        }

        t.Commit();
    }

    public async Task<Rental?> GetRentalOfUser(int userId)
    {
        using var connection = db.CreateConnection();
        var rental = await connection.QuerySingleOrDefaultAsync<Rental>(
            "SELECT rental_id AS Id, user_id, bike_id, start_station_id, end_station_id, start_timestamp, end_timestamp, cost FROM Rentals WHERE user_id = @userId AND end_timestamp IS NULL",
            new { userId });
        return rental;
    }
    
    public async Task<ExtendedRental?> GetExtendedRentalOfUser(int userId)
    {
        using var connection = db.CreateConnection();
        var rental = await connection.QuerySingleOrDefaultAsync<ExtendedRental>(
            """
            SELECT rental_id AS Id, user_id, bike_id, start_station_id, end_station_id, start_timestamp, end_timestamp, cost, s.name AS StartStationName
            FROM Rentals r
            JOIN Stations s ON s.station_id = r.start_station_id
            WHERE user_id = @userId
            AND end_timestamp IS NULL
            """,
            new { userId });
        return rental;
    }

    public async Task<ExtendedRental[]> GetExtendedRentalHistoryOfUser(int userId)
    {
        using var connection = db.CreateConnection();
        var rentals = await connection.QueryAsync<ExtendedRental>(
            """
            SELECT rental_id AS Id, user_id, bike_id, start_station_id, end_station_id, start_timestamp, end_timestamp, cost, s.name AS StartStationName, s2.name AS EndStationName
            FROM Rentals r
            JOIN Stations s ON s.station_id = r.start_station_id
            JOIN Stations s2 ON s2.station_id = r.end_station_id
            WHERE user_id = @userId
            AND end_timestamp IS NOT NULL
            """,
            new { userId });
        return rentals.ToArray();
    }
}