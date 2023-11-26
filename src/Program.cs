using System.Text.Json;
using Hangfire;
using Hangfire.Common;
using Hangfire.Storage.SQLite;
using kontaktica.Jobs;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Serilog.Events;

namespace kontaktica;

public class Program
{
    public static int Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(C.IsDebug ? LogEventLevel.Debug : LogEventLevel.Information)
            .MinimumLevel.Override(nameof(Microsoft), LogEventLevel.Warning)
            .MinimumLevel.Override(nameof(Hangfire), LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
            )
            .CreateLogger();

        InitializeDirectories();

        try
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(C.Settings.Origins.ToArray());
                policy.WithHeaders("content-type");
                policy.WithMethods("post");
            }));
            builder.Host.UseSerilog();
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });
            builder.Services.AddMemoryCache();
            builder.Services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSQLiteStorage(C.Paths.Hangfire, new SQLiteStorageOptions
                {
                    QueuePollInterval = TimeSpan.FromSeconds(15),
                    InvisibilityTimeout = TimeSpan.FromMinutes(30),
                    DistributedLockLifetime = TimeSpan.FromSeconds(30),
                    JobExpirationCheckInterval = TimeSpan.FromHours(1),
                    CountersAggregateInterval = TimeSpan.FromMinutes(5),
                }));

            var captureFilter = GlobalJobFilters.Filters.OfType<JobFilter>().Where(c => c.Instance is CaptureCultureAttribute).FirstOrDefault();
            if (captureFilter != null)
                GlobalJobFilters.Filters.Remove(captureFilter.Instance);
            builder.Services.AddHangfireServer(o =>
            {
                o.ServerName = nameof(kontaktica);
                o.WorkerCount = 1;//Math.Max(2, Environment.ProcessorCount / 2);
            });

            builder.Services.AddControllers();

            var app = builder.Build();
            app.UseForwardedHeaders();
            app.UseCors();
            app.MapControllers();
            app.UseJobDashboard();

            Log.Information("App started");
            app.Run();

            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    static void InitializeDirectories()
    {
        Directory.CreateDirectory(C.Paths.Config);
        var settingsJsonExample = C.Paths.ConfigFor("settings.example.json");
        if (!File.Exists(settingsJsonExample))
            File.WriteAllText(settingsJsonExample, JsonSerializer.Serialize(C.Settings, C.JsonOpt));

        var settingsJson = C.Paths.ConfigFor("settings.json");
        if (!File.Exists(settingsJson))
            throw new FileNotFoundException("Must configure settings.json");

        var settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(settingsJson), C.JsonOpt);
        if (settings == null)
            throw new JsonException("Could not parse settings.json");

        C.Settings = settings;
    }
}
