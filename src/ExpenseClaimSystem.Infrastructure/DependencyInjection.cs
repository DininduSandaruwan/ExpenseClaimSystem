using System;
using System.Collections.Generic;
using System.Text;
using ExpenseClaimSystem.Application.Interfaces;
using ExpenseClaimSystem.Application.Services;
using ExpenseClaimSystem.Infrastructure.Repositories;
using ExpenseClaimSystem.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseClaimSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>(); 
            services.AddScoped<IExpenseClaimRepository, ExpenseClaimRepository>();
            services.AddScoped<IExpenseClaimService, ExpenseClaimService>();

            return services;
        }
    }
}
