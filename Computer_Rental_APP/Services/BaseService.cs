using System.Text.Json;
using Computer_Rental_APP.Models.Enums;
using Computer_Rental_APP.Models.Interfaces;

namespace Computer_Rental_APP.Services;

public abstract class BaseService<T> where T : IEntity
{
    private List<T> _items = [];
    private readonly string _filePath;

    protected BaseService(string filePath)
    {
        _filePath = filePath;
        LoadData();
    }

    private void LoadData()
    {
        if (File.Exists(_filePath))
        {
            try
            {
                var json = File.ReadAllText(_filePath);
                _items = JsonSerializer.Deserialize<List<T>>(json) ?? [];
                OnLoad(_items);
            }
            catch
            {
                Console.WriteLine($"File {_filePath} not found. Initializing with empty list.");
                _items = [];
                
            }
        }else
        {
            _items = []; 
            OnFailLoad(_items);

            if (_items.Count <= 0) return;
            SaveData(); 
            Console.WriteLine($"Utworzono nowy plik bazy: {_filePath}");
        }
    }

    protected virtual void OnLoad(List<T> items)
    {
    }
    protected virtual void OnFailLoad(List<T> items)
    {
    }

    private void SaveData()
    {
        var json = JsonSerializer.Serialize(
            _items,
            new JsonSerializerOptions()
            {
                WriteIndented = true
            });
        File.WriteAllText(_filePath, json);
    }

    protected OperationResult AddItem(T? item)
    {
        if (item == null) return OperationResult.NotFound;
        
        var nextId = _items.Count != 0 ? _items.Max(x => x.Id) + 1 : 1;
        item.Id = nextId;
        
        _items.Add(item);
        SaveData();
        return OperationResult.Success;
    }


    protected OperationResult DeleteItem(T? item)
    {
        if (item == null) return OperationResult.NotFound;

        _items.Remove(item);
        SaveData();
        return OperationResult.Success;
    }

    protected OperationResult UpdateItem(T? item)
    {
        if (item == null) return OperationResult.NotFound;
        
        SaveData();
        return OperationResult.Success;
    }

   


    protected List<T> GetItemsList() => _items;
    
    protected T? GetItemById(int id)=> _items.FirstOrDefault(x => x.Id == id);
    
    
}