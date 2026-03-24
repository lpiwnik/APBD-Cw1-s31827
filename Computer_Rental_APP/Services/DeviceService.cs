using Computer_Rental_APP.core;
using Computer_Rental_APP.Models.Abstractions;
using Computer_Rental_APP.Models.Enums;

namespace Computer_Rental_APP.Services;

public class DeviceService(string filePath) : BaseService<Device>(filePath)
{
    public OperationResult AddDevice(Device device) => AddItemWithResult(device, device.Name);


    public OperationResult RemoveDevice(int deviceId) => DeleteItemWithResult(deviceId, $" Id-{deviceId}");

    public OperationResult UpdateDeviceDailyRate(int deviceId, decimal dailyRate)
    {
        if (dailyRate < 0)
            return OperationResult.Failure(
                "Daily rate cannot be negative.",
                OperationStatus.ValidationError
            );

        return UpdateItemProperty(
            deviceId,
            d => d.DailyRate = dailyRate,
            $"Successfully updated daily rate: {dailyRate} for role device {deviceId}"
        );
    }

    public OperationResult UpdateDeviceState(int deviceId, DeviceState state) =>
        UpdateItemProperty(
            deviceId,
            d => d.State = state,
            $"Successfully updated state: {state} for device {deviceId} "
        );
    
    public OperationResult MarkAsBroken(int deviceId) => 
        UpdateDeviceState(deviceId, DeviceState.Broken);


    public List<Device> GetDevicesList() => GetItemsList();

    public List<Device> GetDevicesList(DeviceState state) =>
        GetItemsList().Where(x => x.State == state).ToList();

    public OperationResult<Device> GetDeviceById(int deviceId) =>GetItemById(deviceId);
}