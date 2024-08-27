using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AHI.Infrastructure.Exception;
using AHI.Infrastructure.Validation.Abstraction;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace IdpServer.Application.Pipeline
{
    public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEnumerable<FluentValidation.IValidator<TRequest>> _validators;
        public RequestValidationBehavior(IServiceProvider serviceProvider, IEnumerable<FluentValidation.IValidator<TRequest>> validators)
        {
            _serviceProvider = serviceProvider;
            _validators = validators;
        }
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var context = new FluentValidation.ValidationContext(request);

            var failures = _validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                throw new EntityValidationException(failures);
            }

            var dynamicValidator = _serviceProvider.GetService<IDynamicValidator>();
            if (dynamicValidator != null)
                await dynamicValidator.ValidateAsync(context.InstanceToValidate, cancellationToken);

            return await next();
        }
    }
}
