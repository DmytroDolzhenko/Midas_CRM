using Application.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filters
{
    public class ValidationFilter(IServiceProvider serviceProvider) : IAsyncActionFilter
    {
        public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
        {
            public override void OnException(ExceptionContext context)
            {
                switch (context.Exception)
                {
                    case ValidationException validationException:
                        HandleValidationException(context, validationException);
                        break;
                    case NotFoundException notFoundException:
                        HandleNotFoundException(context, notFoundException);
                        break;
                    default:
                        base.OnException(context);
                        break;
                }
            }

            private void HandleValidationException(ExceptionContext context, ValidationException exception)
            {
                var errors = exception.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                var details = new ValidationProblemDetails(errors)
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "Validation Error",
                    Detail = "One or more validation errors occurred."
                };

                context.Result = new BadRequestObjectResult(details);
                context.ExceptionHandled = true;
            }

            private void HandleNotFoundException(ExceptionContext context, NotFoundException exception)
            {
                var details = new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Title = "The specified resource was not found.",
                    Status = StatusCodes.Status404NotFound,
                    Detail = exception.Message
                };

                context.Result = new NotFoundObjectResult(details);
                context.ExceptionHandled = true;
            }
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments)
            {
                if (argument.Value == null)
                {
                    continue;
                }

                var argumentType = argument.Value.GetType();
                var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);

                if (serviceProvider.GetService(validatorType) is IValidator validator)
                {
                    var validationContext = new ValidationContext<object>(argument.Value);
                    var validationResult = await validator.ValidateAsync(validationContext);

                    if (!validationResult.IsValid)
                    {
                        var errors = validationResult.Errors
                            .GroupBy(x => x.PropertyName)
                            .ToDictionary(
                                g => g.Key,
                                g => g.Select(x => x.ErrorMessage).ToArray());

                        context.Result = new BadRequestObjectResult(
                            new ValidationProblemDetails
                            {
                                Errors = errors,
                                Title = "Validation Failed",
                                Detail = "One or more validation errors occurred.",
                                Status = 400
                            });

                        return;
                    }
                }
            }

            await next();
        }
    }
}
 