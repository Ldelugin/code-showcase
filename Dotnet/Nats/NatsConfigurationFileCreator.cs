using System.Linq;
using System.Text;
using -- redacted --;

namespace -- redacted --;

/// <summary>
/// The Nats configuration file creator.
/// </summary>
public static class NatsConfigurationFileCreator
{
    public const string LogDebugTemplate = "[LOG_DEBUG]";
    public const string LogTraceTemplate = "[LOG_TRACE]";
    public const string LogFileTemplate = "[LOGFILE]";
    public const string ListenUrlTemplate = "[URL]";
    public const string PortTemplate = "[PORT]";
    public const string MonitorPortTemplate = "[MONITOR_PORT]";
    public const string UserNameTemplate = "[USERNAME]";
    public const string PasswordTemplate = "[PASSWORD]";
    public const string ClusterNameTemplate = "[CLUSTER_NAME]";
    public const string RoutesPortTemplate = "[ROUTES_PORT]";
    public const string AllRoutesTemplate = "[ALL_ROUTES]";
    public const string LogDebugFormat = $"debug = {LogDebugTemplate}";
    public const string LogTraceFormat = $"trace = {LogTraceTemplate}";
    public const string LogFileFormat = $"log_file = {LogFileTemplate}";
    public const string ListenFormat = $"listen = {ListenUrlTemplate}";
    public const string PortFormat = $"port = {PortTemplate}";
    public const string HttpFormat = $"http = localhost:{MonitorPortTemplate}";
    public const string AuthorizationFormat = $@"authorization = {{
	 users = [
	  {{ user = {UserNameTemplate}, password = {PasswordTemplate} }}
	]
}}";
    public const string ClusterFormat = $@"cluster = {{
	 name = {ClusterNameTemplate}
	 listen = 0.0.0.0:{RoutesPortTemplate}
	 routes = [
	   {AllRoutesTemplate}
	]
}}";
    public const string LineEnding = "\n";

    /// <summary>
    /// Create the contents of the configuration file. 
    /// </summary>
    /// <param name="options">Instance of <see cref="NatsOptions"/> to get the options from.</param>
    /// <param name="serverName">The name of the server.</param>
    /// <param name="natsServerOptionsManager">Instance of <see cref="INatsServerOptionsManager"/>.</param>
    /// <returns>A new string containing the contents of the configuration file.</returns>
    public static string Create(NatsOptions options, string serverName, INatsServerOptionsManager natsServerOptionsManager)
    {
        var builder = new StringBuilder(GetTemplate());
        _ = builder.Replace(LogDebugTemplate, options.LogDebug.ToString().ToLower())
            .Replace(LogTraceTemplate, options.LogTrace.ToString().ToLower())
            .Replace(LogFileTemplate, options.LogFile)
            .Replace(ListenUrlTemplate, options.CreateListenUrl())
            .Replace(PortTemplate, options.NatsServerPort.ToString())
            .Replace(MonitorPortTemplate, options.MonitorPort.ToString())
            .Replace(UserNameTemplate, options.NatsServerClientUserName)
            .Replace(PasswordTemplate, options.NatsServerClientPassword);

        var allExternalRoutes = natsServerOptionsManager.GetRegisteredServerNames()
            .Where(server => server != serverName)
            .Select(options.CreateRouteUrl);

        var result = builder.Replace(ClusterNameTemplate, options.ClusterName)
            .Replace(RoutesPortTemplate, options.RoutePort.ToString())
            .Replace(AllRoutesTemplate, string.Join(LineEnding, allExternalRoutes))
            .ToString();

        return result;
    }

    /// <summary>
    /// Get the template of the configuration file.
    /// </summary>
    /// <returns>A new string with the template of the configuration file.</returns>
    public static string GetTemplate()
    {
        return LogDebugFormat + LineEnding +
               LogTraceFormat + LineEnding +
               LogFileFormat + LineEnding +
               ListenFormat + LineEnding +
               PortFormat + LineEnding +
               HttpFormat + LineEnding +
               AuthorizationFormat + LineEnding +
               ClusterFormat + LineEnding;
    }
}