using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MIDASM.Application.Services.Authentication;
using MIDASM.Application.Services.Crypto;
using MIDASM.Application.Services.FileServices;
using MIDASM.Application.Services.HostedServices.Abstract;
using MIDASM.Application.Services.Mail;
using MIDASM.Infrastructure.Authentication;
using MIDASM.Infrastructure.Crypto;
using MIDASM.Infrastructure.Files;
using MIDASM.Infrastructure.HostedServices.Abstract;
using MIDASM.Infrastructure.HostedServices.MailSenderBackgroundService;
using MIDASM.Infrastructure.Mail;
using MIDASM.Infrastructure.Options;

namespace MIDASM.Infrastructure;

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
