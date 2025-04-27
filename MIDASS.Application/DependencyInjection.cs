
using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MIDASS.Contract;
using MIDASS.Application.Commons.Options;

namespace MIDASS.Application;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureApplicationLayer(this IServiceCollection services)
    {
        var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        services.Configure<JwtTokenOptions>(config.GetRequiredSection(nameof(JwtTokenOptions)));
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
