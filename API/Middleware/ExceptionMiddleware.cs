using System.Net;
using System.Text.Json;

namespace API.Errors;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    //RequestDelegate represents the logic that handles an incoming HTTP request
    //middleware goes from one bit of middleware to the next bit of middleware, always calling next
    //It's what's going to happen after I've done my part
    // Or Who's the next bit of middleware that we need to go onto?
    //Create a constructor, use the logger to log the exceptions 
    // so that we see the outputs inside the terminal
    //IHostEnvironment allow us to see whether we're running in development mode or in production mode.
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger,
        IHostEnvironment env)
    {
        _env = env;
        _logger = logger;
        _next = next;
    }
    
    //this method has to be called invoke async because we're relying on the framework
    //to recognize or we're going to tell our framework that this is middleware
    //the middleware, the framework is going to expect to see a method called invoke async 
    //Context gives us access to the HTTP request that's currently being passed through the middleware
    public async Task InvokeAsync(HttpContext context)
    {
        //Try catch is responsible for handling the exceptions
        try
        {
            await _next(context);
        }
        //To handle the exception/what to do with the exceptions
        catch (Exception ex)
        {
            //Log the error with logger to the terminal
            _logger.LogError(ex, ex.Message);
            //this is the part where we're returning something to the client
            context.Response.ContentType = "application/json";
            //going to give it a status code of 500
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            //Check the environment if we are in development mode
            //What are we going to do if we are in development mode and what are we going to do if we are not?
            var response = _env.IsDevelopment()
            //If we are in development mode, we need to put the ? here, cause if we dont cause an exception
            //we might break the internet
                ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                //If we are not in development mode, return internal server error
                : new ApiException(context.Response.StatusCode, ex.Message, "Internal server error");

            //Return the response as JSON
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            //Create the json response, pass our response and the options as its arguments
            var json = JsonSerializer.Serialize(response, options);

            //return as our HTTP response
            await context.Response.WriteAsync(json);
        }
    }
}