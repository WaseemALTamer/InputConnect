using InputConnect.UI.Containers;
using Avalonia.Controls;
using InputConnect.Network;
using System.Collections.Generic;
using Avalonia.Threading;
using InputConnect.Structures;
using System;




namespace InputConnect.UI.Pages
{
    public class Connections : Base
    {



        private List<ConnectedDevice> _Devices = new List<ConnectedDevice>();
        public List<ConnectedDevice> Devices
        {
            get { return _Devices; }
            set { _Devices = value; }
        }

        private int AdsPaddyY = 10;




        public Connections(Canvas? master) : base(master)
        {
            // we ensure that it runs on the main thread because we are working with the ui
            MessageManager.OnConnect += (message) => { Dispatcher.UIThread.Post(() => Update()); };
            MessageManager.OnAccept += (message) => { Dispatcher.UIThread.Post(() => Update()); };
            MessageManager.OnDecline += (message) => { Dispatcher.UIThread.Post(() => Update()); };
        }


        public void OnResize(object? sender = null, SizeChangedEventArgs? e = null)
        {



        }




        public void Update(object? sender = null, object? e = null){
            for (int i = 0; i < InputConnect.Connections.Devices.ConnectionList.Count; i++){
                var _found = false; // this will be used to indecated if we found the device responsible for the message
                var device = InputConnect.Connections.Devices.ConnectionList[i];
                for (int j = 0; j < Devices.Count; j++)
                {
                    var UIobject = Devices[j];
                    if (UIobject == null || UIobject.Device == null) continue;
                    if (UIobject.Device.MacAddress == device.MacAddress){
                        UIobject.Device = device;
                        UIobject.Update(); // update it for values inside of it
                        _found = true;
                        break;
                    }
                }
                if (_found) continue;

                Add(device);
            }

            // now we can check for any devices that we have that are not in the advertisement
            for (int i = Devices.Count - 1; i >= 0; i--)
            {

                var _found = false;
                var connection = Devices[i];
                if (connection == null || connection.Device == null) continue;
                for (int j = 0; j < InputConnect.Connections.Devices.ConnectionList.Count; j++){
                    var _device = InputConnect.Connections.Devices.ConnectionList[j];
                    if (connection.Device == _device){
                        _found = true;
                        break;
                    }
                }
                if (_found) continue;
                connection.Kill();
                Devices.Remove(connection);
                PlaceAds();
            }

        }


        public void Add(Connection device){
            if (Devices != null){
                var _ad = new ConnectedDevice(MainCanvas, device);
                Devices.Add(_ad);
                if (MainCanvas != null)
                {
                    MainCanvas.Children.Add(_ad);
                }
                PlaceAds();
            }
        }

        private void PlaceAds()
        {
            if (Devices == null || MainCanvas == null) return;

            int _index = 0;
            int _lostindex = 0;
            foreach (var _advertisement in Devices)
            {
                if (_advertisement != null)
                {
                    //Canvas.SetRight(_advertisement, (MainCanvas.Width - _advertisement.Width) / 2);
                    //Canvas.SetTop(_advertisement, AdsPaddyY + (_advertisement.Height + AdsPaddyY) * (_index - _lostindex));
                    _advertisement.SetPostionTranslate((MainCanvas.Width - _advertisement.Width) / 2, AdsPaddyY + (_advertisement.Height + AdsPaddyY) * (_index - _lostindex));
                    MainCanvas.Height = AdsPaddyY + (_advertisement.Height + AdsPaddyY) * (_index - _lostindex) + (_advertisement.Height + AdsPaddyY);
                }
                else
                {
                    _lostindex++;
                }
                _index++;
            }
            ShowOnlyVissibleAds();
        }



        // this function sill needs more work as of now it shows everything that is in the array
        // you should later on make it only show ones that are displayed on screen
        private void ShowOnlyVissibleAds()
        {
            if (Devices == null) return;

            int _index = 0;
            int _lostindex = 0;
            foreach (var _advertisement in Devices)
            {
                if (_advertisement != null)
                {
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
