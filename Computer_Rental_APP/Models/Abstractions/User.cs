using System.Text.Json.Serialization;
using Computer_Rental_APP.Models.Interfaces;
using Computer_Rental_APP.Models.Users;

namespace Computer_Rental_APP.Models.Abstractions;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Student), typeDiscriminator: "student")]
[JsonDerivedType(typeof(Employee), typeDiscriminator: "employee")]
public abstract class User : IDisplayable
{
    
    public User() { }

    public User(string firstName, string lastName, string email, int roleId)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        RoleId = roleId;
    }

    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public int RoleId { get; set; }

    [JsonIgnore]
    public UserRole? UserRole { get; set; }

    public string ToShortRow() => 
        $"{Id,-5} | {$"{FirstName} {LastName}",-25} | {UserRole?.Name ?? "Unknown",-15}"; 
    public virtual string ToTemplateRow() => ToShortRow();
    
}