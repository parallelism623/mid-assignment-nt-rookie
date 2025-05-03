
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MIDASM.Application.Services.AuditLogServices;
using MIDASM.Application.Services.Authentication;
using MIDASM.Application.UseCases;
using MIDASM.Domain.Abstract;
using MIDASM.Domain.Repositories;
using MIDASM.Persistence.Interceptors;
using MIDASM.Persistence.Repositories;
using MIDASM.Persistence.Services;

namespace MIDASM.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection ConfigurePersistenceLayer(this IServiceCollection services)
    {
        var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

        services.ConfigureApplicationDbContext(config);
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICategoryServices, CategoryServices>();
        services.AddScoped<IBookServices, BookServices>();
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IUserServices, UserServices>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBookBorrowingRequestRepository, BookBorrowingRequestRepository>();
        services.AddScoped<ITransactionManager, TransactionManager>();
        services.AddScoped<IBookBorrowingRequestServices, BookBorrowingRequestServices>();
        services.AddScoped<IBookBorrowingRequestRepository, BookBorrowingRequestRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IBookBorrowingRequestDetailRepository, BookBorrowingRequestDetailRepository>();
        services.AddScoped<IBookBorrowingRequestDetailServices, BookBorrowingRequestDetailServices>();
        services.AddScoped<IBookReviewRepository, BookReviewRepository>();
        services.AddScoped<IBookReviewServices, BookReviewServices>();
        services.AddScoped<IEmailRecordRepository, EmailRecordRepository>();
        services.AddScoped<IRoleServices, RoleServices>();
        services.AddScoped<IAuditLogger, AuditLogger>();
        return services;
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
            var interceptorAuditablEntity = sp.GetRequiredService<AuditableEntitiesInterceptor>();
            options.UseSqlServer(config.GetConnectionString("Default"));
            options.AddInterceptors(interceptorAuditablEntity);
        });
    }
}
