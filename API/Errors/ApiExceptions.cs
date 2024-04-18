namespace API;


//Error handling/for exception handling middleware
public class ApiException
{
    //Create a constructor
    public ApiException(int statusCode, string message, string details)
    {
        StatusCode = statusCode;
        Message = message;
        Details = details;
    }

    
    public int StatusCode { get; set; }
    public string Message { get; set; }
    //Details will contains the stack trace
    public string Details { get; set; }
}