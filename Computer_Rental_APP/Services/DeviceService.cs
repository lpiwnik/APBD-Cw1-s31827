using System.Text.Json;
using Computer_Rental_APP.Models.Abstractions;
using Computer_Rental_APP.Models.Enums;

namespace Computer_Rental_APP.Services;

public class DeviceService
{
    private List<Device> _device=[];
    private readonly string _filePath;
    private int _lastInt;

    public DeviceService(string filePath)
    {
        _filePath = filePath;
        LoadData();
    }

    private void LoadData()
    {
        if (!File.Exists(_filePath))
        {
            _device = [];
            return;
        }

        try
        {
            var json = File.ReadAllText(_filePath);
            _device = JsonSerializer.Deserialize<List<Device>>(json) ?? [];
            _lastInt = _device.Max(d => d.Id);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error loading file {_filePath}: {e.Message}");
            _device = [];
        }
    }

    private void SaveData()
    {
        var options = JsonSerializerOptions.Default;
        var json = JsonSerializer.Serialize(_device, options);
        
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
        
        File.WriteAllText(_filePath, json);
    }
    public void AddDevice(Device device)
    {
        device.Id=++_lastInt;
        _device.Add(device);
        SaveData();
    }

    public void RemoveDevice(int deviceId)
    {
        _device.Remove(_device.First(device => device.Id == deviceId));
        SaveData();
    }
    public List<Device> GetDevicesList()=> _device;
    
    
    public List<Device> GetDevicesList(DeviceState state)=> _device.Where(device => device.State==state).ToList();
    
    public Device GetDevice(int deviceId)=> _device.First(device => device.Id == deviceId);

    public void UpdateState(int deviceId,DeviceState state)
    {
        GetDevice(deviceId).State=state;

    }
}