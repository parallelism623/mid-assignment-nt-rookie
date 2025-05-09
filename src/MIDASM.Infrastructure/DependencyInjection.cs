using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MIDASM.Application.Commons.Options;
using MIDASM.Application.Services.Authentication;
using MIDASM.Application.Services.Crypto;
using MIDASM.Application.Services.FileServices;
using MIDASM.Application.Services.HostedServices.Abstract;
using MIDASM.Application.Services.ImportExport;
using MIDASM.Application.Services.Mail;
using MIDASM.Application.Services.ScheduleJobs;
using MIDASM.Contract.Enums;
using MIDASM.Infrastructure.Authentication;
using MIDASM.Infrastructure.Crypto;
using MIDASM.Infrastructure.Files;
using MIDASM.Infrastructure.HostedServices.Abstract;
using MIDASM.Infrastructure.HostedServices.MailSenderBackgroundService;
using MIDASM.Infrastructure.ImportExport.Export;
using MIDASM.Infrastructure.ImportExport.Export.Excels;
using MIDASM.Infrastructure.ImportExport.Export.Pdfs;
using MIDASM.Infrastructure.Mail;
using MIDASM.Infrastructure.Options;
using MIDASM.Infrastructure.ScheduleJobs;
using Quartz;
using Quartz.Impl.Matchers;

namespace MIDASM.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services)
    {

        var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

        services.AddCryptoService<RsaCryptoService, RsaCryptoOptions>(nameof(CryptoAlgorithmType.RSA));
        services.AddScoped<IExecutionContext, ApplicationExecutionContext>();
        services.AddScoped<IJwtTokenServices, JwtTokenServices>();
        services.AddScoped<ICryptoServiceFactory, CryptoServiceFactory>();
        services.AddScoped<IApplicationAuthentication, ApplicationAuthentication>();
        services.Configure<EmailSettingsOptions>(config.GetRequiredSection(nameof(EmailSettingsOptions)));
        services.Configure<AWSS3Options>(config.GetRequiredSection(nameof(AWSS3Options)));
        services.AddTransient<IMailServices, MailServices>();
        services.AddHostedService<MailSenderBackgroundService>();
        services.AddSingleton(typeof(IBackgroundTaskQueue<>), typeof(BackgroundTaskQueue<>));
        services.ConfigureAwsS3Services(config);
        services.AddScoped<IImageStorageServices, ImageStorageServices>();
        services.ConfigureImportExportServices();
        services.Configure<JwtTokenOptions>(config.GetRequiredSection(nameof(JwtTokenOptions)));
        services.ConfigureScheduleJobs(config);
        return services;
    }

    public static IServiceCollection ConfigureAwsS3Services(this IServiceCollection services, IConfiguration config)
    {
        services.AddDefaultAWSOptions(config.GetAWSOptions());
        services.AddAWSService<IAmazonS3>();
        return services;
    }
    public static IServiceCollection ConfigureImportExportServices(this IServiceCollection services)
    {
        services.AddKeyedScoped<IExportServices, ExportToExcel>(nameof(ExportToExcel));
        services.AddScoped<IExportFactory, ExportFactory>();
        services.AddKeyedScoped<IExportServices, ExportToPdf>(nameof(ExportToPdf));
        return services;
    }

    public static IServiceCollection ConfigureScheduleJobs(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IScheduleJobServices, ScheduleJobServices>();
        services.Configure<ScheduleJobSettings>(config.GetRequiredSection(nameof(ScheduleJobSettings)));
        services.AddQuartz(q =>
        {
            q.UsePersistentStore(store =>
            {
                store.UseProperties = true;
                store.UseSystemTextJsonSerializer();
                store.UseSqlServer(options =>
                {
                    options.ConnectionString = config.GetRequiredSection("QuartzSettings:SqlStore:ConnectionStrings").Value ?? string.Empty;
                    options.TablePrefix = config.GetRequiredSection("QuartzSettings:SqlStore:TablePrefix").Value ??
                                          string.Empty;
                });
            });
            q.AddJob<SendMailInformDueDateJob>(opts => opts
                .WithIdentity(SendMailInformDueDateJob.Key)
                .StoreDurably()
                .RequestRecovery()
            );
            q.AddJob<ScanBookBorrowingDueDateJob>(opts => opts
                .WithIdentity(ScanBookBorrowingDueDateJob.Key)
                .StoreDurably()
                .RequestRecovery()
            );

            q.AddJobListener<JobHandlingExceptionListener>(
                KeyMatcher<JobKey>.KeyEquals(SendMailInformDueDateJob.Key)
            );
        });
        services.AddQuartzHostedService(opt =>
        {
            opt.WaitForJobsToComplete = true;
        });
        return services;
    }
}
