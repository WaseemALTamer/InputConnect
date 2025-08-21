using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputConnect.SharedData
{
    public static class Events
    {
        public static Action? OnConnection; // estaplished connection gets made with a connected status
        public static Action? OnDisconnect;



        public static Action? TargetDeviceConnectionChanged; // this triggeres when the connections is closed for the TargetDevice

    }
}
