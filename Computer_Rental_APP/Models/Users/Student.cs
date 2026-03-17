using Computer_Rental_APP.Models.Abstractions;
using Computer_Rental_APP.Models.Enums;

namespace Computer_Rental_APP.Models.Users;

public class Student(int id, string firstName, string lastName, string phone, string email, UserRole userRole,string studentNumber, string semester,StudyLevel studyLevel) 
    : User(id, firstName, lastName, phone, email, userRole)
{
    public string StudentNumber { get; set; } = studentNumber;
    public string Semester { get; set; } = semester;
    public StudyLevel StudyLevel { get; set; } = studyLevel;
}