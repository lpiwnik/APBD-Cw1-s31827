using Computer_Rental_APP.Models.Abstractions;

namespace Computer_Rental_APP.Models.Users;

public class StaffUser(string firstName, string lastName, string email, int roleId) : User(firstName, lastName, email, roleId)
{
    
}