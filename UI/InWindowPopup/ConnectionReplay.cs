using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using InputConnect.Network;
using InputConnect.Setting;
using InputConnect.UI.Containers;
using InputConnect.UI.Holders;
using System;
using System.Diagnostics.Metrics;
using static System.Net.Mime.MediaTypeNames;


namespace InputConnect.UI.InWindowPopup
{

    public class ConnectionReplay : Border
    {

        // this class is a simple pop up that will appear when someone tries to connect to you
        // the main perpose of this class is for you to add the token and connect to the other
        // device  both tokens should  be the  same on both computers  for  a pop up  the main
        // border is always just a full screen window which darkens everytihng beneath it

        private Canvas? _Master;
        public Canvas? Master{
            get { return _Master; }
            set { _Master = value; }
        }


        private Canvas? _MainCanvas;
        public Canvas? MainCanvas{
            get { return _MainCanvas; }
            set { _MainCanvas = value; }
        }


        // make sure the transitions dont clash out do that by checking if the functions are running
        private Animations.Transations.EaseOut? TransitionIn; // helps to transion in, then helps to transtion back if the connection was not established
        private Animations.Transations.EaseOut? TransitionOut; // if the device is accepted then transition to the right


        private Animations.Transations.Uniform? WrongTokenTranstion;

        private CloseButton? CloseButton;

        private TextBlock? DeviceData;
        private TextBox? Entry;

        private Button? AcceptButton;

        public ConnectionReplay(Canvas master) {
            Master = master;

            Opacity = 1;
            IsVisible = false;
            ClipToBounds = true;
            IsHitTestVisible = false;
            CornerRadius = new CornerRadius(Config.CornerRadius);
            Background = Themes.InWindowPopup;
            ZIndex = 6; // this makes sure it sets on top of everything



            MainCanvas = new Canvas{
                IsHitTestVisible = true,
            };


            // CloseButton.Trigger  // attach the trigger on the function


            TransitionIn = new Animations.Transations.EaseOut{
                StartingValue = 0,
                EndingValue = 0.5,
                Duration = Config.TransitionDuration * 2,
                Trigger = TriggerSetXPostion,
            };

            TransitionOut = new Animations.Transations.EaseOut{ // reversed so we get EaseIn function
                StartingValue = 1,
                EndingValue = 0.5,
                CurrentValue = 0.5,
                Duration = Config.TransitionDuration * 2,
                Trigger = TriggerSetXPostion,
            };


            WrongTokenTranstion = new Animations.Transations.Uniform{
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration,
                Trigger = TriggerWrongToken,
            };

            Connections.Manager.ActionOnIncomingConnection += OnIncomingConnection;

            if (Master != null){
                OnResize(); // trigger the function to set the sizes
                Master.SizeChanged += OnResize;
            }

            // create the button after setting the size
            CloseButton = new CloseButton(MainCanvas);
            MainCanvas.Children.Add(CloseButton);
            CloseButton.Show();

            CloseButton.Trigger += OnClickClosingButton;



            DeviceData = new TextBlock { // this is only going to hold the device name and the device ip and the time of the message
                                         // text will only be pushed in when we start to show this popup
                Text = "            Connection Request\n\n" +
                       "Device: \n" +
                       "IP: \n" +
                       "Time: \n",
                Width = Width - 50,
                Height = 150,
                FontSize = Config.FontSize,
            };

            MainCanvas.Children.Add(DeviceData);

            Entry = new TextBox{
                Width = 300,
                Height = 40,
                FontSize = Config.FontSize,
                CornerRadius = new CornerRadius(Config.CornerRadius),
                Watermark = "Token",
                PasswordChar = char.Parse("*"),
                Background = Themes.Entry,
            };
            MainCanvas.Children.Add(Entry);


            

            AcceptButton = new Button{
                Content = "Accept",
                Width = 150,
                Height = 50,
                Background = Themes.Button,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                FontSize = Config.FontSize,
                CornerRadius = new CornerRadius(Config.CornerRadius)
            };
            MainCanvas.Children.Add(AcceptButton);

            AcceptButton.Click += OnClickAcceptButton;



            Child = MainCanvas;

        }



        public void OnResize(object? sender = null, SizeChangedEventArgs? e = null)
        {
            if (Master != null)
            {
                Width = Master.Width * 0.50;
                Height = Master.Height * 0.35;

                Canvas.SetTop(this, (Master.Height - Height) / 2);
                if (TransitionIn != null && !TransitionIn.FunctionRunning &&
                    TransitionOut != null && !TransitionOut.FunctionRunning &&
                    IsVisible)
                {
                    Canvas.SetLeft(this, (Master.Width - Width) / 2);
                }


                if (MainCanvas != null) { 
                    MainCanvas.Width = Width; 
                    MainCanvas.Height = Height;

                    if (DeviceData != null) {

                        Canvas.SetLeft(DeviceData, 30);
                        Canvas.SetTop(DeviceData, 10);

                    }

                    if (Entry != null){
                        Canvas.SetLeft(Entry, 25);
                        Canvas.SetTop(Entry, (MainCanvas.Height - Entry.Height) - 25);

                    }

                    if (AcceptButton != null){
                        Canvas.SetLeft(AcceptButton, MainCanvas.Width - AcceptButton.Width - 10);
                        Canvas.SetTop(AcceptButton, MainCanvas.Height - AcceptButton.Height - 10);
                    }
                }
            }
        }


