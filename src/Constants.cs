using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace kontaktica;

public static class C
{
    public static readonly bool IsDebug;
    public static JsonSerializerOptions JsonOpt { get; } = new() { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };
    public static Settings Settings { get; set; } = new();
    public static readonly TimeZoneInfo TZ;
    public static readonly CultureInfo Locale;
    static C()
    {
        IsDebug = Environment.GetEnvironmentVariable("DEBUG") == "1";

        try { TZ = TimeZoneInfo.FindSystemTimeZoneById(Environment.GetEnvironmentVariable("TZ") ?? "America/Chicago"); }
        catch (Exception) { TZ = TimeZoneInfo.Local; }

        try { Locale = CultureInfo.GetCultureInfo(Environment.GetEnvironmentVariable("LOCALE") ?? "en-US"); }
        catch (Exception) { Locale = CultureInfo.InvariantCulture; }

        CultureInfo.DefaultThreadCurrentCulture = Locale;
        CultureInfo.DefaultThreadCurrentUICulture = Locale;
    }
    public static class Routes
    {
        public const string Root = "/";
        public const string Jobs = "/jobs";
        public const string Healthcheck = "/healthcheck";
        public const string CikloWeb = "/ciklo-web";
        public const string CikloWebService = "/ciklo-web-service";
        public const string IcaWeb = "/ica-web";
        public const string ModWeb = "/mod-web";
    }
    public static class Paths
    {
        public static string Config => Path.Combine(Environment.CurrentDirectory, "config");
        public static string ConfigFor(string file) => Path.Combine(Config, file);
        public static readonly string Hangfire = ConfigFor("jobs.db");
    }
}