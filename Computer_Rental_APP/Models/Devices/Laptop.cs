using Computer_Rental_APP.Models.Abstractions;

namespace Computer_Rental_APP.Models.Devices;

public class Laptop(int id,string name, decimal dailyRate, int ramSizeGb, string processorModel)
    : Device(id,name, dailyRate)

{
    public int RamSizeGb { get; } = ramSizeGb;
    public string ProcessorModel { get; } = processorModel;
}