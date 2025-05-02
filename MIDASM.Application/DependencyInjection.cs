
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MIDASM.Application;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureApplicationLayer(this IServiceCollection services)
    {
        var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        
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
