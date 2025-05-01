using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MIDASS.Application.Services.Authentication;
using MIDASS.Application.Services.Crypto;
using MIDASS.Application.Services.FileServices;
using MIDASS.Application.Services.HostedServices.Abstract;
using MIDASS.Application.Services.Mail;
using MIDASS.Infrastructure.Authentication;
using MIDASS.Infrastructure.Crypto;
using MIDASS.Infrastructure.Files;
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
        services.Configure<EmailSettingsOptions>(config.GetRequiredSection(nameof(EmailSettingsOptions)));
        services.Configure<AWSS3Options>(config.GetRequiredSection(nameof(AWSS3Options)));
        services.AddTransient<IMailServices, MailServices>();
        services.AddHostedService<MailSenderBackgroundService>();
        services.AddSingleton(typeof(IBackgroundTaskQueue<>), typeof(BackgroundTaskQueue<>));
        services.ConfigureAwsS3Services(config);
        services.AddScoped<IImageStorageServices, ImageStorageServices>();
        return services;
    }

    public static IServiceCollection ConfigureAwsS3Services(this IServiceCollection services, IConfiguration config)
    {
        services.AddDefaultAWSOptions(config.GetAWSOptions());
        services.AddAWSService<IAmazonS3>();
        return services;
    }
}
