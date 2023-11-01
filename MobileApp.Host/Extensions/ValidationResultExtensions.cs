using FluentValidation.Results;

namespace MobileApp.Host.Extensions;

public static class ValidationResultExtensions
{
    public static Result<T> ToResult<T>(this ValidationResult validationResult)
    {
        var result = new Result<T>
        {
            Code = ResultCode.BadRequest,
            Errors = new List<ErrorMessage>()
            {
                new ErrorMessage()
                {
                    Message = validationResult.Errors.First().ErrorMessage
                }
            }
        };

        return result;
    }
}