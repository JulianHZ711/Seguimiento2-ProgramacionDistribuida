using Access.Application.Exceptions;
using Access.Application.Utilities.Results;
using Access.Domain.Exceptions;

namespace Access.Application.UseCases.Common
{
    public abstract class BaseHandler
    {
        protected async Task<Result<T>> ExecuteAsync<T>(Func<Task<T>> action)
        {
            try
            {
                T? value = await action();
                return Result<T>.Success(value);
            }
            catch (CustomValidationException ex)
            {
                IEnumerable<Error> errors = ex.Errors.Select(error => new Error(Code: $"Validation.{error.PropertyName}",
                                                                                Description: error.ErrorMessage,
                                                                                Type: ErrorType.Validation));

                return Result<T>.Failure(errors);
            }
            catch (BussinesRuleException ex)
            {
                return Result<T>.Failure(new Error(Code: "BusinessRule.Violation",
                                                   Description: ex.Message,
                                                   Type: ErrorType.Business));
            }
            catch (NotFoundException ex)
            {
                return Result<T>.Failure(new Error(Code: "BusinessRule.NotFound",
                                                   Description: ex.Message,
                                                   Type: ErrorType.NotFound));
            }

            // TODO: Implement logging for unexpected errors
            catch (Exception ex)
            {
                return Result<T>.Failure(new Error(Code: "Unexpected",
                                                   Description: "Unexpected error occurred",
                                                   Type: ErrorType.Unexpected));
            }
        }

        protected async Task<Result> ExecuteAsync(Func<Task> action)
        {
            try
            {
                await action();
                return Result.Success();
            }
            catch (CustomValidationException ex)
            {
                IEnumerable<Error> errors = ex.Errors.Select(error => new Error(Code: $"Validation.{error.PropertyName}",
                                                                                Description: error.ErrorMessage,
                                                                                Type: ErrorType.Validation));

                return Result.Failure(errors);
            }
            catch (BussinesRuleException ex)
            {
                return Result.Failure(new Error(Code: "BusinessRule.Violation",
                                                   Description: ex.Message,
                                                   Type: ErrorType.Business));
            }
            catch (NotFoundException ex)
            {
                return Result.Failure(new Error(Code: "BusinessRule.NotFound",
                                                   Description: ex.Message,
                                                   Type: ErrorType.NotFound));
            }

            // TODO: Implement logging for unexpected errors
            catch (Exception ex)
            {
                return Result.Failure(new Error(Code: "Unexpected",
                                                   Description: "Unexpected error occurred",
                                                   Type: ErrorType.Unexpected));
            }
        }
    }
}
