using System.Text.Json;
using System.Text.Json.Serialization;
using Computer_Rental_APP.Models.Abstractions;
using Computer_Rental_APP.Models.Enums;
using Computer_Rental_APP.Models.Users;

namespace Computer_Rental_APP.Services;

public class UserService(string filePath,UserRoleService roleService):BaseService<User>(filePath)
{
    public OperationResult AddUser(User user)
    {
        user.UserRole=roleService.GetRoleById(user.RoleId);
        return AddItem(user);
    }

    protected override void OnLoad(List<User> items)
    {
        foreach (var item in items)
        {
            item.UserRole=roleService.GetRoleById(item.RoleId);
        }
    }

    public OperationResult RemoveUser(User user) =>DeleteItem(user);
    
    public List<User> GetUsersList() => GetItemsList();
    
    public List<User> GetUsersList(UserRole userRole)=>
        GetItemsList().Where(x => x.UserRole == userRole).ToList();
    
    public User? GetUserById(int userId) => GetItemById(userId);
    
}