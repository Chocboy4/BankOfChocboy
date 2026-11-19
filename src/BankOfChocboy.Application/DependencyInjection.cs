using Microsoft.Extensions.DependencyInjection;

namespace BankOfChocboy.Application;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IBankingService, BankingService>();
        services.AddScoped<AccountNumberGenerator>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IAirtimeService, AirtimeService>();
        services.AddScoped<IDataPurchaseService, DataPurchaseService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IAuditService, AuditService>();

        return services;
    }
}
