using System;
using System.Collections.Generic;
using System.Text;
using ExpenseClaimSystem.Application.Interfaces;
using ExpenseClaimSystem.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseClaimSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
