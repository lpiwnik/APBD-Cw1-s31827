using Computer_Rental_APP.Models.Abstractions;
using Computer_Rental_APP.Models.Enums;

namespace Computer_Rental_APP.Models.Users;

public class Employee
    (int id, string firstName, string lastName, string phone,
        string email, UserRole userRole, string alias,
        EmploymentType employmentType,DateTime hireDate)
    : User(id, firstName, lastName, phone, email, userRole)
{
    public string EmployeeAlias { get; set; } = alias;
    public EmploymentType EmploymentType { get; set; }=employmentType;
    public DateTime HireDate { get; set; }=hireDate;
    
}