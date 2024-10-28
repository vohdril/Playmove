using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BaseProject.API.Util
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public ValidateModelStateAttribute()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var erro = string.Join(" ", context.ModelState.GetErrors());
                // When ModelState is not valid, we throw exception
                throw new ValidationException(erro);
            }
        }
    }
    public static class ModelStateExtensions
    {
        public static List<string> GetErrors(this ModelStateDictionary modelState)
        {
            var validationErrors = new List<string>();

            foreach (var state in modelState)
            {
                validationErrors.AddRange(state.Value.Errors
                    .Select(error => error.ErrorMessage)
                    .ToList());
            }

            return validationErrors;
        }
    }
}
