using InputConnect.Structures;
using SharpHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InputConnect.Controllers
{

    


    public static class Hook{
        // this file provides a global hook so we can use it accross the code through it is adviced
        // to use the hook with the Controllers files for Mouse and Keyboard



        public static TaskPoolGlobalHook GlobalHook = new TaskPoolGlobalHook();
        public static EventSimulator GlobalEventSimulator = new EventSimulator();

        public static Connection? TargetConnection = null; // this can be used to send data to the TargetedConnection
                                                           // TargetConnection is  updated  by  the mouse  Controller
                                                           // if you want to use the Keyboard with no  mouse then you
                                                           // have to manually specify which Device it is



        public static bool StartHook() {
            new Thread(() => {
                GlobalHook.RunAsync();
            }).Start();


            // start our Controllers Manually
            Mouse.Start(); 
            Keyboard.Start();

            return true;
        }

    }
}
