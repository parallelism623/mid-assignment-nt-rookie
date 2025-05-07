
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MIDASM.Application.Services.AuditLogServices;
using MIDASM.Application.UseCases;
using MIDASM.Domain.Abstract;
using MIDASM.Domain.Repositories;
using MIDASM.Persistence.Interceptors;
using MIDASM.Persistence.Repositories;
using MIDASM.Persistence.UseCases;

namespace MIDASM.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection ConfigurePersistenceLayer(this IServiceCollection services)
    {
        var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

        return services.ConfigureApplicationDbContext(config)
            .ConfigureRepositories()
            .ConfigureServices();
    }

    public static IServiceCollection ConfigureApplicationDbContext(this IServiceCollection services,
        IConfiguration config)
    {
        services.AddScoped<AuditableEntitiesInterceptor>();
        services.AddDbContext<AuditLogDbContext>(options =>
        {
            options.UseSqlServer(config.GetConnectionString("AuditLog"));
        });
        return services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var interceptorAuditableEntity = sp.GetRequiredService<AuditableEntitiesInterceptor>();
            options.UseSqlServer(config.GetConnectionString("Default"));
            options.AddInterceptors(interceptorAuditableEntity);
        });
    }

    public static IServiceCollection ConfigureRepositories(this IServiceCollection services)
    {

        services.AddScoped<ICategoryRepository, CategoryRepository>()
                .AddScoped<IBookRepository, BookRepository>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IBookBorrowingRequestRepository, BookBorrowingRequestRepository>()
                .AddScoped<IBookBorrowingRequestRepository, BookBorrowingRequestRepository>()
                .AddScoped<IRoleRepository, RoleRepository>()
                .AddScoped<IBookBorrowingRequestDetailRepository, BookBorrowingRequestDetailRepository>()
                .AddScoped<IBookReviewRepository, BookReviewRepository>()
                .AddScoped<IEmailRecordRepository, EmailRecordRepository>();
        return services;
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<ICategoryServices, CategoryServices>()
                .AddScoped<IBookServices, BookServices>()
                .AddScoped<IUserServices, UserServices>()
                .AddScoped<ITransactionManager, TransactionManager>()
                .AddScoped<IBookBorrowingRequestServices, BookBorrowingRequestServices>()
                .AddScoped<IBookBorrowingRequestDetailServices, BookBorrowingRequestDetailServices>()
                .AddScoped<IBookReviewServices, BookReviewServices>()
                .AddScoped<IRoleServices, RoleServices>()
                .AddScoped<IAuditLogger, AuditLogger>()
                .AddScoped<IReportServices, ReportServices>();
        return services;
    }
}
