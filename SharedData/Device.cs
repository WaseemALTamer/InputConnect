using System.Collections.Generic;
using Avalonia.Platform;
using Avalonia.Controls;
using System;




namespace InputConnect.SharedData
{
    public static class Device{

        public static string? DeviceName = Environment.UserName; // this will get the device name from the OS
                                                                 // you can also get it through the Device.cs
                                                                 // inside the Netowrk namespace

        public static IReadOnlyList<Screen>? Screens; // this will be grabed by the MainWindow and then will
                                                      // be shared across

        public static TopLevel? TopLevel; // this is going to contain the Toplevel Container for system
                                          // spcific tasks like  clipboard  management  or even screens
                                          // detection
                                                       

    }
}
