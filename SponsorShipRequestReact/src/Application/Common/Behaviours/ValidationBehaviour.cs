using ValidationException = TechAssessment.Application.Common.Exceptions.ValidationException;

namespace TechAssessment.Application.Common.Behaviours;

public class ValidationBehaviour<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IRequestHandler<TRequest, TResponse> _inner;
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IRequestHandler<TRequest ,TResponse> inner,IEnumerable<IValidator<TRequest>> validators)
    {
        _inner = inner;
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, /*RequestHandlerDelegate<TResponse> next,*/ CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var validationResults = await Task.WhenAll(
                _validators.Select(v =>
                    v.ValidateAsync(new ValidationContext<TRequest>(request), cancellationToken)));

            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Count != 0)
                throw new ValidationException(failures);
        }

        //return await next();
        return await _inner.Handle(request, cancellationToken);
    }
}

/* Backup copy
 * public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var validationResults = await Task.WhenAll(
                _validators.Select(v =>
                    v.ValidateAsync(new ValidationContext<TRequest>(request), cancellationToken)));

            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Count != 0)
                throw new ValidationException(failures);
        }

        return await next();
    }
}

 * */
