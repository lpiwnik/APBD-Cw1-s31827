using System.Text.Json.Serialization;
using Computer_Rental_APP.Models.Abstractions;

namespace Computer_Rental_APP.Models.Devices;

[method: JsonConstructor]
public class Laptop(string name, decimal dailyRate, int ramSizeGb, string processorModel)
    : Device(name, dailyRate)
{
    [JsonInclude] public int RamSizeGb { get; protected internal set; } = ramSizeGb;
    [JsonInclude] public string ProcessorModel { get; protected internal set; } = processorModel;

    public override string ToTemplateRow()
    {
        return base.ToTemplateRow() + $" | {RamSizeGb,-15} | {ProcessorModel}";
    }
}