using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens.Experimental;
using TechAssessment.Application.Common.Interfaces;

namespace TechAssessment.Application.Common.Models;

public class Result<T> : IResult
{
    //public Result(bool succeeded, IEnumerable<string> errors)
    //{
    //    Succeeded = succeeded;
    //    Errors = errors.ToArray();
    //}

    //public bool Succeeded { get; init; }

    //public string[] Errors { get; init; }

    //public static Result Success()
    //{
    //    return new Result(true, Array.Empty<string>());
    //}

    //public static Result Failure(IEnumerable<string> errors)
    //{
    //    return new Result(false, errors);
    //}


    private ObservableCollection<string> _errors = new ObservableCollection<string>();

    private ObservableCollection<ValidationError> _validationErrors = new ObservableCollection<ValidationError>();

    [JsonInclude]
    public T? Value { get; init; }

    [JsonIgnore]
    public Type ValueType => typeof(T);

    private ResultStatus InitialStatus { get; set; }

    [JsonInclude]
    public ResultStatus Status { get; protected set; }

    public bool IsSuccess => Status == ResultStatus.Ok;

    [JsonInclude]
    public string SuccessMessage { get; protected set; } = string.Empty;

    [JsonInclude]
    public string CorrelationId { get; protected set; } = string.Empty;

