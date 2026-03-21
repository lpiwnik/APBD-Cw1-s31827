using Computer_Rental_APP.Models.Abstractions;
using Computer_Rental_APP.Models.Enums;

namespace Computer_Rental_APP.Models.Users;

public class Employee : User
{
    
    public Employee() : base() { }

    
    public Employee(string firstName, string lastName, string email, int roleId) 
        : base(firstName, lastName, email, roleId)
    {
        HireDate = DateTime.Now;
    }

    private string EmployeeAlias { get; set; } = string.Empty;
    private EmploymentType EmploymentType { get; set; } = EmploymentType.FullTime;
    private DateTime HireDate { get; set; }
    
    public override string ToTemplateRow() => 
        base.ToTemplateRow()+$" | {EmployeeAlias,-12} |  {EmploymentType,-2} | {HireDate}";
}