        // this uses mathmaticall equation that can be mapped to for both Transtions at once
        private void TriggerSetXPostion(double value) {
            if (Master == null) return;

            Canvas.SetLeft(this, (((Master.Width + Width) * value) - Width));


            if (value == 0 || value == 1){
                IsVisible = false;
                IsHitTestVisible = false;


                // we reset the Transtions
                TransitionIn = new Animations.Transations.EaseOut{
                    StartingValue = 0,
                    EndingValue = 0.5,
                    Duration = Config.TransitionDuration * 2,
                    Trigger = TriggerSetXPostion,
                };

                TransitionOut = new Animations.Transations.EaseOut{ // reversed so we get EaseIn function
                    StartingValue = 1,
                    EndingValue = 0.5,
                    CurrentValue = 0.5,
                    Duration = Config.TransitionDuration * 2,
                    Trigger = TriggerSetXPostion,
                };
            }
            else { 
                IsVisible = true;
                IsHitTestVisible = true;
            }
        }


        private void TriggerWrongToken(double value){
            if (Background is SolidColorBrush solidBrush){
                var originalColor = Themes.Entry;
                var targetColor = Themes.WrongToken;

                byte Lerp(byte start, byte end) => (byte)(start + (end - start) * value); // mathmaticall function

                var newColor = Color.FromArgb(
                    Lerp(originalColor.Color.A, targetColor.Color.A),
                    Lerp(originalColor.Color.R, targetColor.Color.R),
                    Lerp(originalColor.Color.G, targetColor.Color.G),
                    Lerp(originalColor.Color.B, targetColor.Color.B)
                );
                if (Entry != null) {
                    Entry.Background = new SolidColorBrush(newColor);
                }
                
                if(WrongTokenTranstion != null && 
                    WrongTokenTranstion.FunctionRunning == false && 
                    value == 1) 
                { // loop around and go back to normal
                    WrongTokenTranstion.TranslateBackward();
                }
            }
        }




        // this will be ran from a very far thread comming from the MessageManager
        // but the rule in the PublicWidgets.cs has not been broken as this is the
        // function that we put in it
        public void OnIncomingConnection() { // coming from <MessageMangaer Thread>


            if (Master != null && TransitionIn != null && SharedData.IncomingConnection.Message != null){





                // ensures that it is on the right thread before
                // updating the UI
                Dispatcher.UIThread.Post(() => {
                    if (DeviceData != null){
                        DeviceData.Text = "            Connection Request\n\n" +
                           $"Device: {SharedData.IncomingConnection.Message.DeviceName}\n" +
                           $"IP: {SharedData.IncomingConnection.Message.IP}\n" +
                           $"Time: {SharedData.IncomingConnection.Message.Time}\n";
                    }

                    if (Entry != null) {
                        Entry.Text = "";
                    }

                    TransitionIn.TranslateForward();
                    if (PublicWidgets.UIDimOverlay != null) {
                        PublicWidgets.UIDimOverlay.Show();
                    }
                }); 

            }
        }




        public void OnClickClosingButton() {
            if (TransitionIn == null) return;            
            TransitionIn.TranslateBackward();
            if (PublicWidgets.UIDimOverlay != null){
                PublicWidgets.UIDimOverlay.Hide();
            }


            if (SharedData.IncomingConnection.Message != null) {
                Connections.Manager.RejectIncomingConnection(SharedData.IncomingConnection.Message, "Decline"); // Decline the Connection
                SharedData.IncomingConnection.Clear(); // remove the the message
            }

        }


        private void OnClickAcceptButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e){

            if (Entry != null && 
                Entry.Text != null &&
                SharedData.IncomingConnection.Message != null&&
                SharedData.IncomingConnection.Message.Text != null) 
            {
                
                var DecreaptedMessge = Encryptor.Decrypt(SharedData.IncomingConnection.Message.Text, Entry.Text);

                if (DecreaptedMessge == Connections.Constants.PassPhase){

                    SharedData.IncomingConnection.Token = Entry.Text;

                    if (TransitionOut != null){
                        TransitionOut.TranslateBackward(); // rversed so we get EaseIn
                    }

                    Connections.Manager.AcceptIncomingConnection(SharedData.IncomingConnection.Message, // establish the connection
                                                                 SharedData.IncomingConnection.Token);

                    if (PublicWidgets.UIDimOverlay != null){
                        PublicWidgets.UIDimOverlay.Hide();
                    }



                }
                else {
                    if (WrongTokenTranstion != null) {
                        WrongTokenTranstion.TranslateForward();
                    }
                }
            }
        }
    }
}
