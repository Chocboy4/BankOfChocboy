using BankOfChocboy.Infrastructure.HttpClients;
using BankOfChocboy.Infrastructure.Repositories;
using BankOfChocboy.Infrastructure.Security;
using BankOfChocboy.Infrastructure.Seeding;

namespace BankOfChocboy.Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IAirtimeRepository, AirtimeRepository>();
        services.AddScoped<IAirtimeVendRepository, AirtimeVendRepository>();
        services.AddScoped<IDataPurchaseRepository, DataPurchaseRepository>();
        services.AddScoped<IAdminUserRepository, AdminUserRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IInterbankTransferRepository, InterbankTransferRepository>();
        services.AddScoped<IIncomingTransferRepository, IncomingTransferRepository>();

        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<AdminSeeder>();

        services.AddHttpClient<ITatumBankClient, TatumBankClient>(client =>
        {
            client.BaseAddress = new Uri(
                configuration["TatumBank:BaseUrl"]
                ?? throw new InvalidOperationException("TatumBank BaseUrl is not configured."));
            client.Timeout = TimeSpan.FromSeconds(15);

            var apiKey = configuration["TatumBank:ApiKey"];
            if (!string.IsNullOrWhiteSpace(apiKey))
                client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
        });

        return services;
    }
}
