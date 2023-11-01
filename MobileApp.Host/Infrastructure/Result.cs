namespace MobileApp.Host.Infrastructure;

public class Result
{
    public bool Success => Code == ResultCode.Ok;

    public IEnumerable<ErrorMessage> Errors { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public ResultCode Code { get; set; }

    public static Result Ok()
    {
        return new Result()
        {
            Code = ResultCode.Ok
        };
    }

    public static Result<T> Ok<T>(T value)
    {
        return new Result<T>()
        {
            Value = value,
            Code = ResultCode.Ok
        };
    }

    public static Result BadRequest(string message)
    {
        var result = new Result
        {
            Code = ResultCode.BadRequest,
            Errors = new List<ErrorMessage>()
            {
                new ErrorMessage()
                {
                    Message = message
                }
            }
        };

        return result;
    }

    public static Result BadRequest(IEnumerable<ErrorMessage> messages)
    {
        var result = new Result
        {
            Code = ResultCode.BadRequest,
            Errors = messages
        };

        return result;
    }

    public static Result<T> BadRequest<T>(string message) where T : new()
    {
        var result = new Result<T>
        {
            Code = ResultCode.BadRequest,
            Errors = new List<ErrorMessage>()
            {
                new ErrorMessage()
                {
                    Message = message
                }
            },
            Value = new T()
        };

        return result;
    }

    public static Result<T> BadRequest<T>(IEnumerable<ErrorMessage> messages) where T : new()
    {
        var result = new Result<T>
        {
            Code = ResultCode.BadRequest,
            Errors = messages,
            Value = new T()
        };

        return result;
    }
    public static Result NotFound()
    {
        return new Result()
        {
            Code = ResultCode.NotFound,
            Errors = new List<ErrorMessage>()
            {
                new ErrorMessage()
                {
                    Message = "There is no such object"
                }
            }
        };
    }

    public static Result<T> NotFound<T>(Guid id) where T : new()
    {
        var result = new Result<T>
        {
            Code = ResultCode.NotFound,
            Errors = new List<ErrorMessage>()
            {
                new ErrorMessage()
                {
                    Message = $"There is no object with the id {id}"
                }
            },
            Value = new T()
        };

        return result;
    }

    public static Result<T> NotFound<T>(string message) where T : new()
    {
        var result = new Result<T>
        {
            Code = ResultCode.NotFound,
            Errors = new List<ErrorMessage>()
            {
                new ErrorMessage()
                {
                    Message = $"There is no object {message}"
                }
            },
            Value = new T()
        };

        return result;
    }

    public static Result<T> NotFound<T>() where T : new()
    {
        var result = new Result<T>
        {
            Code = ResultCode.NotFound,
            Errors = new List<ErrorMessage>()
            {
                new ErrorMessage()
                {
                    Message = $"The requested object was not found"
                }
            },
            Value = new T()
        };

        return result;
    }

    public static Result NotFound(Guid id)
    {
        var result = new Result
        {
            Code = ResultCode.NotFound,
            Errors = new List<ErrorMessage>()
            {
                new ErrorMessage()
                {
                    Message = $"There is no object with the id {id}"
                }
            }
        };

        return result;
    }
}


public class Result<T> : Result
{
    public T Value { get; set; }
}

public class ErrorMessage
{
    public string PropertyName { get; set; }
    public string Message { get; set; }
}

public enum ResultCode
{
    Ok,
    BadRequest,
    NotFound,
    AccessDenied,
    Forbidden,
    NoContent
}