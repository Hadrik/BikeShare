using BikeShare.Web.Models;
using Dapper;

namespace BikeShare.Web.Services;

public class StationService(DatabaseService db)
{
    public async Task<Station[]> GetAllStationInfo()
    {
        using var connection = db.CreateConnection();
        return (await connection.GetListAsync<Station>()).ToArray();
    }

    public async Task<Station?> GetStation(int id)
    {
        using var connection = db.CreateConnection();
        return await connection.GetAsync<Station>(id);
    }

    public async Task<Bike[]> GetBikesAtStation(int id)
    {
        using var connection = db.CreateConnection();
        return (await connection.GetListAsync<Bike>(
            "WHERE station_id = @id AND status = @availableStatus",
            new { id, availableStatus = BikeStatus.Available })).ToArray();
    }
}