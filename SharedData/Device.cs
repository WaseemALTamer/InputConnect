using Avalonia.Controls;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputConnect.SharedData
{
    public static class Device
    {

        public static string? DeviceName = Environment.UserName; // this will get the device name from the OS
                                                                 // you can also get it through the Device.cs
                                                                 // inside the Netowrk namespace

        public static IReadOnlyList<Screen>? Screens; // this will be grabed by the MainWindow and then will
                                                      // be shared across
                                                       

    }
}
