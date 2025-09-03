using InputConnect.Structures;
using System.Reflection;
using System.Text.Json;
using System.IO;
using System;
using System.Collections.Generic;




namespace InputConnect
{
    public static class AppData
    {
        public static string ConfigPath = $"AppData/Config.json";
        public static string ConnectionPath = $"AppData/Connections.json";

        




        //public static ConfigStruct LoadConfig(){

        //}




        //public static void ApplyConfig(ConfigStruct? config)
        //{

        //}

        //public static void SaveConfig(ConfigStruct config){

        //}






        public static void SaveConnections(){


            for (int i = Connections.Devices.ConnectionList.Count - 1; i >= 0; i--){
                if (Connections.Devices.ConnectionList[i].State != Connections.Constants.StateConnected) {
                    Connections.Devices.ConnectionList.RemoveAt(i); // this will remove all the connections that are pending
                }
            }

            // this function will simply just save the config file to disk
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(Connections.Devices.ConnectionList, options);
            File.WriteAllText(ConnectionPath, json);
        }



        public static void LoadConnections() {
            if (!File.Exists(ConnectionPath)){
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
            catch{
                SaveConnections();
            }


        }


    }
}
