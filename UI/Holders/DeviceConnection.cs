
using InputConnect.Network.Constants;
using Avalonia.Controls.Primitives;
using InputConnect.UI.Animations;
using InputConnect.Structures;
using Avalonia.Interactivity;
using InputConnect.Setting;
using InputConnect.Network;
using Avalonia.Controls;
using Avalonia;
using System;







namespace InputConnect.UI.Holders
{
    // this file will only provide the structure of the canvase and how it looks like
    // it will also provide the tools to add and remove  the  children  of it none of
    // its logic will be ran through it

    // this file ended also running the logic as well

    public class DeviceConnection : Border, IDisplayable
    {

        private Canvas? _Master;
        public Canvas? Master{
            get { return _Master; }
            set { _Master = value; }
        }


        private bool _IsDisplayed = false;
        public bool IsDisplayed{
            get { return _IsDisplayed; }
            set { _IsDisplayed = value; }
        }

        private Animations.Transations.Uniform? ShowHideTransition;

        private ScrollViewer? _ScrollViewer;
        private SmoothScrolling? _ScrollingAnimation;

        private Canvas? _MainCanvas;
        public Canvas? MainCanvas{
            get { return _MainCanvas; }
            set { _MainCanvas = value; }
        }


        private Button? ConnectButton;

        public DeviceConnection(Canvas? master)
        {
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

            _ScrollViewer = new ScrollViewer
            {
                IsHitTestVisible = true
            };


            _ScrollingAnimation = new Animations.SmoothScrolling
            {
                Trigger = SmoothVerticalScrollerTrigger
            };


            MainCanvas = new Canvas
            {
                IsHitTestVisible = true,
            };
            MainCanvas.ClipToBounds = true;

            _ScrollViewer.Content = MainCanvas;
            _ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            _ScrollViewer.AddHandler(PointerWheelChangedEvent, _ScrollingAnimation.OnPointerWheelChanged, RoutingStrategies.Tunnel); // this methode is used as the file will be already loaded and the function will already have
                                                                                                                                     // functions that end the event totally
            
            PointerWheelChanged += _ScrollingAnimation.OnPointerWheelChanged;
            MainCanvas.PointerWheelChanged += _ScrollingAnimation.OnPointerWheelChanged;


            // here where all the children should be

            ConnectButton = new Button
            {
                Content = "Connect",
                Width = 150,
                Height = 50,
                Background = Themes.Button,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                FontSize = Config.FontSize,
                CornerRadius = new CornerRadius(Config.CornerRadius)
            };
            MainCanvas.Children.Add(ConnectButton);
            ConnectButton.Click += OnClickConnectButton;




            MessageManager.OnConnect += (Message) =>{
                Console.WriteLine(Message.IP);
                Console.WriteLine(Message.Text);
                if (Message.Text == null) return;
                Console.WriteLine(Encryptor.Decrypt(Message.Text, "RandomToken"));
            };



            if (Master != null)
            {
                OnResize(); // trigger the function to set the sizes
                Master.SizeChanged += OnResize;
            }

            Child = _ScrollViewer;
        }


        public void OnResize(object? sender = null, SizeChangedEventArgs? e = null){
            if (Master != null){
                Width = Master.Width * 0.85;
                Height = Master.Height * 0.85;


                if (MainCanvas != null)
                {
                    MainCanvas.Width = Width;
                    MainCanvas.Height = Height;

                    if (ConnectButton != null) {

                        Canvas.SetLeft(ConnectButton, (MainCanvas.Width - ConnectButton.Width) - 10);
                        Canvas.SetTop(ConnectButton, (MainCanvas.Height - ConnectButton.Height) - 10);
                    }
                }

                if (_ScrollViewer != null)
                {
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

        private void SmoothVerticalScrollerTrigger(double Value){
            if (_ScrollViewer != null){
                _ScrollViewer.Offset = new Vector(_ScrollViewer.Offset.X, _ScrollViewer.Offset.Y + Value);
            }
        }


        private void OnClickConnectButton(object? sender = null, RoutedEventArgs? e = null) {

            string text = Encryptor.Encrypt($"hello there my name is waseem and i am a 19 yeras old student in the universtiy of surry and here a a var that might change everything", "RandomToken");

            //string v = Encryptor.Decrypt(text, "RandomToken");

            //Console.WriteLine(v);

            MessageUDP message = new MessageUDP { 
                MessageType = MessageTypes.Connect,
                Text = text,
                IsEncrypted = true,
            };

            if (SharedData.TargetedDevice.IP != null)
                ConnectionUDP.SendUDP(SharedData.TargetedDevice.IP, message);
        
        }
    }
}
