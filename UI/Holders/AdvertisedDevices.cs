
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




namespace InputConnect.UI.Holders
{
    // this file will only provide the structure of the canvase and how it looks like
    // it will also provide the tools to add and remove  the  children  of it none of
    // its logic will be ran through it

    // this file ended also running the logic as well

    public class AdvertisedDevices : Border, IDisplayable
    {



        private Canvas? _Master;
        public Canvas? Master { get { return _Master; } 
                                set { _Master = value; } }


        private bool _IsDisplayed = false;
        public bool IsDisplayed{
            get { return _IsDisplayed; }
            set { _IsDisplayed = value; }
        }

        private Animations.Transations.Uniform? ShowHideTransition;

        private ScrollViewer? _ScrollViewer;
        private SmoothScrolling? _ScrollingAnimation;

        private Canvas? _MainCanvas;
        public Canvas? MainCanvas { get { return _MainCanvas; } 
                                    set { _MainCanvas = value; } }


        private List<Advertisement>? _Advertisements = new List<Advertisement>();
        public List<Advertisement>? Advertisements { get { return _Advertisements; } set { _Advertisements = value; } }

        private int AdsPaddyY = 10;


        private DispatcherTimer? _timer;

        public AdvertisedDevices(Canvas? master){
            Master = master;

            Opacity = 0;
            IsVisible = false;
            ClipToBounds = true;
            IsHitTestVisible = true;
            CornerRadius = new CornerRadius(Config.CornerRadius);
            Background = Themes.Holders;




            ShowHideTransition = new Animations.Transations.Uniform
            {
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration,
                Trigger = SetOpeicity,
            };

            _ScrollViewer = new ScrollViewer{
                IsHitTestVisible = true
            };


            _ScrollingAnimation = new Animations.SmoothScrolling
            {
                Trigger = SmoothVerticalScrollerTrigger
            };


            MainCanvas = new Canvas{
                IsHitTestVisible = true,
            };
            MainCanvas.ClipToBounds = true;

            _ScrollViewer.Content = MainCanvas;
            _ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            _ScrollViewer.AddHandler(PointerWheelChangedEvent, _ScrollingAnimation.OnPointerWheelChanged, RoutingStrategies.Tunnel); // this methode is used as the file will be already loaded and the function will already have                                                                                                   // functions that end the event totally
            PointerWheelChanged += _ScrollingAnimation.OnPointerWheelChanged;
            MainCanvas.PointerWheelChanged += _ScrollingAnimation.OnPointerWheelChanged;







            if (Master != null){
                OnResize(); // trigger the function to set the sizes
                Master.SizeChanged += OnResize;
            }


            Child = _ScrollViewer;


            _timer = new DispatcherTimer{
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += AdsManager;
            _timer.Start();

            MessageManager.OnAdvertisement += () => { Dispatcher.UIThread.Post(() => AdsManager()); };
        }


        public void OnResize(object? sender = null, SizeChangedEventArgs? e = null){
            if (Master != null){
                Width = Master.Width * 0.85;
                Height = Master.Height * 0.85;

                if (MainCanvas != null){
                    MainCanvas.Width = Width;
                    MainCanvas.MinHeight = Height;
                }

                if (_ScrollViewer != null){
                    _ScrollViewer.Width = Width;
                    _ScrollViewer.Height = Height;
                }

                Canvas.SetRight(this, (Master.Width - Width) / 2);
                Canvas.SetBottom(this, (Master.Height - Height) / 2);
            }
        }


        public void Show(){
            IsDisplayed = true;
            if (ShowHideTransition != null)
                ShowHideTransition.TranslateForward();
        }

        public void Hide(){
            IsDisplayed = false;
            if (ShowHideTransition != null)
                ShowHideTransition.TranslateBackward();
        }

        public void SetOpeicity(double Value){
            Opacity = Value;
            IsVisible = Opacity != 0; // reducndency line for the Avoilonia Thread to ignore the window when not needed
        }

        private void SmoothVerticalScrollerTrigger(double Value)
        {
            if (_ScrollViewer != null){
                _ScrollViewer.Offset = new Vector(_ScrollViewer.Offset.X, _ScrollViewer.Offset.Y + Value);
                PlaceAds();
            }
        }


        public void AdsManager(object? sender = null, object? e = null){
            if (Advertisements == null) return;
            Advertisements.RemoveAll(item => item == null); // Remove null values
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

                for (int j = Advertisements.Count - 1; j >= 0; j--){
                    var UiAd = Advertisements[j];
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

            for (int j = Advertisements.Count - 1; j >= 0; j--){
                var UiAd = Advertisements[j];
                if (UiAd.Message != null && ((MessageUDP)UiAd.Message).Time != null && MainCanvas != null) {
                    time = DateTime.Parse(((MessageUDP)UiAd.Message).Time);
                    if ((DateTime.Now - time) > TimeSpan.FromSeconds(Config.AdvertiseTimeSpan)){
                        Advertisements.Remove(UiAd);
                        UiAd.Kill();
                        PlaceAds();
                        continue;
                    }
                }
            }
        }


        public void Add(MessageUDP advertisement) {

            if (Advertisements != null) {
                var _ad = new Advertisement(MainCanvas, advertisement);
                Advertisements.Add(_ad);
                if (MainCanvas != null){
                    MainCanvas.Children.Add(_ad);
                }
                PlaceAds();
            }
        }

        private void PlaceAds() {
            if (Advertisements == null || MainCanvas == null) return;


            int _index = 0;
            int _lostindex = 0;
            foreach (var _advertisement in Advertisements) {
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
            if (Advertisements == null) return;

            int _index = 0;
            int _lostindex = 0;
            foreach (var _advertisement in Advertisements){
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
