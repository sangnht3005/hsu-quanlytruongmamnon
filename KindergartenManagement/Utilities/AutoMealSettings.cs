using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace KindergartenManagement.Utilities;

public class AutoMealSettings
{
    public bool Breakfast { get; set; } = true;
    public bool Lunch { get; set; } = true;
    public bool Snack { get; set; } = true;
    public bool Dinner { get; set; } = true;

    [JsonIgnore]
    private static string SettingsPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "KindergartenManagement",
        "meal-settings.json");

    public static AutoMealSettings Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                var settings = JsonSerializer.Deserialize<AutoMealSettings>(json);
                if (settings != null)
                    return settings;
            }
        }
        catch
        {
            // ignore and fallback to defaults
        }

        return new AutoMealSettings();
    }

    public void Save()
    {
        try
        {
            var dir = Path.GetDirectoryName(SettingsPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(SettingsPath, json);
        }
        catch
        {
            // ignore write errors for now
        }
    }

    public static bool IsEnabled(string mealType)
    {
        var settings = Load();
        return mealType?.ToLowerInvariant() switch
        {
            "breakfast" => settings.Breakfast,
            "lunch" => settings.Lunch,
            "snack" => settings.Snack,
            "dinner" => settings.Dinner,
            _ => true
        };
    }
}
