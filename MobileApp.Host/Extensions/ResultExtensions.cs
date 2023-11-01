namespace MobileApp.Host.Extensions;

public static class ResultExtensions
{
    public static async Task<IActionResult> Process<T>(this Task<Result<T>> resultTask)
    {
        var result = await resultTask;
        switch (result.Code)
        {
            case ResultCode.Ok:
                return new OkObjectResult(result);
            case ResultCode.BadRequest:
                return new BadRequestObjectResult(result);
            case ResultCode.NotFound:
                return new NotFoundObjectResult(result);
            case ResultCode.AccessDenied:
                return new UnauthorizedResult();
            case ResultCode.Forbidden:
                return new ForbidResult();
            case ResultCode.NoContent:
                return new NoContentResult();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}