using Computer_Rental_APP.core;
using Computer_Rental_APP.Models.Enums;
using Computer_Rental_APP.Models.Loans;

namespace Computer_Rental_APP.Services;

public class LoanService(string filePath, UserService userService, DeviceService deviceService)
    : BaseService<Loan>(filePath)
{
    protected override void OnLoad(List<Loan> items)
    {
        foreach (var loan in items)
        {
            loan.User=userService.GetUserById(loan.UserId).ObjectData;
            loan.Device=deviceService.GetDeviceById(loan.DeviceId).ObjectData;
            if (loan.User == null)
                OperationResult.Failure(
                    $"Integrity Error: Loan {loan.Id} refers to missing User {loan.UserId}",
                    OperationStatus.CriticalError
                    );
            
            if (loan.Device == null)
                OperationResult.Failure(
                    $"Integrity Error: Loan {loan.Id} refers to missing Device {loan.DeviceId}",
                    OperationStatus.CriticalError
                    );
        }
        
    }

    public OperationResult CreateLoan(Loan loan)
    {
        var deviceResult = deviceService.GetDeviceById(loan.DeviceId);
        var userResult = userService.GetUserById(loan.UserId);

        if (deviceResult.Status != OperationStatus.Success) 
            return deviceResult;

        if (userResult.Status != OperationStatus.Success) 
            return userResult;

        var device = deviceResult.ObjectData!;
        var user = userResult.ObjectData!;
        var role = user.UserRole;

        loan.Device = device;
        loan.User = user;

        var activeLoansCount = GetItemsList().Count(l => 
            l.UserId == loan.UserId && l.LoanStatus is LoanStatus.Active or LoanStatus.Overdue);

        if (user.RoleId == 0 || role == null) 
            return OperationResult.Failure("User has no assigned role.", OperationStatus.ValidationError);

        if (activeLoansCount >= role.LoanLimit) 
            return OperationResult.Failure($"Limit exceeded ({role.LoanLimit}).", OperationStatus.LimitExceeded);

        if (device.State != DeviceState.Available) 
            return OperationResult.Failure("Device busy.", OperationStatus.Busy);

        loan.SnapDeviceDailyRate = device.DailyRate;
        loan.SnapPenaltyRate = role.PenaltyRate;
        loan.LoanStatus = LoanStatus.Active;
        

        var updateResult = deviceService.UpdateDeviceState(loan.DeviceId, DeviceState.Rented);
        if (!updateResult.Success) 
            return updateResult;

        return AddItemWithResult(loan, $" for user: {user.Id}, for device: {device.Id}");
    }

    
    public OperationResult ReturnLoan(int loanId)
    {
        var dateEnd = DateTime.Now;
        var loanResult = GetItemById(loanId);
        if (loanResult.Status != OperationStatus.Success) return loanResult;

        var loan = loanResult.ObjectData!;

        if (loan.LoanStatus is not (LoanStatus.Active or LoanStatus.Overdue))
            return OperationResult.Failure("Loan is already returned.", OperationStatus.ValidationError);

       
        var totalRentedDays = (decimal)Math.Max(1, Math.Ceiling((dateEnd - loan.LoanDate).TotalDays));

      
        decimal daysOverdue = 0;
        if (dateEnd > loan.DueDateTime)
        {
            daysOverdue = (decimal)Math.Ceiling((dateEnd - loan.DueDateTime).TotalDays);
        }

       
        var standardPrice = totalRentedDays * loan.SnapDeviceDailyRate;
        var penaltyFee = daysOverdue * loan.SnapPenaltyRate;
        var totalFee = standardPrice + penaltyFee;
    
        var finalStatus = daysOverdue > 0 ? LoanStatus.ReturnedLate : LoanStatus.Returned;

        if (loan.Device != null || loan.DeviceId > 0)
        {
            deviceService.UpdateDeviceState(loan.DeviceId, DeviceState.Available);
        }

        return UpdateItemProperty(
            loanId,
            l =>
            {
                l.ReturnedDateTime = dateEnd;
                l.TotalFee = totalFee;
                l.LoanStatus = finalStatus;
            },
            $"Returned ID:{loanId}. Total: {totalFee:C} (Rent: {standardPrice:C}, Penalty: {penaltyFee:C})"
        );
    }
    
    public List<Loan> GetActiveUserLoans(int userId) => 
        GetItemsList().Where(l => l.UserId == userId && l.LoanStatus == LoanStatus.Active).ToList();
    
    public List<Loan> GetOverdueLoans() => 
        GetItemsList().Where(l => l.LoanStatus == LoanStatus.Active && DateTime.Now > l.DueDateTime).ToList();
    
    public void SimulateDueDate(int loanId, DateTime newDue) =>
        UpdateItemProperty(loanId, l => l.DueDateTime = newDue, $"DueDate backdated for loan {loanId}");
    public List<Loan> GetLoansList() => GetItemsList();
    public OperationResult<Loan> GetLoanById(int loanId) => GetItemById(loanId);

}
