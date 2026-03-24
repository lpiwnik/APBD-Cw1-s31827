using System.Text.Json.Serialization;
using Computer_Rental_APP.Models.Interfaces;
using Computer_Rental_APP.Models.Users;

namespace Computer_Rental_APP.Models.Abstractions;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Student), typeDiscriminator: "student")]
[JsonDerivedType(typeof(Employee), typeDiscriminator: "employee")]
[JsonDerivedType(typeof(StaffUser), typeDiscriminator: "staffUser")]
[method: JsonConstructor]
public abstract class User(string firstName, string lastName, string email, int roleId)
    : IEntity, IDisplayable
{
    [JsonInclude] public int Id { get; set; }
    [JsonInclude] public string FirstName { get; private set; } = firstName;

    [JsonInclude] public string LastName { get; private set; } = lastName;

    [JsonInclude] public string Email { get; protected internal set; } = email;

    [JsonInclude] public int RoleId { get; protected internal set; } = roleId;

    [JsonIgnore] public UserRole? UserRole { get; set; }


    public string ToShortRow() =>
        $"{Id,-5} | {$"{FirstName} {LastName}",-25} | {UserRole?.Name ??"Unknown",-15}";

    public virtual string ToTemplateRow() => ToShortRow();
}