using System.Net;
using System.Text.Json;
using costumersApi.Exceptions;
using FluentValidation;

namespace costumersApi.Middlewares
{
  public class ErrorHandlingMiddleware
  {
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
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
      HttpStatusCode statusCode;
      object response;

      switch (exception)
      {
        case ValidationException validationException:
          statusCode = HttpStatusCode.BadRequest;
          response = new
          {
            statusCode = (int)statusCode,
            message = "Erro de validação",
            errors = validationException.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
          };
          break;

        case CostumerNotFoundException costumerNotFoundException:
          statusCode = HttpStatusCode.NotFound;
          response = new
          {
            statusCode = (int)statusCode,
            message = costumerNotFoundException.Message
          };
          break;

        default:
          statusCode = HttpStatusCode.InternalServerError;
          response = new
          {
            statusCode = (int)statusCode,
            message = "Erro inesperado",
            detail = exception.Message
          };
          break;
      }

      context.Response.ContentType = "application/json";
      context.Response.StatusCode = (int)statusCode;

      var json = JsonSerializer.Serialize(response);
      return context.Response.WriteAsync(json);
    }
  }
}