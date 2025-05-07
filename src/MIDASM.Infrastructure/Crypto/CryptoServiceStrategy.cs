
using Microsoft.Extensions.DependencyInjection;
using MIDASM.Application.Services.Crypto;
using MIDASM.Contract.Helpers;
using MIDASM.Contract.Messages.ExceptionMessages;
using Rookies.Contract.Exceptions;

namespace MIDASM.Infrastructure.Crypto;

public class CryptoServiceFactory(IServiceProvider serviceProvider) : ICryptoServiceFactory
{

    public ICryptoService SetCryptoAlgorithm(string algorithm)
    {
        try
        {
            return serviceProvider.GetRequiredKeyedService<ICryptoService>(algorithm);
        }
        catch
        {
            var exMessage = StringHelper.ReplacePlaceholders(ApplicationExceptionMessages.NoSupportCryptoAlgorithmType, 
                                                                algorithm);
            throw new BadRequestException(exMessage);
        }
    }
}