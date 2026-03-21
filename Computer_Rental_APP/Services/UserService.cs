using System.Text.Json;
using System.Text.Json.Serialization;
using Computer_Rental_APP.Models.Abstractions;
using Computer_Rental_APP.Models.Users;

namespace Computer_Rental_APP.Services;

public class UserService
{
    private List<User> _users = []; 
    private readonly string _filePath;
    private readonly UserRoleService _roleService; 
    private int _lastInt = 0;
    
    public UserService(string filePath, ref UserRoleService roleService)
    {
        _filePath = filePath;
        _roleService = roleService;
        LoadData();
    }

    
    public void AddUser(User user)
    {
        user.Id = ++_lastInt;
        
        user.UserRole =_roleService.GetRoleById(user.RoleId) ; 

        _users.Add(user);
        SaveData();
    }

    public void RemoveUser(User? user)
    {
        if (user != null) _users.Remove(user);
        SaveData();
    }
    
   
    public List<User> GetUsersList() => _users;
    
    public List<User> GetUsersList(UserRole userRole)=>_users.Where(u => u.UserRole == userRole).ToList();
   

    public User? GetUserById(int userId) => _users.FirstOrDefault(u => u.Id == userId);
    
    
    private void LoadData()
    {
        if (!File.Exists(_filePath))
        {
            _users = [];
            return;
        } 
        try 
        { 
            var json = File.ReadAllText(_filePath);
            _users = JsonSerializer.Deserialize<List<User>>(json) ?? [];
            
            foreach (var user in _users)
            {
                user.UserRole = _roleService.GetRoleById(user.RoleId);
            }

            if (_users.Count != 0) _lastInt = _users.Max(u => u.Id);
        } 
        catch(Exception e)
        {
            Console.WriteLine($"Error while loading file {_filePath}: {e.Message}");
            _users = [];
        }
    }
    
    private void SaveData()
    {
        var options = JsonSerializerOptions.Default;
        var json = JsonSerializer.Serialize(_users, options);
        
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
        
        File.WriteAllText(_filePath, json);
    }
}