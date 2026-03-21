using Computer_Rental_APP.Models.Interfaces;

namespace Computer_Rental_APP.Models.Users;


public class UserRole : IDisplayable
{
    public int id { get; init; }
    public string Name { get; init; }
    public int LoanLimit { get; set; }
    public decimal PenaltyRate { get; set; }


    public string ToShortRow() => $"{id,-5} | {Name,-25} | {LoanLimit,-15} | {PenaltyRate,-6}";
    
    public string ToTemplateRow()=>ToShortRow();
    
}