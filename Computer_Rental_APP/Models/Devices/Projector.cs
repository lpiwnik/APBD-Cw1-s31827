using System.Text.Json.Serialization;
using Computer_Rental_APP.Models.Abstractions;

namespace Computer_Rental_APP.Models.Devices;

[method: JsonConstructor]
public class Projector(string name, decimal dailyRate, string resolution, int lampHoursUsed) :
    Device(name, dailyRate)
{
    [JsonInclude] public string Resolution { get; protected internal set; } = resolution;
    [JsonInclude] public int LampHoursUsed { get; protected internal set; } = lampHoursUsed;

    public override string ToTemplateRow()
    {
        return base.ToTemplateRow() + $"{Resolution,-15} | {LampHoursUsed}";
    }
}