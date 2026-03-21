using Computer_Rental_APP.Models.Abstractions;
using Computer_Rental_APP.Models.Enums;

namespace Computer_Rental_APP.Models.Users;

public class Student : User
{
    
    public Student() : base() { }
    
    public Student(string firstName, string lastName, string email, int roleId) 
        : base(firstName, lastName, email, roleId){}

    public string StudentNumber { get; init; } = string.Empty;
    private int Semester { get; set; } = 1;
    private StudyLevel StudyLevel { get; set; } = StudyLevel.Engineer;

    public override string ToTemplateRow()
    {
        return base.ToTemplateRow()+$" | {StudentNumber,-12} | {Semester,-2} | {StudyLevel}";
    }
}