
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MIDASM.Domain.Repositories;
using MIDASM.Persistence.Interceptors;
using MIDASM.Persistence.Repositories;

namespace MIDASM.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection ConfigurePersistenceLayer(this IServiceCollection services)
    {
        var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

        return services.ConfigureApplicationDbContext(config)
            .ConfigureRepositories();
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
                .AddScoped<IEmailRecordRepository, EmailRecordRepository>()
                .AddScoped<IAuditLoggerRepository, AuditLoggerRepository>()
                .AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<ApplicationDbContext>());
        return services;
    }

}
