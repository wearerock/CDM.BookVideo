using System.Text.Json;
using System.Text;
using CDM.BookVideo.Domain.Exceptions;

namespace CDM.BookVideo.API.Extensions {
  /// <summary>
  /// Extension methods for IApplicationBuilder
  /// </summary>
  public static class ApplicationBuilderExtensions {
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder applicationBuilder) {
      return applicationBuilder.Use(async (context, next) => {
        try {
          await next();
        }
        catch (ValidationException ex) {
          var jsonMessage = JsonSerializer.Serialize(ex.Message);
          var data = Encoding.UTF8.GetBytes(jsonMessage);
          context.Response.ContentType = "application/json";
          context.Response.StatusCode = (int)ex.StatusCode;
          await context.Response.Body.WriteAsync(data, 0, data.Length);
        }
      });
    }
  }
}
