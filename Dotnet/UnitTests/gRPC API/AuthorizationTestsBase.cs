using System;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using -- redacted --;

namespace -- redacted --;

public abstract class AuthorizationTestsBase
{
    protected static -- redacted --<Startup> WebApplicationFactory { get; set; }
    protected static GrpcChannel Channel { get; set; }
    protected const string UserName = "-- redacted --";
    protected const string Email = "-- redacted --";
    protected const string Password = "-- redacted --";
    protected const string BaseAddress = "https://localhost:5001";
    private const string Credentials = $"{UserName}:{Password}";
    private static readonly string EncodedCredentials = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(Credentials));

    private static CallCredentials CreateCallCredentials() => CallCredentials.FromInterceptor((_, metadata) =>
    {
        if (!string.IsNullOrEmpty(Credentials))
        {
            metadata.Add("Authorization", $"Basic {EncodedCredentials}");
        }

        return Task.CompletedTask;
    });

    private static GrpcChannelOptions CreateGrpcChannelOptions() => new()
    {
        Credentials = ChannelCredentials.Create(new SslCredentials(), CreateCallCredentials()),
        HttpHandler = WebApplicationFactory.Server.CreateHandler()
    };

    protected static GrpcChannel CreateGrpcChannel() => GrpcChannel.ForAddress(BaseAddress, CreateGrpcChannelOptions());
}