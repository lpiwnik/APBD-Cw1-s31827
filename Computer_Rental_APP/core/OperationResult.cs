using Computer_Rental_APP.Models.Enums;

namespace Computer_Rental_APP.core;

public class OperationResult
{
    public bool Success { get; private set; }
    public string Message { get; private set; }
    public OperationStatus Status { get; private set; } 
    


    protected OperationResult(bool success, string message, OperationStatus status)
    {
        Success = success;
        Message = message;
        Status = status;
    }

    public static OperationResult Ok(string message ="Success") 
        => new(true, message, OperationStatus.Success);

    public static OperationResult Failure(string message, OperationStatus status) 
        => new(false, message, status);
}

public class OperationResult<T> : OperationResult
{
    public T? ObjectData { get; private set; }
    
    private OperationResult(bool success, string message, OperationStatus status, T? objectData) 
        : base(success, message, status) 
    {
        ObjectData = objectData;
    }

    
    public static OperationResult<T> Success(T data, string message = "Success") 
        => new(true, message, OperationStatus.Success, data);

    
    public new static OperationResult<T> Failure(string message, OperationStatus status) 
        => new(false, message, status, default);
    
    
}