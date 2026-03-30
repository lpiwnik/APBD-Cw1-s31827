using System.Text.Json;
using System.Text.Json.Serialization;
using Computer_Rental_APP.core;
using Computer_Rental_APP.Models.Abstractions;
using Computer_Rental_APP.Models.Enums;
using Computer_Rental_APP.Models.Users;

namespace Computer_Rental_APP.Services;

public class UserService(string filePath, UserRoleService roleService) : BaseService<User>(filePath)
{
    protected override void OnLoad(List<User> items)
    {
        foreach (var item in items)
        {
            item.UserRole = roleService.GetRoleById(item.RoleId).ObjectData;
            if (item.UserRole == null)
            {
                OperationResult.Failure($"Critical Integrity Error: User {item.Id} has no valid role."
                    , OperationStatus.CriticalError);
            }
                
        }
    }

    protected override void OnFailLoad(List<User> items)
    {
        items.Add(new StaffUser("Super-Admin", "Super-Admin", "admin@admin.com", 1)
        {
            UserRole = roleService.GetRoleById(1).ObjectData,
        });
    }

    public OperationResult AddUser(User user)
    {
        var role=roleService.GetRoleById(user.RoleId);
        if (role.ObjectData == null)
            return role;
        
        user.UserRole=role.ObjectData;

        if (user is Employee newEmployee)
        {
            

            if (GetItemsList().OfType<Employee>()
                .Any(e => e.EmployeeAlias.Equals(
                    newEmployee.EmployeeAlias, 
                    StringComparison.OrdinalIgnoreCase)
                ))
                return OperationResult.Failure(
                    $"Employee alias {newEmployee.EmployeeAlias} is already taken.",
                    OperationStatus.AlreadyExists);
        }
        else if (user is Student newStudent)
        {
            if (GetItemsList().OfType<Student>()
                .Any(s => s.StudentNumber.Equals(newStudent.StudentNumber, StringComparison.OrdinalIgnoreCase)))
                return OperationResult.Failure(
                    $"Student with ID number {newStudent.StudentNumber} already exists in the system.",
                    OperationStatus.AlreadyExists);
        }
        
        return AddItemWithResult(user,user.FirstName + " " + user.LastName);
    }

    public OperationResult RemoveUser(int userId)=>DeleteItemWithResult(userId,"with "+userId);
    

    public OperationResult UpdateUserEmail(int userId, string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            return OperationResult.Failure("Invalid email format.", OperationStatus.ValidationError);
        }

        return UpdateItemProperty(
            userId,
            u => u.Email = email,
            $"Successfully updated email for user ID({userId})"
        );
    }

    public OperationResult UpdateUserRole(int userId, int roleId)
    {
        var role = roleService.GetRoleById(roleId);
        
        if (role.ObjectData == null)
            return role;
        

        return UpdateItemProperty(
            userId,
            u =>
            {
                u.RoleId = roleId;
                u.UserRole = role.ObjectData;
            },
            $"Changed role to {role.ObjectData!.Name} for user ID({userId})");
    }

    
    


    public List<User> GetUsersList() => GetItemsList();

    public List<User> GetUsersList(UserRole userRole) =>
        GetItemsList().Where(x => x.UserRole == userRole).ToList();

    public OperationResult<User> GetUserById(int userId) => GetItemById(userId);
}