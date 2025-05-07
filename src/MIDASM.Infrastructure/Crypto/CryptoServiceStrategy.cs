
using Microsoft.Extensions.DependencyInjection;
using MIDASM.Application.Services.Crypto;
using MIDASM.Contract.Helpers;
using MIDASM.Contract.Messages.ExceptionMessages;
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
            var exMessage = StringHelper.ReplacePlaceholders(ApplicationExceptionMessages.NoSupportCryptoAlgorithmType, 
                                                                algorithm);
            throw new BadRequestException(exMessage);
        }
    }
}