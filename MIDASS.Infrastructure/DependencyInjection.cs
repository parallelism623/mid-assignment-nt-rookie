using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MIDASS.Application.Services.Authentication;
using MIDASS.Application.Services.Crypto;
using MIDASS.Application.Services.HostedServices.Abstract;
using MIDASS.Application.Services.Mail;
using MIDASS.Infrastructure.Authentication;
using MIDASS.Infrastructure.Crypto;
using MIDASS.Infrastructure.HostedServices.Abstract;
using MIDASS.Infrastructure.HostedServices.MailSenderBackgroundService;
using MIDASS.Infrastructure.Mail;
using MIDASS.Infrastructure.Options;

namespace MIDASS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services)
    {
        services.AddCryptoService<RsaCryptoService, RsaCryptoOptions>("RSA");
        services.AddScoped<IExecutionContext, ApplicationExecutionContext>();
        services.AddScoped<IJwtTokenServices, JwtTokenServices>();
        services.AddScoped<ICryptoServiceFactory, CryptoServiceFactory>();
        services.AddScoped<IApplicationAuthentication, ApplicationAuthentication>();
        var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        services.Configure<EmailSettingsOptions>(config.GetRequiredSection("EmailSettingsOptions"));
        services.AddTransient<IMailServices, MailServices>();
        services.AddHostedService<MailSenderBackgroundService>();
        services.AddSingleton(typeof(IBackgroundTaskQueue<>), typeof(BackgroundTaskQueue<>));
        return services;
    }
}
