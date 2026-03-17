using System.Text.Json.Serialization;
using Computer_Rental_APP.Models.Devices;
using Computer_Rental_APP.Models.Enums;
using Computer_Rental_APP.Models.Users;

namespace Computer_Rental_APP.Models.Abstractions;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Student), typeDiscriminator: "student")]
[JsonDerivedType(typeof(Employee), typeDiscriminator: "employee")]
public abstract class User(int id, string firstName, string lastName, string phone, string email, UserRole userRole)
{
    
    public int Id { get; set; } = id;
    public string FirstName { get; protected set; } = firstName;
    public string LastName { get; protected set; } = lastName;
    public string Phone { get; set; } = phone;
    public string Email { get; set; } = email;
    public UserRole UserRole { get; set; } = userRole;
}