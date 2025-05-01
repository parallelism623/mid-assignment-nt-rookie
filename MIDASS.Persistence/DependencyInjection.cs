
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MIDASS.Application.Services.Authentication;
using MIDASS.Application.UseCases;
using MIDASS.Domain.Abstract;
using MIDASS.Domain.Repositories;
using MIDASS.Persistence.Interceptors;
using MIDASS.Persistence.Repositories;
using MIDASS.Persistence.Services;

namespace MIDASS.Persistence;

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
        return services;
    }

    public static IServiceCollection ConfigureApplicationDbContext(this IServiceCollection services,
        IConfiguration config)
    {
        var executionContext = services.BuildServiceProvider().GetRequiredService<IExecutionContext>();
        return services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(config.GetConnectionString("Default"));
            options.AddInterceptors(new AuditableEntitiesInterceptor(executionContext));
        });
    }
}
