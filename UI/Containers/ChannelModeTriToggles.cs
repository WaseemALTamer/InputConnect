using InputConnect.UI.Containers.Common;
using InputConnect.Setting;
using Avalonia.Controls;
using Avalonia;
using InputConnect.SharedData;
using System;
using InputConnect.Structures;





namespace InputConnect.UI.Containers
{
    public class ChannelModeTriToggles : Border
    {


        private Canvas? Master;

        private Canvas? _MainCanvas;
        public Canvas? MainCanvas{
            get { return _MainCanvas; }
            set { _MainCanvas = value; }
        }





        /// <summary>
        ///  Transmit has a state of <int 2>
        ///  Recieve has a state of <int 0>
        ///  None has a state of <int 1>
        ///  this applies to the three state toggle
        /// </summary>
        /// 

        private TriStateToggle MouseChannelToggle = new TriStateToggle();
        private TriStateToggle KeyboardChannelToggle = new TriStateToggle();
        private TriStateToggle AudioChannelToggle = new TriStateToggle();

        private TextBlock? SendText;
        private TextBlock? ReceiveText;


        private Image? MouseImage;
        private Image? KeyboardImage;
        private Image? AudioImage;



        //private Animations.Transations.Uniform? WrongTokenTranstion;

        public ChannelModeTriToggles(Canvas? master){ 
            Master = master;

            Opacity = 1;
            ClipToBounds = true;
            Background = Themes.Holder;
            CornerRadius = new CornerRadius(Config.CornerRadius);

            Width = 400; Height = 390;


            MainCanvas = new Canvas{
                ClipToBounds = true,
            };
            Child = MainCanvas;

            MainCanvas.Children.Add(MouseChannelToggle);
            MainCanvas.Children.Add(KeyboardChannelToggle);
            MainCanvas.Children.Add(AudioChannelToggle);

            MouseChannelToggle.Trigger += SetMouseState;
            KeyboardChannelToggle.Trigger += SetKeyboardState;
            AudioChannelToggle.Trigger += SetAudioState;


            SendText = new TextBlock{
                Text = "Transmit",
                Width = 110, Height = 50,
                FontSize = 25
            };
            MainCanvas.Children.Add(SendText);

            ReceiveText = new TextBlock{
                Text = "Receive",
                Width = 100, Height = 50,
                FontSize = 25
            };
            MainCanvas.Children.Add(ReceiveText);


            MouseImage = new Image { 
                Stretch = Avalonia.Media.Stretch.Fill,
                Width = 75, 
                Height = 75,
            };
            MainCanvas.Children.Add(MouseImage);
            Assets.AddAwaitedAction(() => {
                MouseImage.Source = Assets.MouseBitmap;
            });

            KeyboardImage = new Image{
                Stretch = Avalonia.Media.Stretch.Fill,
                Width = 75,
                Height = 75,
            };
            MainCanvas.Children.Add(KeyboardImage);
            Assets.AddAwaitedAction(() => {
                KeyboardImage.Source = Assets.KeyboardBitmap;
            });

            AudioImage = new Image{
                Stretch = Avalonia.Media.Stretch.Fill,
                Width = 75,
                Height = 75,
            };
            MainCanvas.Children.Add(AudioImage);
            Assets.AddAwaitedAction(() => {
                AudioImage.Source = Assets.AudioBitmap;
            });



            OnSizeChanged();

            Events.TargetDeviceConnectionChanged += Update;

            if (Master != null){
                Master.SizeChanged += OnSizeChanged;
            }
        }

        public void OnSizeChanged(object? sender = null, object? e = null){
            if (Master != null) {

                if (MainCanvas != null){
                    MainCanvas.Width = Width; 
                    MainCanvas.Height = Height;

                    double spacingToggles = (MainCanvas.Height - MouseChannelToggle.Height) / 3;
                    


                    Canvas.SetLeft(MouseChannelToggle, (MainCanvas.Width - MouseChannelToggle.Width) / 2);
                    Canvas.SetTop(MouseChannelToggle, spacingToggles * 1 - 25);


                    Canvas.SetLeft(KeyboardChannelToggle, (MainCanvas.Width - KeyboardChannelToggle.Width) / 2);
                    Canvas.SetTop(KeyboardChannelToggle, spacingToggles * 2 - 25);


                    Canvas.SetLeft(AudioChannelToggle, (MainCanvas.Width - AudioChannelToggle.Width) / 2);
                    Canvas.SetTop(AudioChannelToggle, spacingToggles * 3 - 25);

                    double spacingImages;
                    if (MouseImage != null) {
                        spacingImages = (MainCanvas.Height - MouseImage.Width) / 3;
                    }
                    else {
                        spacingImages = (MainCanvas.Height - 75) / 3;
                    }
                    

                    if (MouseImage != null){
                        Canvas.SetLeft(MouseImage, (MainCanvas.Width - MouseImage.Width) - 25);
                        Canvas.SetTop(MouseImage, spacingImages * 1 - 30);
                    }

                    if (KeyboardImage != null){
                        Canvas.SetLeft(KeyboardImage, (MainCanvas.Width - KeyboardImage.Width) - 25);
                        Canvas.SetTop(KeyboardImage, spacingImages * 2 - 30);
                    }

                    if (AudioImage != null){
                        Canvas.SetLeft(AudioImage, (MainCanvas.Width - AudioImage.Width) - 25);
                        Canvas.SetTop(AudioImage, spacingImages * 3 - 10);
                    }





                    if (SendText != null) {
                        Canvas.SetLeft(SendText, (MainCanvas.Width - SendText.Width - 25));
                        Canvas.SetTop(SendText, 20);
                    }

                    if (ReceiveText != null){
                        Canvas.SetLeft(ReceiveText, 25);
                        Canvas.SetTop(ReceiveText, 20);
                    }


                }
            }
        }


