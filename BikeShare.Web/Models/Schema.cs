using System.Data;
using Dapper;

namespace BikeShare.Web.Models;

[Table("Stations")]
public record Station
{
    [Key]
    [Column("station_id")]
    public int Id { get; set; }
    
    [Column("name")]
    public required string Name { get; set; }
    
    [Column("location_long")]
    public double Latitude { get; set; }
    
    [Column("location_lat")]
    public double Longitude { get; set; }
}

[Table("Bikes")]
public record Bike
{
    [Key]
    [Column("bike_id")]
    public int Id { get; set; }
    
    [Column("station_id")]
    public int? StationId { get; set; }
    
    [Column("status")]
    public BikeStatus Status { get; set; }
    
    [Column("last_status_change")]
    public DateTime LastUpdated { get; set; }
}

[Table("Rentals")]
public record Rental
{
    [Key]
    [Column("rental_id")]
    public int Id { get; set; }
    
    [Column("bike_id")]
    public int BikeId { get; set; }
    
    [Column("user_id")]
    public int UserId { get; set; }
    
    [Column("start_station_id")]
    public int StartStationId { get; set; }
    
    [Column("end_station_id")]
    public int? EndStationId { get; set; }
    
    [Column("start_timestamp")]
    public DateTime StartTimestamp { get; set; }
    
    [Column("end_timestamp")]
    public DateTime? EndTimestamp { get; set; }
    
    [Column("distance")]
    public int? Distance { get; set; }
    
    [Column("cost")]
    public int? Cost { get; set; }
}

[Table("StatusHistory")]
public record StatusHistory
{
    [Key]
    [Column("status_history_id")]
    public int Id { get; set; }

    [Column("bike_id")] 
    public int BikeId { get; set; }

    [Column("station_id")]
    public int? StationId { get; set; }
    
    [Column("status")]
    public BikeStatus Status { get; set; }
    
    [Column("timestamp")]
    public DateTime Timestamp { get; set; }
}

[Table("Users")]
public record User
{
    [Key]
    [Column("user_id")]
    public int Id { get; set; }
    
    [Column("role_id")]
    public int RoleId { get; set; }
    
    [Column("email")]
    public required string Email { get; set; }
    
    [Column("username")]
    public required string Username { get; set; }
    
    [Column("password_hash")]
    public required string PasswordHash { get; set; }
}

[Table("Roles")]
public record Role
{
    [Key]
    [Column("role_id")]
    public int Id { get; set; }
    
    [Column("name")]
    public required string Name { get; set; }
    
    [Column("permissions")]
    public required int Permissions { get; set; }
}


public class DateTimeToUnixHandler : SqlMapper.TypeHandler<DateTime>
{
    public override void SetValue(IDbDataParameter parameter, DateTime value)
    {
        parameter.Value = new DateTimeOffset(value).ToUnixTimeSeconds();
    }

    public override DateTime Parse(object value)
    {
        var unixTime = Convert.ToInt64(value);
        return DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;
    }
}

// https://stackoverflow.com/questions/37264655/dapper-and-enums-as-strings
public struct BikeStatus
{
    string _value;

    public static BikeStatus Available => "Available";
    public static BikeStatus Maintenance => "Maintenance";
    public static BikeStatus InUse => "InUse";

    private BikeStatus(string value)
    {
        _value = value;
    }

    public static implicit operator BikeStatus(string value)
    {
        return new BikeStatus(value);
    }

    public static implicit operator string(BikeStatus country)
    {
        return country._value;
    }
}

public class BikeStatusHandler : SqlMapper.ITypeHandler
{
    public void SetValue(IDbDataParameter parameter, object value)
    {
        parameter.DbType = DbType.String;
        parameter.Value = (string)((dynamic)value);
    }

    public object? Parse(Type destinationType, object value)
    {
        if (destinationType == typeof(BikeStatus))
            return (BikeStatus)((string)value);
        return null;
    }
}