namespace Utility.Libraries.Shared;

public record Credentials(string Identifier)
{
    private const string UrlKey = "Url";
    private const string UsernameKey = "Username";
    private const string PasswordKey = "Password";
    private static readonly EnvironmentVariableTarget Target = EnvironmentVariableTarget.User;

    public string Url => GetVariable(UrlKey);
    public string Username => GetVariable(UsernameKey);
    public string Password => GetVariable(PasswordKey);

    public void Set(string url, string username, string password)
    {
        SetVariable(UrlKey, url);
        SetVariable(UsernameKey, username);
        SetVariable(PasswordKey, password);
    }
    
    private void SetVariable(string key, string value) => Environment.SetEnvironmentVariable($"{Identifier}:{key}", value, Target);
    private string GetVariable(string key) => Environment.GetEnvironmentVariable($"{Identifier}:{key}", Target) ?? string.Empty;
}