        public void Update(){
            // this function will update and will set the state postion
            // based on the targed device in the Shared Data

            if (TargetedDevice.Connection != null && 
                TargetedDevice.Connection.State == Connections.Constants.StateConnected)
            {

                MouseChannelToggle.SetLock(false);
                KeyboardChannelToggle.SetLock(false);
                AudioChannelToggle.SetLock(false);

                if (TargetedDevice.Connection.MouseState == Connections.Constants.Transmit){
                    MouseChannelToggle.SetState(2);
                }
                if (TargetedDevice.Connection.MouseState == Connections.Constants.Receive){
                    MouseChannelToggle.SetState(0);
                }
                if (TargetedDevice.Connection.MouseState == null || 
                    TargetedDevice.Connection.MouseState == "")
                {
                    MouseChannelToggle.SetState(1);
                }


                if (TargetedDevice.Connection.KeyboardState == Connections.Constants.Transmit){
                    KeyboardChannelToggle.SetState(2);
                }
                if (TargetedDevice.Connection.KeyboardState == Connections.Constants.Receive){
                    KeyboardChannelToggle.SetState(0);
                }
                if (TargetedDevice.Connection.KeyboardState == null || 
                    TargetedDevice.Connection.KeyboardState == "")
                {
                    KeyboardChannelToggle.SetState(1);
                }


                if (TargetedDevice.Connection.AudioState == Connections.Constants.Transmit){
                    AudioChannelToggle.SetState(2);
                }
                if (TargetedDevice.Connection.AudioState == Connections.Constants.Receive){
                    AudioChannelToggle.SetState(0);
                }
                if (TargetedDevice.Connection.AudioState == null || 
                    TargetedDevice.Connection.AudioState == "")
                {
                    AudioChannelToggle.SetState(1);
                }


            }
            else{
                MouseChannelToggle.SetLock(true);
                KeyboardChannelToggle.SetLock(true);
                AudioChannelToggle.SetLock(true);

                MouseChannelToggle.SetState(1);
                KeyboardChannelToggle.SetState(1);
                AudioChannelToggle.SetState(1);
            }
        }

        void SetMouseState(int code)
        {
            ///  Transmit has a state of <int 2>
            ///  Recieve has a state of <int 0>
            ///  None has a state of <int 1>
            if (TargetedDevice.Connection != null && TargetedDevice.Connection.State == Connections.Constants.StateConnected){
                if (code == 0){
                    TargetedDevice.Connection.MouseState = Connections.Constants.Receive;
                }
                if (code == 1){
                    TargetedDevice.Connection.MouseState = null;
                }
                if (code == 2){
                    TargetedDevice.Connection.MouseState = Connections.Constants.Transmit;
                }
            }
            else{
                MouseChannelToggle.SetState(1);
            }
        }

        void SetKeyboardState(int code){
            ///  Transmit has a state of <int 2>
            ///  Recieve has a state of <int 0>
            ///  None has a state of <int 1>
            if (TargetedDevice.Connection != null && TargetedDevice.Connection.State == Connections.Constants.StateConnected){
                if (code == 0){
                    TargetedDevice.Connection.KeyboardState = Connections.Constants.Receive;
                }
                if (code == 1){
                    TargetedDevice.Connection.KeyboardState = null;
                }
                if (code == 2){
                    TargetedDevice.Connection.KeyboardState = Connections.Constants.Transmit;
                }
            }
            else {
                KeyboardChannelToggle.SetState(1);
            }
        }

        void SetAudioState(int code) {
            ///  Transmit has a state of <int 2>
            ///  Recieve has a state of <int 0>
            ///  None has a state of <int 1>
            if (TargetedDevice.Connection != null && TargetedDevice.Connection.State == Connections.Constants.StateConnected){
                if (code == 0) {
                    TargetedDevice.Connection.AudioState = Connections.Constants.Receive;
                }
                if (code == 1){
                    TargetedDevice.Connection.AudioState = null;
                }
                if (code == 2){
                    TargetedDevice.Connection.AudioState = Connections.Constants.Transmit;
                }
            }
            else{
                AudioChannelToggle.SetState(1);
            }
        }

    }
}


