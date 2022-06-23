using System.Text.Json;
using kontaktica.Endpoints;
using Serilog;

namespace kontaktica;

public class Program
{
    public static void Main(string[] args)
    {
        using var consoleLogger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        Log.Logger = consoleLogger; // Set to global logger

        try
        {
            InitializeDirectories();

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var app = builder.Build();

            app.MapCikloWeb();

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Something went wrong");
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
