using ApiHttpClient.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Web.Api.Manager.Filter
{
    public class ServiceExceptionFilters : ExceptionFilterAttribute, IAsyncExceptionFilter
    {
        private readonly ILogger _logger;

        public ServiceExceptionFilters(ILogger<ServiceExceptionFilters> logger)
        {
            _logger = logger;
        }


        public override Task OnExceptionAsync(ExceptionContext context)
        {
            if (!context.ExceptionHandled)
            {
                var exception = context.Exception;


                int statusCode;


                switch (true)
                {
                    case bool _ when exception is UnauthorizedAccessException:
                        statusCode = (int)HttpStatusCode.Unauthorized;
                        break;


                    case bool _ when exception is InvalidOperationException:
                        statusCode = (int)HttpStatusCode.BadRequest;
                        break;


                    default:
                        statusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }


                _logger.LogError($"GlobalExceptionFilter: Error in {context.ActionDescriptor.DisplayName}. {exception.Message}. Stack Trace: {exception.StackTrace}");

                var error = new ApiResponse(exception.Message) { IsError = true, StatusCode = statusCode };
                context.Result = new JsonResult(error);
                return Task.CompletedTask;
                // Custom Exception message to be returned to the UI
                // context.Result = new ObjectResult(exception.Message) { StatusCode = statusCode };
                //context.Result = new ObjectResult(new ApiResponse(exception.Message, null) { IsError = true }) { StatusCode = statusCode };
            }
            _logger.LogError($"GlobalExceptionFilter: Lỗi không xác định");
            return Task.CompletedTask;
        }
    }
}
