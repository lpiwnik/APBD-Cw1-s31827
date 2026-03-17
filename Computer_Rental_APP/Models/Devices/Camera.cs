using Computer_Rental_APP.Models.Abstractions;

namespace Computer_Rental_APP.Models.Devices;

public class Camera(int id,string name, decimal dailyRate, int shutterCount, string lensType)
    : Device(id,name, dailyRate)
{
    public int ShutterCount { get; set; } = shutterCount;
    public string LensType { get; } = lensType;
}