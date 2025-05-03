using System.Data;
using BikeShare.Web.Models;
using Dapper;

namespace BikeShare.Web.Services;

public class BikeService(DatabaseService db)
{
    public async Task<Bike[]> GetAllBikes()
    {
        using var connection = db.CreateConnection();
        var bikes = await connection.GetListAsync<Bike>();
        return bikes.Where(b => b.Status != "Deleted").ToArray();
    }
    
    public async Task<Bike?> GetBike(int id)
    {
        using var connection = db.CreateConnection();
        var bike = await connection.GetAsync<Bike>(id);
        return bike?.Status == "Deleted" ? null : bike;
    }

    public async Task AddBike(string status, int? stationId = null)
    {
        using var connection = db.CreateConnection();
        using var t = connection.BeginTransaction();

        try
        {
            if (stationId == -1) stationId = null;
            var bike = new Bike
            {
                Status = status,
                LastUpdated = DateTime.UtcNow,
                StationId = stationId
            };
            var id = await connection.InsertAsync(bike, t);
            
            var sh = new StatusHistory
            {
                BikeId = id!.Value,
                StationId = bike.StationId,
                Status = bike.Status,
                Timestamp = bike.LastUpdated
            };
            await connection.InsertAsync(sh, t);
            t.Commit();
        }
        catch (Exception e)
        {
            t.Rollback();
            throw new Exception("Failed to add bike", e);
        }
    }

    public async Task DeleteBike(int id)
    {
        using var connection = db.CreateConnection();
        using var t = connection.BeginTransaction();

        try
        {
            var bike = await connection.GetAsync<Bike>(id, t);
            if (bike == null)
            {
                throw new KeyNotFoundException($"Bike with ID {id} not found.");
            }

            await UpdateStatus(id, "Deleted", transaction: t);
            t.Commit();
        }
        catch (Exception e)
        {
            t.Rollback();
            throw new Exception("Failed to delete bike", e);
        }
    }

    /// <summary>
    /// Updates the status of a bike and saves the history. Optionally updates the station ID.
    /// </summary>
    /// <param name="id">Bike ID</param>
    /// <param name="status">New Status</param>
    /// <param name="stationId">New Station ID. <c>-1</c> for no update</param>
    /// <param name="insertionTime">Custom timestamp. Uses <c>DateTime.UtcNow;</c> if <c>null</c></param>
    /// <param name="transaction">Use existing transaction</param>
    /// <returns><c>bool</c> - success</returns>
    public async Task<bool> UpdateStatus(int id, string status, int? stationId = -1, DateTime? insertionTime = null, IDbTransaction? transaction = null)
    {
        var connection = transaction?.Connection ?? db.CreateConnection();
        var t = transaction ?? connection.BeginTransaction();
        var now = insertionTime ?? DateTime.UtcNow;

        try
        {
            var bike = await connection.GetAsync<Bike>(id, t);

            if (bike.Status == "Deleted")
            {
                // Bike is deleted, cannot update
                if (transaction == null)
                {
                    t.Commit();
                    connection.Dispose();
                }
                return false;
            }

            if (bike.Status == status && (stationId == -1 || bike.StationId == stationId))
            {
                // No change, no need to update
                if (transaction == null)
                {
                    t.Commit();
                    connection.Dispose();
                }
                return false;
            }

            if (bike.Status == "InUse" && status != "Available")
            {
                // Bike is in use and can only be set to "Available"
                if (transaction == null)
                {
                    t.Commit();
                    connection.Dispose();
                }
                return false;
            }

            if (stationId != -1)
            {
                bike.StationId = stationId;
            }

            bike.Status = status;
            bike.LastUpdated = now;
            
            await connection.UpdateAsync(bike, t);

            var sh = new StatusHistory
            {
                BikeId = id,
                StationId = bike.StationId,
                Status = status,
                Timestamp = now
            };
            await connection.InsertAsync(sh, t);

            if (transaction == null)
            {
                t.Commit();
                connection.Dispose();
            }

            return true;
        }
        catch (Exception e)
        {
            if (transaction == null)
            {
                t.Rollback();
                connection.Dispose();
            }
            throw new Exception("Failed to update bike status", e);
        }
    }
    
    public async Task<StatusHistory[]> GetStatusHistory(int id)
    {
        using var connection = db.CreateConnection();
        return (await connection.QueryAsync<StatusHistory>("SELECT status_history_id AS Id, bike_id, station_id, status, timestamp FROM StatusHistory WHERE bike_id = @id", new { id })).ToArray();
    }
}