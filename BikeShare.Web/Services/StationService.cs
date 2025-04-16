using BikeShare.Web.Models;
using Dapper;

namespace BikeShare.Web.Services;

public class StationService(DatabaseService db)
{
    public async Task<Station[]> GetAllStationInfo()
    {
        using var connection = db.CreateConnection();

        var qresp = await connection.QueryAsync<Station>("SELECT * FROM Stations");
        return qresp.ToArray();
    }
}