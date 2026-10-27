using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Access.Application.Utilities.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Access.Application
{
    public static class ApplicationServicesRegistry
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Mediator
            services.AddScoped<IMediator, SimpleMediator>();

            // Use Cases
            // Cada caso de uso se registra aquí cuando se cree, igual que en Inmoby:
            // services.AddScoped<IRequestHandler<CreateEmployeeCommand, Result<Guid>>, CreateEmployeeUseCase>();

            // Validations
            services.AddValidatorsFromAssembly(typeof(ApplicationServicesRegistry).Assembly);

            return services;
        }
    }
}
