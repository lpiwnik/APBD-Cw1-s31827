using Computer_Rental_APP.Models.Abstractions;
using Computer_Rental_APP.Models.Enums;

namespace Computer_Rental_APP.Services;

public class DeviceService(string filePath):BaseService<Device>(filePath)
{
    
    public OperationResult AddDevice(Device device) => AddItem(device);
    
    public OperationResult RemoveDevice(int deviceId)=>DeleteItem(GetItemById(deviceId));


    public OperationResult UpdateState(int deviceId,DeviceState state)
    {
        var item = GetItemById(deviceId);
        if (item == null) return OperationResult.NotFound;
        item.State = state;
        return UpdateItem(item);
    }
    
    public List<Device> GetDevicesList() => GetItemsList();
    public List<Device> GetDevicesList(DeviceState state)=>
        GetItemsList().Where(x => x.State == state).ToList();
    
    public Device? GetDevice(int deviceId) => GetItemsList().FirstOrDefault(x => x.Id == deviceId);
    
}