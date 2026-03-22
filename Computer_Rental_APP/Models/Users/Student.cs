using System.Text.Json.Serialization;
using Computer_Rental_APP.Models.Abstractions;
using Computer_Rental_APP.Models.Enums;

namespace Computer_Rental_APP.Models.Users;

[method: JsonConstructor]
public class Student(
    string firstName,
    string lastName,
    string email,
    int roleId,
    string studentNumber,
    int semester,
    StudyLevel studyLevel)
    : User(firstName, lastName, email, roleId)
{
    [JsonInclude] public string StudentNumber { get; init; } = studentNumber;

    [JsonInclude] public int Semester { get; protected internal set; } = semester;

    [JsonInclude] public StudyLevel StudyLevel { get; protected internal set; } = studyLevel;

    public override string ToTemplateRow()
    {
        return base.ToTemplateRow() + $" | {StudentNumber,-12} | {Semester,-2} | {StudyLevel}";
    }
}