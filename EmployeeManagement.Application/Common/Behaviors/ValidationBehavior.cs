using FluentValidation; 

namespace EmployeeManagement.Application.Common.Behaviors;

// Generic validation behavior that runs before each MediatR request handler.
// TRequest:  the command/query type being handled.
// TResponse: the type returned by the handler.
// The primary constructor accepts all validators that apply to this TRequest.
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : notnull                
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators; 

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any()) 
        {
            var context = new ValidationContext<TRequest>(request); 

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))); 

            var failures = validationResults
                .SelectMany(r => r.Errors) 
                .Where(f => f != null)     
                .ToList();                 

            if (failures.Count != 0)
                throw new ValidationException(failures); 
        }

        return await next(cancellationToken); 
    }
}
