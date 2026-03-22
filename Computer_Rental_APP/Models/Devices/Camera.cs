using System.Text.Json.Serialization;
using Computer_Rental_APP.Models.Abstractions;

namespace Computer_Rental_APP.Models.Devices;

[method: JsonConstructor]
public class Camera(string name, decimal dailyRate, int shutterCount, string lensType)
    : Device(name, dailyRate)
{
    [JsonInclude] public int ShutterCount { get; protected internal set; } = shutterCount;

    [JsonInclude] public string LensType { get; protected internal set; } = lensType;

    public override string ToTemplateRow()
    {
        // Sprawdź czy na pewno masz " | " przed ShutterCount
        return base.ToShortRow() + $"{ShutterCount,-15} | {LensType} ";
    }
}