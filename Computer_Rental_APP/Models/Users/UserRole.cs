namespace Computer_Rental_APP.Models.Enums;


public class UserRole
{
    public int id { get; set; }
    public string Name { get; set; }
    public int LoanLimit { get; set; }
    public decimal PenaltyRate { get; set; }
    
}