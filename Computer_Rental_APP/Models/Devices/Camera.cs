using Computer_Rental_APP.Models.Abstractions;

namespace Computer_Rental_APP.Models.Devices;

public class Camera : Device 
{
    public Camera() : base() { } // Ważny dla deserializacji
    
    public Camera(string name, decimal dailyRate, int shutterCount, string lensType)
        : base(name, dailyRate)
    {
        ShutterCount = shutterCount;
        LensType = lensType;
    }

    public int ShutterCount { get; set; }
    
    
    public string LensType { get; set; } 

    public override string ToTemplateRow()
    {
        // Sprawdź czy na pewno masz " | " przed ShutterCount
        return base.ToShortRow() + $"{ShutterCount,-15} | {LensType} ";
    }
}