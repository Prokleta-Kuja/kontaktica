using System.Text.Json;
using kontaktica.Endpoints;
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
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();

        try
        {
            InitializeDirectories();

            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog();
            builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(C.Settings.Origins.ToArray());
                policy.WithHeaders("content-type");
                policy.WithMethods("post");
            }));

            var app = builder.Build();
            app.UseCors();
            app.MapCikloWeb();
            app.MapCikloWebService();
            app.MapIcaWeb();
            app.MapModWeb();

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
