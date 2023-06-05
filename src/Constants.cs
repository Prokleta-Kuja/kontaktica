using System.Text.Json;
using System.Text.Json.Serialization;

namespace kontaktica;

public static class C
{
    public static readonly bool IsDebug;
    public static JsonSerializerOptions JsonOpt { get; } = new() { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };
    public static Settings Settings { get; set; } = new();
    static C()
    {
        IsDebug = Environment.GetEnvironmentVariable("DEBUG") == "1";
    }
    public static class Routes
    {
        public const string CikloWeb = "/ciklo-web";
        public const string IcaWeb = "/ica-web";
        public const string ModWeb = "/mod-web";
    }
    public static class Paths
    {
        public static string Config => Path.Combine(Environment.CurrentDirectory, "config");
        public static string ConfigFor(string file) => Path.Combine(Config, file);
    }
}