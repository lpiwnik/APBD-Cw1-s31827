using System.Text.Json.Serialization;
using Computer_Rental_APP.Models.Abstractions;
using Computer_Rental_APP.Models.Enums;
using Computer_Rental_APP.Models.Interfaces;

namespace Computer_Rental_APP.Models.Loans;

[method: JsonConstructor]
public class Loan(int userId, int deviceId, DateTime dueDateTime) : IEntity, IDisplayable
{
    [JsonInclude] public int Id { get; set; }
    [JsonInclude] public int UserId { get; private set; } = userId;
    [JsonInclude] public int DeviceId { get; private set; } = deviceId;
    [JsonInclude] public DateTime LoanDate { get; private set; } = DateTime.Now;
    [JsonInclude] public DateTime DueDateTime { get; protected internal set; } = dueDateTime;
    [JsonInclude] public DateTime ReturnedDateTime { get; protected internal set; }
    [JsonInclude] public LoanStatus LoanStatus { get; protected internal set; } = LoanStatus.Active;
    [JsonInclude] public decimal TotalFee { get; protected internal set; }
    
    [JsonIgnore] public User? User { get; set; }
    [JsonIgnore] public Device? Device { get; set; }


    public string ToShortRow()=>
    $"{Id,-5} | " +
    $"{DeviceId,3} : {Device?.Name ?? "N/A",-15} | " +
    $"{UserId,3} : {User?.FirstName ?? "N/A"} {User?.LastName ?? "",-25} | " +
    $"{LoanStatus,-12}";
    

    public string ToTemplateRow()
    {
        var returnInfo = LoanStatus is LoanStatus.Returned or LoanStatus.ReturnedLate
            ? ReturnedDateTime.ToString("yyyy-MM-dd HH:mm")
            : "--- STILL OUT ---";
        
        var feeDisplay = TotalFee == 0 && LoanStatus == LoanStatus.Active
            ? "NOT CALCULATED YET" 
            : $"{TotalFee:0.00} PLN";
        

        return "------------------------------------------------------------\n" +
               $"LOAN DETAILS [ID: {Id}]\n" +
               $"  Status:    {LoanStatus}\n" +
               $"  User:      {User?.FirstName} {User?.LastName} (ID: {UserId})\n" +
               $"  Device:    {Device?.Name} (ID: {DeviceId})\n" +
               $"  Borrowed:  {LoanDate:yyyy-MM-dd HH:mm}\n" +
               $"  Deadline:  {DueDateTime:yyyy-MM-dd HH:mm}\n" +
               $"  Returned:  {returnInfo}\n" +
               $"  TOTAL FEE: {feeDisplay} PLN\n" +
               "------------------------------------------------------------";;
    }
}