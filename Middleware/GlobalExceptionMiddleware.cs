using System.Net;
using System.Text.Json;
using InventoryManagement.Entities;

public class GlobalExceptionMiddleware
{
    // Middleware to handle exceptions globally and return a standardized error response
    private readonly RequestDelegate _next; // Delegate to the next middleware in the pipeline

    

    public GlobalExceptionMiddleware(RequestDelegate next) // Constructor to initialize the middleware with the next delegate
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context) // Method to invoke the middleware and handle exceptions
    {
        try 
        {
            await _next(context); // Call the next middleware in the pipeline and await its completion
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex); // Handle any exceptions that occur and return a standardized error response
        }
    }
    private static Task HandleExceptionAsync(HttpContext context, Exception ex) // Method to handle exceptions and return a standardized error response
    {
        var response = new ErrorResponse  // Create an error response object with the exception message and a 400 Bad Request status code
        {
            Message = ex.Message,
            StatusCode = (int)HttpStatusCode.BadRequest
        };

        context.Response.ContentType = "application/json"; // Set the response content type to JSON
        context.Response.StatusCode = response.StatusCode; // Set the response status code to the one defined in the error response (400 Bad Request)

        var json = JsonSerializer.Serialize(response); // Serialize the error response object to JSON format
// Write the JSON response to the HTTP response body
        return context.Response.WriteAsync(json); // Return a task that represents the asynchronous operation of writing the response
    }
}