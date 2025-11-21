using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FormSubmissions.API.Validation;

public sealed class FluentValidationActionFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var errors = new List<ValidationFailure>();

        foreach (var arg in context.ActionArguments.Values)
        {
            if (arg == null)
                continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(arg.GetType());
            var validatorObj = context.HttpContext.RequestServices.GetService(validatorType) as IValidator;

            if (validatorObj == null)
                continue;

            var validationContext = new ValidationContext<object>(arg);
            var result = await validatorObj.ValidateAsync(validationContext, context.HttpContext.RequestAborted);

            if (!result.IsValid)
                errors.AddRange(result.Errors);
        }

        if (errors.Count > 0)
        {
            var payload = errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            context.Result = new BadRequestObjectResult(new { errors = payload });
            return;
        }

        await next();
    }
}
