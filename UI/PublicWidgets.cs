using InputConnect.UI.Containers;
using System.Collections.Generic;
using InputConnect.UI.Holders;
using System.Threading.Tasks;

using InputConnect.Setting;
using Avalonia.Controls;
using Avalonia;
using System;
using Metsys.Bson;
using InputConnect.Structures;



namespace InputConnect.UI
{
    public static class PublicWidgets{

        // This will hold all the Big  Widgets togather mostly Canvases
        // This file will also contain data that can be used all around
        // this also keeps track of the history of what was on screen





        // We use a singlton  methode to create the  AdvertisedDevices 
        // instance  then we can  access it  using its public  pointer
        private static AdvertisedDevices? _UIAdvertisedDevices;
        public static AdvertisedDevices? UIAdvertisedDevices{
            get { return _UIAdvertisedDevices; }
            set { _UIAdvertisedDevices = value;}
        }


        private static DeviceConnection? _UIDeviceConnection;
        public static DeviceConnection? UIDeviceConnection{
            get { return _UIDeviceConnection; }
            set { _UIDeviceConnection = value; }
        }


        private static DeviceSetting? _UIDeviceSetting;
        public static DeviceSetting? UIDeviceSetting
        {
            get { return _UIDeviceSetting; }
            set { _UIDeviceSetting = value; }
        }

        private static BackButton? _BackButton;
        public static BackButton? BackButton{
            get { return _BackButton; }
            set { _BackButton = value; }
        }

        private static SettingButton? _SettingButton;
        public static SettingButton? SettingButton{
            get { return _SettingButton; }
            set { _SettingButton = value; }
        }


        private static List<IDisplayable> Holders = new List<IDisplayable>(); // a list of all the holders this is legacy code not used for recent code
        private static List<IDisplayable> HoldersHistory = new List<IDisplayable>(); // this will be used with the back button to go back from where we came from

        private static IDisplayable? DisplayedHolder;

        public static void Initialize(AvaloniaObject master) {
            var Master = master as Canvas;
            if (Master == null) return; // redundency if statment


            // We create all the instances of all our objects here
            // Objects will  connect them selfs  togather the back
            // button is something different


            // <ASSETS START>

            // this creates and attach the function to the back button
            BackButton = new BackButton(Master);
            Master.Children.Add(BackButton);
            BackButton.Trigger = BackButtonFunction;

            // this creates and attach the function to the back button
            SettingButton = new SettingButton(Master);
            Master.Children.Add(SettingButton);
            SettingButton.Trigger = SettingButtonFunction;

            // <ASSETS END>



            // <Holders START>

            // this creates the AdvertisedDevice Holder
            UIAdvertisedDevices = new AdvertisedDevices(Master);
            Master.Children.Add(UIAdvertisedDevices);
            Holders.Add(UIAdvertisedDevices);

            // this creates the ConnectionDevice Holder
            UIDeviceConnection = new DeviceConnection(Master);
            Master.Children.Add(UIDeviceConnection);
            Holders.Add(UIDeviceConnection);

            // this creates the DeviceSetting Holder
            UIDeviceSetting = new DeviceSetting(Master);
            Master.Children.Add(UIDeviceSetting);
            Holders.Add(UIDeviceSetting);

            // <HOLDERS END>


            BackButton.Show();
            SettingButton.Show();

            TransitionForward(UIAdvertisedDevices);
        }





        private static bool _TransitionForwardRunning = false;
        public static async void TransitionForward(IDisplayable Holder) {
            if (_TransitionForwardRunning) return;
            _TransitionForwardRunning = true;
            if (DisplayedHolder != null) {
                HoldersHistory.Add(DisplayedHolder);
                DisplayedHolder.Hide();
                DisplayedHolder = Holder; // redundency
                await Task.Delay(Config.TransitionDuration / 2); // wait for at bit to give ot a smoother feel
            }
            DisplayedHolder = Holder;
            Holder.Show();
            _TransitionForwardRunning = false;
        }

        private static bool _TransitionBackRunning = false;
        public static async void TransitionBack()
        {
            if (_TransitionBackRunning) return;
            _TransitionBackRunning = true;
            if (HoldersHistory.Count > 0 &&
                DisplayedHolder != null)
            {
                var lastHolder = HoldersHistory[HoldersHistory.Count - 1];
                HoldersHistory.RemoveAt(HoldersHistory.Count - 1);


                DisplayedHolder.Hide();
                DisplayedHolder = lastHolder;
                await Task.Delay(Config.TransitionDuration / 2);
                DisplayedHolder.Show();
            }
            _TransitionBackRunning = false;
        }


        public static void BackButtonFunction()
        {

            // this is for the setting
            if (UIDeviceSetting != null &&
                SettingButton != null &&
                UIDeviceSetting.IsDisplayed)
            {
                SettingButtonFunction(); // loop back to the other function if the button was pressed
                return;
            }

            TransitionBack(); // Transision backward
        }



        public static void SettingButtonFunction()
        {
            if (_TransitionForwardRunning || _TransitionBackRunning) return;
            if (UIDeviceSetting != null &&
                !UIDeviceSetting.IsDisplayed) // if we dont have the setting displayed
            {
                if (SettingButton != null) SettingButton.RotateAntiClockwise();
                TransitionForward(UIDeviceSetting);
                return;
            }
            if (UIDeviceSetting != null &&
                UIDeviceSetting.IsDisplayed) // if we have the setting displayed
            {
                if (SettingButton != null) SettingButton.RotateClockwise();
                TransitionBack();
                return;
            }

        }

    }
}
