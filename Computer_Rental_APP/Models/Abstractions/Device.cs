using System.Text.Json.Serialization;
using Computer_Rental_APP.Models.Devices;
using Computer_Rental_APP.Models.Enums;
using Computer_Rental_APP.Models.Interfaces;

namespace Computer_Rental_APP.Models.Abstractions;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Laptop), typeDiscriminator: "laptop")]
[JsonDerivedType(typeof(Projector), typeDiscriminator: "projector")]
[JsonDerivedType(typeof(Camera), typeDiscriminator: "camera")]
[method: JsonConstructor]
public abstract class Device(string name, decimal dailyRate) : IEntity, IDisplayable
{
    [JsonInclude] public int Id { get; set; }
    [JsonInclude] public string Name { get; private set; } = name;

    [JsonInclude] public decimal DailyRate { get; protected internal set; } = dailyRate;

    [JsonInclude] public DeviceState State { get; protected internal set; } = DeviceState.Available;


    public string ToShortRow() =>
        $"{Id,-5} | {$"{Name}",-25} | {State,-15} | {DailyRate,-5} | ";

    public virtual string ToTemplateRow() => ToShortRow();
}