using System.Net;
using System.ServiceModel;
using Utility.Libraries.ServiceReferences.-- redacted --;
using Utility.Libraries.ServiceReferences.-- redacted --;
using Utility.Libraries.ServiceReferences.-- redacted --;

namespace Utility.Libraries.ServiceReferences;

public sealed class Connector(string server, string username, string password) : IDisposable, IAsyncDisposable
{
    public -- redacted -- { get; private set; }
    public -- redacted -- { get; private set; }
    public -- redacted -- { get; private set; }
    public bool IsConnected => -- redacted --?.State == CommunicationState.Opened ||
                               -- redacted --?.State == CommunicationState.Opened ||
                               -- redacted --?.State == CommunicationState.Opened;
    
    public string Server { get; private set; } = server;
    public string Username { get; private set; } = username;
    public string Password { get; private set; } = password;
    public string -- redacted -- => $"{Server}/-- redacted --";
    public string -- redacted -- => $"{Server}/-- redacted --";
    public string -- redacted -- => $"{Server}/-- redacted --";

    public async Task ConnectAsync(bool checkCredentials = false)
    {
        if (IsConnected)
        {
            return;
        }
        
        ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
        -- redacted -- = new -- redacted --(Constants.WsHttpBinding, new EndpointAddress(-- redacted --));
        -- redacted --.AddCredentials(username, password);
        -- redacted -- = new -- redacted --(Constants.WsHttpBinding, new EndpointAddress(-- redacted --));
        -- redacted --.AddCredentials(username, password);
        -- redacted -- = new -- redacted --(Constants.WsHttpBinding, new EndpointAddress(-- redacted --));
        -- redacted --.AddCredentials(username, password);

        if (checkCredentials)
        {
            await TestConnectionAsync();
        }
    }

    public async Task DisconnectAsync()
    {
        if (!this.IsConnected)
        {
            return;
        }

        if (this.-- redacted --?.State == CommunicationState.Opened)
        {
            await this.-- redacted --.CloseAsync();
        }
        
        if (this.-- redacted --?.State == CommunicationState.Opened)
        {
            await this.-- redacted --.CloseAsync();
        }
        
        if (this.-- redacted --?.State == CommunicationState.Opened)
        {
            await this.-- redacted --.CloseAsync();
        }
    }

    public void Dispose()
    {
        if (this.IsConnected)
        {
            this.DisconnectAsync().GetAwaiter().GetResult();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (this.IsConnected)
        {
            await this.DisconnectAsync();
        }
    }
    
    private async Task TestConnectionAsync() => _ = await -- redacted --?.GetVersionAsync()!;
}