using Computer_Rental_APP.core;
using Computer_Rental_APP.Models.Enums;
using Computer_Rental_APP.Models.Users;

namespace Computer_Rental_APP.Services;

public class UserRoleService(string filePath) : BaseService<UserRole>(filePath)
{
    protected override void OnFailLoad(List<UserRole> items)
    {
        if (items.Count != 0) return;
        items.Add(new UserRole("Unassigned", 0, 999.0m) { Id = 0 });
        items.Add(new UserRole("Admin", 99, 0m) { Id = 1 });
        OperationResult.Ok("System initialized with default roles: Unassigned and Admin.");
    }

    public OperationResult AddRole(UserRole role) => AddItemWithResult(role, role.Name!);


    public OperationResult UpdatePenaltyRate(int roleId, decimal penaltyRate)
    {
        if (penaltyRate < 0)
            return OperationResult.Failure(
                "Penalty rate must be greater than 0",
                OperationStatus.ValidationError
                );

        return UpdateItemProperty(
            roleId,
            r => r.PenaltyRate = penaltyRate,
        $"Successfully updated  penalty rate: {penaltyRate} for role {roleId}"
        );
}

public OperationResult UpdateLoanLimit(int roleId, int loanLimit)
    {
        if(loanLimit<0)
            return OperationResult.Failure(
                "LoanLimit must be greater than 0",
                OperationStatus.ValidationError
                );
        
        return UpdateItemProperty(
            roleId,
            r => r.LoanLimit = loanLimit,
        $"Successfully updated loan limit: {loanLimit} for role {roleId}"
        );
    }

public List<UserRole> GetRolesList() => GetItemsList();

    public OperationResult<UserRole> GetRoleById(int roleId) => GetItemById(roleId);
}