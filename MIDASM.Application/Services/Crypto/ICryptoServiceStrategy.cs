namespace MIDASM.Application.Services.Crypto;

public interface ICryptoServiceFactory
{
    ICryptoService SetCryptoAlgorithm(string algorithm);

}