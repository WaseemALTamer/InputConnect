using InputConnect.Structures;
using InputConnect.Setting;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia;
using Tmds.DBus.Protocol;
using System;





namespace InputConnect.UI.Containers
{



    public class Advertisement : Border
    {


        private Canvas? Master;
        private TextBlock? Data;

        private Animations.Transations.EaseInOut? PostionTranslation;
        private Animations.Transations.Uniform? ShowHideTransition;
        private Animations.Transations.Uniform? HoverTranslation;

        private MessageUDP? _Message;

        public MessageUDP? Message{
            get { return _Message; }
            set { _Message = value; }
        }


        private Canvas? _MainCanvas;

        public Canvas? MainCanvas{
            get { return _MainCanvas; }
            set { _MainCanvas = value; }
        }


        public Advertisement(Canvas? master = null, MessageUDP? advertisement = null){
            Master = master;
            Message = advertisement;

            Background = Themes.Advertisement;
            IsVisible = false;
            Opacity = 0;

            MainCanvas = new Canvas();




            Data = new TextBlock{
                Text = $"Name: None  \n\n" +
                       $"IP: None  \n" +
                       $"MacAddress: None\n"+
                       $"Time: None",
                Width = Width * 0.85,
                Height = Height * 0.7,
                FontSize = Config.FontSize
            };

            ShowHideTransition = new Animations.Transations.Uniform{
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration,
                Trigger = ShowHideSetOpeicity,
            };

            if (Master != null) {
                Update(); // trigger the on resize so we can set the dimention
                Master.SizeChanged += OnSizeChanged;
                CornerRadius = new CornerRadius(Config.CornerRadius);
            }

            HoverTranslation = new Animations.Transations.Uniform{
                StartingValue = 0.5,
                EndingValue = 0.3,
                CurrentValue = 1,
                Duration = Config.TransitionHover,
                Trigger = OnHoverSetBackground
            };

            PointerEntered += HoverTranslation.TranslateForward;
            PointerExited += HoverTranslation.TranslateBackward;
            PointerReleased += OnClick;

            MainCanvas.Children.Add(Data);
            Child = MainCanvas;

        }


        public void OnSizeChanged(object? sender = null, SizeChangedEventArgs? e = null){
            if (Master != null) {
                Width = Master.Width - 100;
                Height = 150;

                if (MainCanvas != null){
                    MainCanvas.Width = Width;
                    MainCanvas.Height = Height;
                }



                if (Data != null){
                    Data.Width = Width * 0.85;
                    Data.Height = Height * 0.85;
                    Canvas.SetLeft(Data, 100);
                    Canvas.SetTop(Data, 10);
                }

            }
        }

        public void Update() {
            OnSizeChanged(); // update the size of it just in case

            if (Data == null) return;
            if (Message != null){
                var ad = (MessageUDP)Message; // we cast it because it did not take it for somereason
                Data.Text = $"Name: {ad.DeviceName ?? "None"}  \n\n" +
                            $"IP: {ad.IP ?? "None"}  \n" +
                            $"MacAddress: {ad.MacAddress ?? "None"}  \n" +
                            $"Time: {ad.Time}";
            }
        }

        public void Show(){
            if (Opacity == 1) return;

            if (ShowHideTransition != null)
                ShowHideTransition.TranslateForward();
        }

        public void Hide(){
            if (Opacity == 0) return;
            if (ShowHideTransition != null)
                ShowHideTransition.TranslateBackward();
        }

        private bool IsKill = false;
        public void Kill(){
            Message = null;
            if (ShowHideTransition != null){
                ShowHideTransition.TranslateBackward();
                IsKill = true;
            }
        }

        public void ShowHideSetOpeicity(double Value){

            //if (HoverTranslation != null &&
            //    Value <= HoverTranslation.CurrentValue) {
            //    Opacity = Value;
            //}

            Opacity = Value;

            IsVisible = Opacity != 0; // reducndency line for the Avoilonia Thread to ignore the window when not needed
            if (Opacity == 0 &&
                IsKill == true &&
                ShowHideTransition != null && 
                ShowHideTransition.FunctionRunning == false &&
                Master != null) { // this if statment just checks if the user want to kill this object and removes it
                                  // from its parent

                Master.Children.Remove(this);

            }
        }

        public void OnHoverSetBackground(double Value){
            

            if (Background is SolidColorBrush solidBrush){
                    var originalColor = solidBrush.Color;
                    byte newAlpha = (byte)(255 * Value);
                    var fadedColor = Color.FromArgb(newAlpha, originalColor.R, originalColor.G, originalColor.B);
                    Background = new SolidColorBrush(fadedColor);
                }

            //if (ShowHideTransition != null && ShowHideTransition.FunctionRunning == false) {
            //    Opacity = Value;
            //    return;
            //}
            //if (HoverTranslation != null) HoverTranslation.Reset();
        }


        private Vector InitialPos;
        private Vector FinalPos;
        public void SetPostionTranslate(double Xpos, double Ypos) {

            if (PostionTranslation != null) PostionTranslation.FunctionRunning = false;

            PostionTranslation = new Animations.Transations.EaseInOut{
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration,
                Trigger = SetPostionTrigger
            };

            if (double.IsNaN(Canvas.GetRight(this))) { // this will only occures on start up
                Canvas.SetRight(this, Xpos);
                Canvas.SetTop(this, Ypos);
                return;
            }
            
            InitialPos = new Vector(Canvas.GetRight(this), Canvas.GetTop(this));
            FinalPos = new Vector(Xpos, Ypos);

            PostionTranslation.TranslateForward();
        }

        private void SetPostionTrigger(double value) {
            Canvas.SetRight(this, InitialPos.X + (FinalPos.X - InitialPos.X) * value);
            Canvas.SetTop(this, InitialPos.Y + (FinalPos.Y - InitialPos.Y) * value);
        }


        private void OnClick(object? sender, PointerReleasedEventArgs e){

            e.Handled = true;
            if (e.GetCurrentPoint(null).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased){
                if (sender is Control control){
                    var pointerPosition = e.GetPosition(control);
                    if (pointerPosition.X < 0 || pointerPosition.Y < 0) return;
                    if (pointerPosition.X > Width || pointerPosition.Y > Height) return;

                    if (Message == null) return;

                    var _message = (MessageUDP)Message;




                    // this will check if the device is in the connected and add it without the ip and as a
                    // Connected device 

                    bool _found = false;
                    for (int i = 0; i < Connections.Devices.ConnectionList.Count; i++){
                        var device = Connections.Devices.ConnectionList[i];
                        if (device.MacAddress == _message.MacAddress){
                            _found = true;
                            // there is a problem that you cant establish a connection again
                            // becuase you dont have the ip address
                            SharedData.TargetedDevice.Clear();
                            SharedData.TargetedDevice.Connection = device;
                            SharedData.TargetedDevice.MacAddress = device.MacAddress;
                            SharedData.TargetedDevice.DeviceName = device.DeviceName;
                            SharedData.TargetedDevice.Token = device.Token;
                            SharedData.TargetedDevice.State = device.State;
                            break;
                        }
                    }

                    // this will add it to connect to it
                    if (!_found) {
                        // Add the date to the TargetedDevice
                        SharedData.TargetedDevice.Clear();
                        SharedData.TargetedDevice.MacAddress = _message.MacAddress;
                        SharedData.TargetedDevice.DeviceName = _message.Text;
                    }




                    // Transition to the next window
                    if (PublicWidgets.UIDevice != null) {
                        PublicWidgets.TransitionForward(PublicWidgets.UIDevice);
                    }
                }
            }
        }
    }
}
