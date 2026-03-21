using Computer_Rental_APP.Models.Abstractions;

namespace Computer_Rental_APP.Models.Devices;

public class Laptop: Device{

        public Laptop():base(){}

        public Laptop( string name, decimal dailyRate, int ramSizeGb, string processorModel)
            :base(name, dailyRate)
        {
            RamSizeGb = ramSizeGb;
            ProcessorModel = processorModel;
        }
        public int RamSizeGb { get; set; }
        public string ProcessorModel { get; set; }

        public override string ToTemplateRow()
            {
                return base.ToTemplateRow()+$"{RamSizeGb,-15} | {ProcessorModel}";
            }
}