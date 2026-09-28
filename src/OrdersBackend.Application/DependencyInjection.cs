using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OrdersBackend.Application.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrdersBackend.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
            services.AddSingleton<TimeProvider>(TimeProvider.System);
            return services;
        }
    }
}
