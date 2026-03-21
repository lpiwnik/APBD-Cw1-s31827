using System.Text.Json.Serialization;
using Computer_Rental_APP.Models.Devices;
using Computer_Rental_APP.Models.Enums;
using Computer_Rental_APP.Models.Interfaces;

namespace Computer_Rental_APP.Models.Abstractions;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Laptop), typeDiscriminator: "laptop")]
[JsonDerivedType(typeof(Projector), typeDiscriminator: "projector")]
[JsonDerivedType(typeof(Camera), typeDiscriminator: "camera")]
public abstract class Device : IDisplayable
{
    protected Device(){}

    protected Device(string name, decimal dailyRate)
    {
        Name = name;
        DailyRate = dailyRate;
        State = DeviceState.Available;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public decimal DailyRate { get; set; }
    public DeviceState State { get; set; }


    public string ToShortRow() =>
        $"{Id,-5} | {$"{Name}",-25} | {State,-15} | {DailyRate, -5} | ";

    public virtual string ToTemplateRow()=>ToShortRow();
}
