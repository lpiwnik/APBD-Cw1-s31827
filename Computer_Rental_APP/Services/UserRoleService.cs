using System.Text.Json;
using Computer_Rental_APP.Models.Users;

namespace Computer_Rental_APP.Services;

public class UserRoleService
{
    private List<UserRole>? _roles;
    private readonly string _filePath;

    public UserRoleService(string filePath)
    {
        _filePath = filePath;
        LoadData();
    }

    private void LoadData()
    {
        if (!File.Exists(_filePath))
        {
            DefaultRoles(); return;
        }

        try
        {
         _roles=JsonSerializer.Deserialize<List<UserRole>>(File.ReadAllText(_filePath))
                ?? [];
         if (_roles.Count == 0) DefaultRoles();
        }catch(Exception e)
        {
            Console.WriteLine($"Error loading roles: {e.Message}");
            DefaultRoles();
        }
        
    }

    private void DefaultRoles()
    {
        _roles =
        [
            new UserRole { id = 1, Name = "Student", LoanLimit = 2, PenaltyRate = 5.0m },
            new UserRole { id = 2, Name = "Employee", LoanLimit = 5, PenaltyRate = 0.0m }
        ];
        SaveData();
    }

    private void SaveData()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath) ?? "Data");

        var json = JsonSerializer.Serialize(_roles, new JsonSerializerOptions { WriteIndented = true });

        File.WriteAllText(_filePath, json);
    }

    public UserRole? GetRoleByName(string name) =>
        _roles!.FirstOrDefault(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    
    public UserRole? GetRoleById(int userRoleId)
    {
       return _roles!.FirstOrDefault(r => r.id == userRoleId);
    }
    
    public List<UserRole>? GetRolesList() => _roles;
    
    

}