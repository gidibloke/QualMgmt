using Application.Core;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net;

namespace Web.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;

        public ErrorHandlingMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            //This is global. Unique key validation can also be handled in the validation by injecting DbContext into the validator and checking before attempting insert
            //This makes the response a validation error and more user friendly
            catch(DbUpdateException uex)
            {
                if(uex?.InnerException?.InnerException is SqlException sqlException)
                {
                    switch (sqlException.Number)
                    {
                        case 2427: //Unique constraint error
                            //customise response
                            break;
                        case 547: // Constraint check violation
                            break;
                        case 2601: //Duplicate key row error
                            break;
                        default:
                            break;

                    }
                }
            }
            catch (Exception ex) 
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            //Other error handling logic can be added here including logging, alerts

            var response = _env.IsDevelopment()
                ? new AppException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                : new AppException(context.Response.StatusCode, "Internal Server Error");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Items["Response"] = response;
            context.Response.Redirect($"/Home/Error");
            return Task.CompletedTask;

        }
    }
}
