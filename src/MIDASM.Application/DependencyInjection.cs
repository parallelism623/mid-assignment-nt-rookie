
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using MIDASM.Application.UseCases.Implements;
using MIDASM.Application.UseCases.Interfaces;

namespace MIDASM.Application;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureApplicationLayer(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddScoped<Dispatcher.Dispatcher>(serviceProvide =>
            new Dispatcher.Dispatcher(serviceProvide)
        );
        return services.ConfigureFluentValidation().ConfigureServices();
    }
    public static IServiceCollection ConfigureFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));
        services.AddFluentValidationAutoValidation(options =>
        {
            options.DisableDataAnnotationsValidation = true;
        });
        return services;
    }
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<ICategoryServices, CategoryServices>()
            .AddScoped<IBookServices, BookServices>()
            .AddScoped<IUserServices, UserServices>()
            .AddScoped<IBookBorrowingRequestServices, BookBorrowingRequestServices>()
            .AddScoped<IBookBorrowingRequestDetailServices, BookBorrowingRequestDetailServices>()
            .AddScoped<IBookReviewServices, BookReviewServices>()
            .AddScoped<IRoleServices, RoleServices>()
            .AddScoped<IReportServices, ReportServices>();
        return services;
    }

}
