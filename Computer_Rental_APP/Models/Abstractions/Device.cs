using System.Text.Json.Serialization;
using Computer_Rental_APP.Models.Devices;
using Computer_Rental_APP.Models.Enums;

namespace Computer_Rental_APP.Models.Abstractions;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Laptop), typeDiscriminator: "laptop")]
[JsonDerivedType(typeof(Projector), typeDiscriminator: "projector")]
[JsonDerivedType(typeof(Camera), typeDiscriminator: "camera")]
public abstract class Device(int id, string name, decimal dailyRate)
{
    
    public int Id { get; } = id;
    public string Name { get; protected set; } = name;
    public decimal DailyRate { get;set; } = dailyRate;
    public DeviceState State { get; protected set; } = DeviceState.Available;
    
   
}
