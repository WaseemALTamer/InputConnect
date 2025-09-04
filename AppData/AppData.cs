using InputConnect.Structures;
using System.Reflection;
using System.Text.Json;
using System.IO;
using System;
using System.Collections.Generic;
using InputConnect.SettingStruct;




namespace InputConnect
{
    public static class AppData
    {
        public static string ConfigPath = $"AppData/Config.json";
        public static string ConnectionPath = $"AppData/Connections.json";
        public static string ThemePath = $"AppData/Theme.json";




        public static void SaveConfig(){
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(Setting.Config, options);
            File.WriteAllText(ConfigPath, json);
        }

        public static void LoadConfig(){
            if (!File.Exists(ConfigPath)){
                SaveConfig();
                return;
            }

            try{
                string json = File.ReadAllText(ConfigPath);
                Setting.Config = JsonSerializer.Deserialize<ConfigStruct>(json)!;
            }
            catch{
                SaveConfig();
            }
        }


        // for the saving the theme it doesnt work this is because you cant turn the avalonia
        // object to pure json and then convert it back to its  object so  you  have to write
        // a function which takes the theme rgb values of everything  and just pushes them in
        // by creating the object kina manually for the saved data
        public static void SaveTheme(){

            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(Setting.Themes, options);
            File.WriteAllText(ThemePath, json);
        }

        public static void LoadTheme(){
            if (!File.Exists(ThemePath)){
                SaveConfig();
                return;
            }

            try{
                string json = File.ReadAllText(ThemePath);
                Setting.Themes = JsonSerializer.Deserialize<ThemesStruct>(json)!;
            }
            catch
            {
                SaveConfig();
            }
        }






        public static void SaveConnections(){
            for (int i = Connections.Devices.ConnectionList.Count - 1; i >= 0; i--){
                if (Connections.Devices.ConnectionList[i].State != Connections.Constants.StateConnected) {
                    Connections.Devices.ConnectionList.RemoveAt(i); // this will remove all the connections that are pending
                }
            }

            // this function will simply just save the Connections file to disk
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(Connections.Devices.ConnectionList, options);
            File.WriteAllText(ConnectionPath, json);
        }



        public static void LoadConnections() {
            if (!File.Exists(ConnectionPath)){
                Console.WriteLine($"Error saved Connections cant open");
                SaveConnections();
                return;
            }

            try{
                string json = File.ReadAllText(ConnectionPath);
                Connections.Devices.ConnectionList = JsonSerializer.Deserialize<List<Connection>>(json)!;
                for (int i = Connections.Devices.ConnectionList.Count - 1; i >= 0; i--){
                    if (Connections.Devices.ConnectionList[i].State != Connections.Constants.StateConnected){
                        Connections.Devices.ConnectionList.RemoveAt(i); // this will remove all the connections that are pending
                    }
                }
            }
            catch (Exception ex){
                Console.WriteLine("Failed to load connections: " + ex);
            }


        }


    }
}
