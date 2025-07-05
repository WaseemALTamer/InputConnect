using InputConnect.Structures;
using InputConnect.Setting;
using System.Reflection;
using System.Text.Json;
using System.IO;
using System;




namespace InputConnect
{
    public static class Appdata
    {
        public static string ConfigPath = $"Appdata/Config.json";
        public static ConfigStruct? Config = LoadConfig();




        public static ConfigStruct LoadConfig()
        {
            ConfigStruct configInstance = new ConfigStruct();

            // If file doesn't exist, create it from current static Config
            if (!File.Exists(ConfigPath)){
                SaveConfig(configInstance);
                return configInstance;
            }

            try{
                string json = File.ReadAllText(ConfigPath);
                configInstance = JsonSerializer.Deserialize<ConfigStruct>(json)!;

                if (configInstance == null)
                    throw new Exception("Deserialized object was null.");

                ApplyConfig(configInstance);
            }
            catch{
                SaveConfig(configInstance);
            }

            return configInstance;
        }

        public static void ApplyConfig(ConfigStruct? config)
        {
            if (config == null) return;

            var configType = typeof(ConfigStruct);
            var staticConfigType = typeof(Config);

            foreach (var property in configType.GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
                var staticField = staticConfigType.GetField(property.Name, BindingFlags.Public | BindingFlags.Static);

                if (staticField != null && staticField.FieldType == property.PropertyType)
                {
                    var value = property.GetValue(config);
                    staticField.SetValue(null, value);
                }
            }
        }

        public static void SaveConfig(ConfigStruct config){
            // this function will simply just save the config file to disk
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(config, options);
            File.WriteAllText(ConfigPath, json);
        }
    }
}
