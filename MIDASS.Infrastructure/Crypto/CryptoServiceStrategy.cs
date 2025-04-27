
using Microsoft.Extensions.DependencyInjection;
using MIDASS.Application.Services.Crypto;
using Rookies.Contract.Exceptions;

namespace MIDASS.Infrastructure.Crypto;

public class CryptoServiceFactory : ICryptoServiceFactory
{
    private readonly IServiceProvider _serviceProvider;
    public CryptoServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public ICryptoService SetCryptoAlgorithm(string algorithm)
    {
        try
        {
            return _serviceProvider.GetRequiredKeyedService<ICryptoService>(algorithm);
        }
        catch
        {
            throw new BadRequestException($"No support {algorithm} crypto algorithm");
        }
    }
}