    [JsonInclude]
    public ObservableCollection<string> Errors
    {
        get
        {
            return _errors;
        }
        set
        {
            _errors = value;
            Errors_CollectionChanged(_errors, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }

    [JsonInclude]
    public ObservableCollection<ValidationError> ValidationErrors
    {
        get
        {
            return _validationErrors;
        }
        set
        {
            _validationErrors = value;
            ValidationErrors_CollectionChanged(_validationErrors, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }

    protected Result()
    {
        Initialize();
    }

    public Result(T value)
    {
        Value = value;
        Initialize();
    }

    protected internal Result(T value, string successMessage)
        : this(value)
    {
        SuccessMessage = successMessage;
        Initialize();
    }

    protected Result(ResultStatus status)
    {
        Status = status;
        Initialize();
    }

    protected void Initialize()
    {
        InitialStatus = Status;
        ValidationErrors.CollectionChanged += ValidationErrors_CollectionChanged!;
        Errors.CollectionChanged += Errors_CollectionChanged!;
    }

    protected void ValidationErrors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        EvaluateResultStatus();
    }

    protected void Errors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        EvaluateResultStatus();
    }

    private void EvaluateResultStatus()
    {
        if (ShouldParseStatusBasedOnResultStatus())
        {
            if (Errors.Count > 0)
            {
                Status = ResultStatus.Error;
            }
            else if (ValidationErrors.Count > 0)
            {
                Status = ResultStatus.Invalid;
            }
            else
            {
                Status = InitialStatus;
            }
        }
    }

    private bool ShouldParseStatusBasedOnResultStatus()
    {
        if (Status != ResultStatus.Ok && Status != ResultStatus.Error)
        {
            return Status == ResultStatus.Invalid;
        }

        return true;
    }

    public static implicit operator T(Result<T> result)
    {
        return result.Value!;
    }

    public static implicit operator Result<T>(T value)
    {
        return new Result<T>(value);
    }

    public static implicit operator Result<T>(Result result)
    {
        return new Result<T>(default(T)!)
        {
            Status = result.Status,
            InitialStatus = result.InitialStatus,
            Errors = result.Errors,
            SuccessMessage = result.SuccessMessage,
            CorrelationId = result.CorrelationId,
            ValidationErrors = result.ValidationErrors
        };
    }

    public object GetValue()
    {
        return Value!;
    }

    //public PagedResult<T> ToPagedResult(PagedInfo pagedInfo)
    //{
    //    return new PagedResult<T>(pagedInfo, Value)
    //    {
    //        Status = Status,
    //        InitialStatus = InitialStatus,
    //        SuccessMessage = SuccessMessage,
    //        CorrelationId = CorrelationId,
    //        Errors = Errors,
    //        ValidationErrors = ValidationErrors
    //    };
    //}

    public static Result<T> Success(T value)
    {
        return new Result<T>(value);
    }

    public static Result<T> Success(T value, string successMessage)
    {
        return new Result<T>(value, successMessage);
    }

    public static Result<T> Error(params string[] errorMessages)
    {
        Result<T> result = new Result<T>(ResultStatus.Error);
        if (errorMessages != null && errorMessages.Length != 0)
        {
            result.Errors = new ObservableCollection<string>(errorMessages);
        }

        return result;
    }

    public static Result<T> Invalid(ValidationError validationError)
    {
        return new Result<T>(ResultStatus.Invalid)
        {
            ValidationErrors = { validationError }
        };
    }

    public static Result<T> Invalid(params ValidationError[] validationErrors)
    {
        Result<T> result = new Result<T>(ResultStatus.Invalid);
        if (validationErrors != null && validationErrors.Length != 0)
        {
            result.ValidationErrors = new ObservableCollection<ValidationError>(validationErrors);
        }

        return result;
    }

    public static Result<T> Invalid(List<ValidationError> validationErrors)
    {
        return Invalid(validationErrors.ToArray());
    }

    public static Result<T> NotFound()
    {
        return new Result<T>(ResultStatus.NotFound);
    }

    public static Result<T> NotFound(params string[] errorMessages)
    {
        Result<T> result = new Result<T>(ResultStatus.NotFound);
        if (errorMessages != null || errorMessages!.Length != 0)
        {
            result.Errors = new ObservableCollection<string>(errorMessages);
        }

        return result;
    }

    public static Result<T> Forbidden()
    {
        return new Result<T>(ResultStatus.Forbidden);
    }

    public static Result<T> Unauthorized()
    {
        return new Result<T>(ResultStatus.Unauthorized);
    }

    public static Result<T> Conflict()
    {
        return new Result<T>(ResultStatus.Conflict);
    }

    public static Result<T> Conflict(params string[] errorMessages)
    {
        Result<T> result = new Result<T>(ResultStatus.Conflict);
        if (errorMessages != null || errorMessages?.Length != 0)
        {
            result.Errors = new ObservableCollection<string>(errorMessages!);
        }

        return result;
    }

    public static Result<T> CriticalError(params string[] errorMessages)
    {
        Result<T> result = new Result<T>(ResultStatus.CriticalError);
        if (errorMessages != null || errorMessages?.Length != 0)
        {
            result.Errors = new ObservableCollection<string>(errorMessages!);
        }

        return result;
    }

    public static Result<T> Unavailable(params string[] errorMessages)
    {
        Result<T> result = new Result<T>(ResultStatus.Unavailable);
        if (errorMessages != null || errorMessages?.Length != 0)
        {
            result.Errors = new ObservableCollection<string>(errorMessages!);
        }

        return result;
    }

}



public class Result : Result<Result>
{
    public Result()
    {
    }

    protected internal Result(ResultStatus status)
        : base(status)
    {
    }

    public static Result Success()
    {
        return new Result();
    }

    public static Result SuccessWithMessage(string successMessage)
    {
        return new Result
        {
            SuccessMessage = successMessage
        };
    }

    public static Result<T> Success<T>(T value)
    {
        return new Result<T>(value);
    }

    public static Result<T> Success<T>(T value, string successMessage)
    {
        return new Result<T>(value, successMessage);
    }

    public new static Result Error(params string[] errorMessages)
    {
        Result result = new Result(ResultStatus.Error);
        if (errorMessages != null && errorMessages.Length != 0)
        {
            result.Errors = new ObservableCollection<string>(errorMessages);
        }

        result.Initialize();
        return result;
    }

    public static Result ErrorWithCorrelationId(string correlationId, params string[] errorMessages)
    {
        Result result = new Result(ResultStatus.Error);
        result.CorrelationId = correlationId;
        if (errorMessages != null && errorMessages.Length != 0)
        {
            result.Errors = new ObservableCollection<string>(errorMessages);
        }

        result.Initialize();
        return result;
    }

    public new static Result Invalid(ValidationError validationError)
    {
        return Invalid(new List<ValidationError> { validationError });
    }

    public new static Result Invalid(params ValidationError[] validationErrors)
    {
        Result result = new Result(ResultStatus.Invalid);
        if (validationErrors != null && validationErrors.Length != 0)
        {
            result.ValidationErrors = new ObservableCollection<ValidationError>(validationErrors);
        }

        result.Initialize();
        return result;
    }

    public new static Result Invalid(List<ValidationError> validationErrors)
    {
        return Invalid(validationErrors.ToArray());
    }

    public new static Result NotFound()
    {
        return NotFound((string[])null!);
    }

    public new static Result NotFound(params string[] errorMessages)
    {
        Result result = new Result(ResultStatus.NotFound);
        if (errorMessages != null && errorMessages.Length != 0)
        {
            result.Errors = new ObservableCollection<string>(errorMessages);
        }

        result.Initialize();
        return result;
    }

    public new static Result Forbidden()
    {
        Result result = new Result(ResultStatus.Forbidden);
        result.Initialize();
        return result;
    }

    public new static Result Unauthorized()
    {
        Result result = new Result(ResultStatus.Unauthorized);
        result.Initialize();
        return result;
    }

    public new static Result Conflict()
    {
        return Conflict((string[])null!);
    }

    public new static Result Conflict(params string[] errorMessages)
    {
        Result result = new Result(ResultStatus.Conflict);
        if (errorMessages != null && errorMessages.Length != 0)
        {
            result.Errors = new ObservableCollection<string>(errorMessages);
        }

        result.Initialize();
        return result;
    }

    public new static Result Unavailable(params string[] errorMessages)
    {
        Result result = new Result(ResultStatus.Unavailable);
        if (errorMessages != null && errorMessages.Length != 0)
        {
            result.Errors = new ObservableCollection<string>(errorMessages);
        }

        result.Initialize();
        return result;
    }

    public new static Result CriticalError(params string[] errorMessages)
    {
        Result result = new Result(ResultStatus.CriticalError);
        if (errorMessages != null && errorMessages.Length != 0)
        {
            result.Errors = new ObservableCollection<string>(errorMessages);
        }

        result.Initialize();
        return result;
    }
}
