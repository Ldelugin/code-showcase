using CommandLine;
using -- redacted --;
using -- redacted --;
using -- redacted --;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OptionsSeeder;

await Parser.Default.ParseArguments<Options>(args)
    .WithParsedAsync(async options =>
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostBuilderContext, builder) =>
                -- redacted --
            .ConfigureLogging(builder => builder.AddConsole())
            .ConfigureServices((hostContext, services) =>
            {
                _ = services.ConfigureDatabaseConnection(hostContext.Configuration)
                    .-- redacted --
                    .AddSingleton<IOptions>(options)
                    .AddHostedService<SeedOptionsWorker>();
            })
            .Build();

        await host.RunAsync();
    });