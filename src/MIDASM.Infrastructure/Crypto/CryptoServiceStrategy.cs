
using Microsoft.Extensions.DependencyInjection;
using MIDASM.Application.Services.Crypto;
using Rookies.Contract.Exceptions;

namespace MIDASM.Infrastructure.Crypto;

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