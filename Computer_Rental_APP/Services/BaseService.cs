using System.Text.Json;
using Computer_Rental_APP.core;
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
        }
        else
        {
            _items = [];
            OnFailLoad(_items);

            if (_items.Count <= 0) return;
            SaveData();
            
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

    private OperationStatus AddItem(T? item)
    {
        if (item == null) return OperationStatus.NotFound;

        var nextId = _items.Count != 0 ? _items.Max(x => x.Id) + 1 : 1;
        item.Id = nextId;

        _items.Add(item);
        SaveData();
        return OperationStatus.Success;
    }

    protected OperationResult AddItemWithResult(T item,string successMsg)
    {
        var result = AddItem(item);
        return result == OperationStatus.Success
            ? OperationResult.Ok($"Successfully added {typeof(T).Name}: {successMsg}")
            : OperationResult.Failure($"Failed to add {typeof(T).Name}.", result);
    }


    private OperationStatus DeleteItem(T? item)
    {
        if (item == null) return OperationStatus.NotFound;

        _items.Remove(item);
        SaveData();
        return OperationStatus.Success;
    }
    protected OperationResult DeleteItemWithResult(int id,string successMsg)
    {
        var item = GetItemById(id);
        if (item.Status!=OperationStatus.Success) return item;
       

        var result = DeleteItem(item.ObjectData);
        return result == OperationStatus.Success
            ? OperationResult.Ok($"Successfully removed {typeof(T).Name}: {successMsg}")
            : OperationResult.Failure($"Failed to remove {typeof(T).Name} with {id}.", result);
    }

    
    private OperationStatus UpdateItem(T? item)
    {
        if (item == null) return OperationStatus.NotFound;

        SaveData();
        return OperationStatus.Success;
    }
    protected OperationResult UpdateItemProperty(int id, Action<T> updateAction, string successMsg)
    {
        var item = GetItemById(id);
        if (item.Status!=OperationStatus.Success) return item;

        updateAction(item.ObjectData!);

        var result = UpdateItem(item.ObjectData);
        return result == OperationStatus.Success
            ? OperationResult.Ok(successMsg)
            : OperationResult.Failure($"Failed to update {typeof(T).Name} in database.", result);
    }


    protected List<T> GetItemsList() => _items;
    
    
    protected OperationResult<T> GetItemById(int id)
    {
        var item = _items.FirstOrDefault(x => x.Id == id);
        if (item == null)
            return OperationResult<T>.Failure($"{typeof(T).Name} with ID {id} not found.", OperationStatus.NotFound);

        return OperationResult<T>.Success(item);
    }

    
    
}