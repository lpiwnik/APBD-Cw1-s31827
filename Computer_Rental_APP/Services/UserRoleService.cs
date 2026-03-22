using Computer_Rental_APP.Models.Users;

namespace Computer_Rental_APP.Services;

public class UserRoleService(string filePath) : BaseService<UserRole>(filePath)
{
    public UserRole? GetRoleById(int roleId) => GetItemById(roleId);

    
    protected override void OnFailLoad(List<UserRole> items)
    {
        if (items.Count != 0) return;
        AddItem(new UserRole("Student",2,5.0m));
        AddItem(new UserRole("Employee",5,0.0m));
        AddItem(new UserRole("Admin",99,0.0m));
    }

    public List<UserRole> GetRolesList() => GetItemsList();
    
    
}
