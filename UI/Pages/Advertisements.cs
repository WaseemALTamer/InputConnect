using Avalonia.Controls.Primitives;
using InputConnect.UI.Containers;
using System.Collections.Generic;
using InputConnect.Structures;
using Avalonia.Interactivity;
using InputConnect.Setting;
using InputConnect.Network;
using Avalonia.Threading;
using Avalonia.Controls;
using Avalonia;
using System;
using InputConnect.UI.Animations;




namespace InputConnect.UI.Pages
{
    // this file will only provide the structure of the canvase and how it looks like
    // it will also provide the tools to add and remove  the  children  of it none of
    // its logic will be ran through it

    // this file ended also running the logic as well

    public class Advertisements : Base
    {



        private List<Advertisement>? _Devices = new List<Advertisement>();
        public List<Advertisement>? Devices { 
            get { return _Devices; } 
            set { _Devices = value; } 
        }

        private int AdsPaddyY = 10;


        private DispatcherTimer? Ticktimer;
        private int _Tick = 100;
        public int Tick{
            get { return _Tick; }
            set { _Tick = value; }
        }

        public Advertisements(Canvas? master) : base(master)
        {
            if (MainCanvas == null) return;

            Ticktimer = new DispatcherTimer{
                Interval = TimeSpan.FromMilliseconds(Tick) // 
            };
            Ticktimer.Tick += AdsManager;
            Ticktimer.Start();

            MessageManager.OnAdvertisement += () => { Dispatcher.UIThread.Post(() => AdsManager()); };
        }


        public void OnResize(object? sender = null, SizeChangedEventArgs? e = null){



        }




        public void AdsManager(object? sender = null, object? e = null){
            if (Devices == null) return;
            if(!IsDisplayed) return; // iti s not displayed there is nothing to do then

            Devices.RemoveAll(item => item == null); // Remove null values
            bool _isAdded = false;
            DateTime time;
            for (int i = MessageManager.Advertisements.Count - 1; i >= 0; i--){
                var message = MessageManager.Advertisements[i];
                if (message.Time == null) continue;
                time = DateTime.Parse(message.Time);

                if ((DateTime.Now - time) > TimeSpan.FromSeconds(Config.AdvertiseTimeSpan)){
                    MessageManager.Advertisements.Remove(message);
                    continue;
                }

                for (int j = Devices.Count - 1; j >= 0; j--){
                    var UiAd = Devices[j];
                    if (UiAd.Message == null) continue;

                    if (message.IP == ((MessageUDP)UiAd.Message).IP){
                        UiAd.Message = message;
                        UiAd.Update();
                        _isAdded = true;
                        break;
                    }
                }
                if (!_isAdded){
                    Add(message);
                }

                _isAdded = false;
            }

            for (int j = Devices.Count - 1; j >= 0; j--){
                var UiAd = Devices[j];
                if (UiAd.Message != null && ((MessageUDP)UiAd.Message).Time != null && MainCanvas != null) {
                    time = DateTime.Parse(((MessageUDP)UiAd.Message).Time);
                    if ((DateTime.Now - time) > TimeSpan.FromSeconds(Config.AdvertiseTimeSpan)){
                        Devices.Remove(UiAd);
                        UiAd.Kill();
                        PlaceAds();
                        continue;
                    }
                }
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
