
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MIDASM.Application.Services.Crypto;

namespace MIDASM.Infrastructure.Crypto;

public static class ConfigureCryptoService
{
    public static IServiceCollection AddCryptoService<TCrypto, TOptions>(this IServiceCollection services,
        string key)
        where TCrypto : class, ICryptoService
        where TOptions : class
    {
        IConfiguration config;
        using (var serviceProvider = services.BuildServiceProvider())
        {
            config = serviceProvider.GetRequiredService<IConfiguration>();
        }
        services.Configure<TOptions>(config.GetRequiredSection($"EncryptionOptions:{key}"));
        return services.AddKeyedScoped<ICryptoService, TCrypto>(key);
    }
}