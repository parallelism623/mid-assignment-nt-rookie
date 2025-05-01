
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace MIDASM.Contract;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureContractLayer(this IServiceCollection services)
    {
        return services.ConfigureFluentValidation();
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
}
