using Avalonia.Controls;
using Avalonia.Threading;
using InputConnect.Network;
using InputConnect.Structures;
using InputConnect.UI.Containers;
using InputConnect.UI.Containers.Common;
using System;
using System.Collections.Generic;






namespace InputConnect.UI.Pages
{
    // this file will only provide the structure of the canvase and how it looks like
    // it will also provide the tools to add and remove  the  children  of it none of
    // its logic will be ran through it

    // this file ended also running the logic as well


    // update this file instead of running it on a dispach timer  you can just simply
    // sub to the new Advertisements and then only update your current displayed list
    // based on what you will be feed, you simply dont  need  to  update every second
    // simply update when there is a new message  that  comes in, if there is message
    // that is more than 10 seconds old just leave it  there as  long as  there is no
    // messages, good luck

    public class Advertisements : Base
    {



        private List<Advertisement> _Devices = new List<Advertisement>();
        public List<Advertisement> Devices { 
            get { return _Devices; } 
            set { _Devices = value; } 
        }

        private int AdsPaddyY = 10;




        public Advertisements(Canvas? master) : base(master){
            // we ensure that it runs on the main thread because we are working with the ui
            MessageManager.OnAdvertisement += () => { Dispatcher.UIThread.Post(() => Update()); };


        }


        public void OnResize(object? sender = null, SizeChangedEventArgs? e = null){



        }




        public void Update(object? sender = null, object? e = null){
            for (int i = 0; i < MessageManager.Advertisements.Count; i++) {
                var _found = false; // this will be used to indecated if we found the device responsible for the message
                var message = MessageManager.Advertisements[i];
                for (int j = 0; j < Devices.Count; j++) {
                    var device = Devices[j];
                    if (device == null || device.Message == null) continue;
                    if (device.Message != null && message.MacAddress == device.Message.MacAddress) { 
                        if (message.Time != device.Message.Time) { // only update it if we change the time
                            device.Message = message;
                            device.Update(); // update it for values inside of it
                        }
                        _found = true;
                        break;
                    }
                }
                if (_found) continue;
                Add(message);
            }

            // now we can check for any devices that we have that are not in the advertisement
            for (int i = Devices.Count - 1; i >= 0; i--){
                var _found = false;
                var device = Devices[i];
                if (device == null || device.Message == null) continue;
                for (int j = 0; j < MessageManager.Advertisements.Count; j++){
                    var message = MessageManager.Advertisements[j];
                    if (device.Message == message) {
                        _found = true;
                        break;
                    }
                }
                if (_found) continue;
                device.Kill();
                Devices.Remove(device);
                PlaceAds();
            }

        }


        public void Add(MessageUDP advertisement) {
            if (Devices != null) {
                var _ad = new Advertisement(MainCanvas, advertisement);
                Devices.Add(_ad);
                if (MainCanvas != null){
                    MainCanvas.Children.Add(_ad);
                }
                PlaceAds();
            }
        }

        private void PlaceAds() {
            if (Devices == null || MainCanvas == null) return;
            int _index = 0;
            int _lostindex = 0;
            foreach (var _advertisement in Devices) {
                if (_advertisement != null){
                    //Canvas.SetRight(_advertisement, (MainCanvas.Width - _advertisement.Width) / 2);
                    //Canvas.SetTop(_advertisement, AdsPaddyY + (_advertisement.Height + AdsPaddyY) * (_index - _lostindex));
                    _advertisement.SetPostionTranslate((MainCanvas.Width - _advertisement.Width) / 2, AdsPaddyY + (_advertisement.Height + AdsPaddyY) * (_index - _lostindex));
                    MainCanvas.Height = AdsPaddyY + (_advertisement.Height + AdsPaddyY) * (_index - _lostindex) + (_advertisement.Height + AdsPaddyY);
                }
                else {
                    _lostindex ++;
                }
                _index++;
            }
            ShowOnlyVissibleAds();
        }



        // this function sill needs more work as of now it shows everything that is in the array
        // you should later on make it only show ones that are displayed on screen
        private void ShowOnlyVissibleAds() {
            if (Devices == null) return;
            int _index = 0;
            int _lostindex = 0;
            foreach (var _advertisement in Devices){
                if (_advertisement != null){
                    _advertisement.Show();
                }
                else
                {
                    _lostindex++;
                }
                _index++;
            }
        }
    }
}
