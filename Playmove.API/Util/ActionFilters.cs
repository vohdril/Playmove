using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Playmove.Util
{
    public class ActionFilters
    {
    }

    public class ModelStateValidationActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var modelState = context.ModelState;

            if (!modelState.IsValid)
                context.Result = new ContentResult()
                {
                    Content = "Houve um erro na requisição, verifique o(s) parâmetro(s) '" + string.Join(", ", modelState.Keys) + "', se o erro peristir contate o suporte!",
                    StatusCode = 400
                };
            base.OnActionExecuting(context);
        }
    }

    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        public HttpGlobalExceptionFilter()
        {
        }

        public class Response
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }

        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            var code = HttpStatusCode.InternalServerError;
            var response = new Response()
            {
                Success = false,
            };

            if (exception is ValidationException)
            {
                code = HttpStatusCode.BadRequest;
                response.Message = exception.Message;
                // We can serialize exception message here instead of throwing Bad request message
            }
            else
            {
                response.Message = "Internal Server Error";
            }

            context.Result = new JsonResult(ControllerExtensions.CreateResponseObject(false, response.Message));
            context.HttpContext.Response.StatusCode = (int)code;
            context.ExceptionHandled = true;
        }
    }
}
