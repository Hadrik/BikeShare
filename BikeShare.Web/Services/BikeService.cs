using BikeShare.Web.Models;
using Dapper;

namespace BikeShare.Web.Services;

public class BikeService(DatabaseService db)
{
    public async Task<Bike[]> GetAllBikes()
    {
        using var connection = db.CreateConnection();
        return (await connection.GetListAsync<Bike>()).ToArray();
    }
    
    public async Task<Bike?> GetBike(int id)
    {
        using var connection = db.CreateConnection();
        return await connection.GetAsync<Bike>(id);
    }

    public async Task<bool> UpdateStatus(int id, BikeStatus status)
    {
        using var connection = db.CreateConnection();
        using var t = connection.BeginTransaction();
        var time = DateTime.UtcNow;
        
        var bike = await connection.GetAsync<Bike>(id, transaction: t);
        if (bike == null)
        {
            return false;
        }
        bike.Status = status;
        bike.LastUpdated = time;
        var rows = await connection.UpdateAsync(bike, transaction: t);
        if (rows == 0)
        {
            t.Rollback();
            return false;
        }
        
        var history = await connection.InsertAsync(new StatusHistory
        {
            BikeId = id,
            StationId = bike.StationId,
            Status = status,
            Timestamp = time
        }, transaction: t);
        if (history == null)
        {
            t.Rollback();
            return false;
        }
        
        t.Commit();
        return true;
    }
    
    public async Task<StatusHistory[]> GetStatusHistory(int id)
    {
        using var connection = db.CreateConnection();
        return (await connection.QueryAsync<StatusHistory>("SELECT * FROM StatusHistory WHERE bike_id = @id", new { id })).ToArray();
    }
}