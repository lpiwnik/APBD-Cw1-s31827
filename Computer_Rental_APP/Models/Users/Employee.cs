using System.Text.Json.Serialization;
using Computer_Rental_APP.Models.Abstractions;
using Computer_Rental_APP.Models.Enums;

namespace Computer_Rental_APP.Models.Users;

[method: JsonConstructor]
public class Employee(
    string firstName,
    string lastName,
    string email,
    int roleId,
    string employeeAlias,
    EmploymentType employmentType,
    DateTime hireDate)
    : User(firstName, lastName, email, roleId)
{
    [JsonInclude] public string EmployeeAlias { get; private set; } = employeeAlias;

    [JsonInclude] public EmploymentType EmploymentType { get; protected internal set; } = employmentType;

    [JsonInclude] public DateTime HireDate { get; private set; } = hireDate;

    public override string ToTemplateRow() =>
        base.ToTemplateRow() + $" | {EmployeeAlias,-12} |  {EmploymentType,-2} | {HireDate}";
}