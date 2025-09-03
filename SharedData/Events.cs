using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputConnect.SharedData
{
    public static class Events{

        // this class is going to be used to pass certain type of interactions
        // for the code to comuncate with each other, having every interaction
        // mapped to one file will make things  more understandable and can be
        // reused on the run

        public static Action? OnConnection; // estaplished connection gets made with a connected status
        public static Action? OnDisconnect;



        public static Action? TargetDeviceConnectionChanged; // this triggeres when the connections is closed for the TargetDevice
                                                             // this is used to update the MointorsGraph and ChannelModeTriToggles

        public static Action? TargetDeviceChannelModeChange;


        public static Action? OnMovingVirtualScreens; // this will be used to tell the ChannelModeTriToggles to update
                                                      // this will also tell  the other  mointors to check if they are
                                                      // in illegal postion


        public static Action? OnSetVirtualScreensPos; // this will only get  triggered when you realease  the  pointer
                                                      // from virtual mointor

    }
}
