using FluentValidation;
using FluentValidation.Results;
using Access.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Access.Application.Utilities.Mediator
{
    public class SimpleMediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;

        public SimpleMediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
        {
            await ValidateRequestAsync(request).ConfigureAwait(false);

            Type useCaseType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));

            var useCase = _serviceProvider.GetService(useCaseType);

            if (useCase is null)
            {
                throw new MediatorException($"No se encontró un handler para {request.GetType().Name}");
            }

            MethodInfo method = useCaseType.GetMethod("Handle")!;

            return await(Task<TResponse>)method.Invoke(useCase, new object[] { request })!;
        }

        public async Task Send(IRequest request)
        {
            await ValidateRequestAsync(request).ConfigureAwait(false);

            Type useCaseType = typeof(IRequestHandler<>).MakeGenericType(request.GetType());

            var useCase = _serviceProvider.GetService(useCaseType);

            if (useCase is null)
            {
                throw new MediatorException($"No se encontró un handler para {request.GetType().Name}");
            }

            MethodInfo method = useCaseType.GetMethod("Handle")!;

            await(Task)method.Invoke(useCase, new object[] { request })!;
        }

        private async Task ValidateRequestAsync(object request)
        {
            Type requestType = request.GetType();
            Type validatorType = typeof(IValidator<>).MakeGenericType(requestType);

            object? validator = _serviceProvider.GetService(validatorType);

            if (validator is null)
            {
                return;
            }

            if (validator is not null)
            {
                MethodInfo validatorMethod = validatorType.GetMethod("ValidateAsync")!;

                Task validationTask = (Task)validatorMethod.Invoke(validator, new object[] { request, default })!;

                await validationTask.ConfigureAwait(false);

                PropertyInfo result = validationTask.GetType().GetProperty("Result")!;
                ValidationResult validationResult = (ValidationResult)result!.GetValue(validationTask)!;

                if (!validationResult.IsValid)
                {
                    throw new CustomValidationException(validationResult.Errors);
                }
            }
        }
    }
}
