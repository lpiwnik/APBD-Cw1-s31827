using System.Text.Json.Serialization;
using Computer_Rental_APP.Models.Interfaces;

namespace Computer_Rental_APP.Models.Users;

[method: JsonConstructor]
public class UserRole(string name, int loanLimit, decimal penaltyRate) : IEntity, IDisplayable
{
    [JsonInclude] public int Id { get; set; }
    [JsonInclude] public string? Name { get; protected internal set; } = name;

    [JsonInclude] public int LoanLimit { get; protected internal set; } = loanLimit;
    [JsonInclude] public decimal PenaltyRate { get; protected internal set; } = penaltyRate;


    public string ToShortRow() => $"{Id,-5} | {Name,-25} | {LoanLimit,-15} | {PenaltyRate,-6}";

    public string ToTemplateRow() => ToShortRow();
}