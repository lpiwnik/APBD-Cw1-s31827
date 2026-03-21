using Computer_Rental_APP.Models.Abstractions;

namespace Computer_Rental_APP.Models.Devices;

public class Projector:Device {
    public Projector():base(){}

    public Projector(string name, decimal dailyRate, string resolution, int lampHoursUsed)
        : base(name, dailyRate)
    {
        Resolution = Resolution;
        LampHoursUsed = LampHoursUsed;
    }
    
    private string Resolution { get; set; }
    private int LampHoursUsed { get; set; }
    
    public override string ToTemplateRow()
    {
        return base.ToTemplateRow()+$"{Resolution,-15} | {LampHoursUsed}";
    }
}