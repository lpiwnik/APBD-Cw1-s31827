using Computer_Rental_APP.Models.Abstractions;

namespace Computer_Rental_APP.Models.Devices;

public class Projector(int id,string name, decimal dailyRate, string resolution, int lampHoursUsed)
    : Device(id,name, dailyRate)
{
    
    public string Resolution { get; protected set; } = resolution;
    public int LampHoursUsed { get; protected set; } = lampHoursUsed;
}