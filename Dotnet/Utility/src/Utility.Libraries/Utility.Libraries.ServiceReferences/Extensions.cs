using System.ServiceModel;
using Utility.Libraries.Shared;

namespace Utility.Libraries.ServiceReferences;

public static class Extensions
{
    public static ClientBase<TChannel> AddCredentials<TChannel>(this ClientBase<TChannel> client, string username, string password)
        where TChannel : class
    {
        client.ClientCredentials.UserName.UserName = username;
        client.ClientCredentials.UserName.Password = password;
        client.ClientCredentials.ServiceCertificate.SslCertificateAuthentication = new System.ServiceModel.Security.X509ServiceCertificateAuthentication()
        {
            CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None,
            RevocationMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck
        };
        return client;
    }
    
    public static Connector CreateConnector(this Credentials credentials) =>
        new(credentials.Url, credentials.Username, credentials.Password);
}