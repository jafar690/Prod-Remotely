using System;
using System.IO;
using System.Text.Json;
using Silgred.ScreenCast.Core.Services;

namespace Silgred.Win.Services
{
    public class Config
    {
        public string Host { get; set; } = "https://mspserver.azurewebsites.net";
        public string Theme { get; set; } = "Default";
        public string Name { get; set; } = "Host";

        private static string ConfigFile =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Silgred", "Config.json");

        private static string ConfigFolder =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Silgred");


        public static Config GetConfig()
        {
            if (!Directory.Exists(ConfigFolder))
                return new Config();

            if (!File.Exists(ConfigFile))
                return new Config();
            try
            {
                return JsonSerializer.Deserialize<Config>(File.ReadAllText(ConfigFile));
            }
            catch
            {
                return new Config();
            }
        }

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(ConfigFolder);
                File.WriteAllText(ConfigFile, JsonSerializer.Serialize(this));
            }
            catch (Exception exception)
            {
                Logger.Write(exception);
            }
        }
    }
}