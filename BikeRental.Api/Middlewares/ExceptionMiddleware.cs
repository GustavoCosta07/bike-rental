using System.Text.Json;
using BikeRental.Application.Models;
using FluentValidation;

namespace BikeRental.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            ErrorResponseDto response;

            switch (exception)
            {
                case ValidationException validationEx:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response = new ErrorResponseDto("Invalid data: " + string.Join("; ", validationEx.Errors.Select(e => e.ErrorMessage)));
                    break;

                case KeyNotFoundException keyNotFoundEx:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    response = new ErrorResponseDto(keyNotFoundEx.Message ?? "Resource not found");
                    break;

                case InvalidOperationException invalidOpEx:
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    response = new ErrorResponseDto(invalidOpEx.Message);
                    break;

                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response = new ErrorResponseDto("An unexpected error occurred");
                    break;
            }

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}