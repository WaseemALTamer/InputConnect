using InputConnect.Network;
using InputConnect.Setting;
using Avalonia.Threading;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia;
using System;
using InputConnect.Structures;







namespace InputConnect.UI.InWindowPopup
{

    public class ConnectionReplay : Base
    {

        // this class is a simple pop up that will appear when someone tries to connect to you
        // the main perpose of this class is for you to add the token and connect to the other
        // device  both tokens should  be the  same on both computers  for  a pop up  the main
        // border is always just a full screen window which darkens everytihng beneath it





        private Animations.Transations.Uniform? WrongTokenTranstion;
        private TextBlock? DeviceData;
        private TextBox? Entry;
        private Button? AcceptButton;
        private Containers.Timer? TimeoutTimer; // this will help close the popup if it was not responded to






        public ConnectionReplay(Canvas master) : base(master)
        {
            if (MainCanvas == null) return;

            // CloseButton.Trigger  // attach the trigger on the function

            Connections.Manager.ActionOnIncomingConnection += OnIncomingConnection;
            

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
                Foreground = Themes.Text
            };
            MainCanvas.Children.Add(Entry);
            Entry.KeyDown += OnEnetryKeyDown;



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


            WrongTokenTranstion = new Animations.Transations.Uniform{
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration,
                Trigger = TriggerWrongToken,
            };


            TimeoutTimer = new Containers.Timer{
                Width = 40,
                Height = 40,
                Thickness = 10,
                Trigger = OnCloseButton, // this will simulate closing the popup when the time ends
            };

            MainCanvas.Children.Add(TimeoutTimer);
            OnShowTrigger = OnShow; // now that we maped the function we can start the timer in it
                                    // which will only start it exactly when the popup is shown
            
            OnHideTrigger = OnHide; // i used this approch so i dont have to send two decline messages
                                    // one from the timer and one from clicking the  button  note that
                                    // the issue only happens if you  click the  button so  we need to
                                    // pause the timer before then


            MainCanvas.SizeChanged += OnResize;

            CloseButtonTrigger += OnCloseButton;

            Child = MainCanvas;

        }

        public void OnShow() {
            if (TimeoutTimer != null) {
                TimeoutTimer.StartTimer(Config.PopupTimeout);
            }
        }

        public void OnHide(){
            if (TimeoutTimer != null){
                TimeoutTimer.Pause();
            }
        }

        public void OnResize(object? sender = null, SizeChangedEventArgs? e = null)
        {
            if (Master != null){
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

                    if (TimeoutTimer != null){
                        Canvas.SetLeft(TimeoutTimer, MainCanvas.Width - TimeoutTimer.Width - 10);
                        Canvas.SetTop(TimeoutTimer, ((MainCanvas.Height - TimeoutTimer.Height) / 2) - 10);
                    }
                }
            }
        }



        private void TriggerWrongToken(double value){
            if (Background is SolidColorBrush solidBrush){
                var originalColorEntry = Themes.Entry;
                var targetColor = Themes.WrongToken;

                byte Lerp(byte start, byte end) => (byte)(start + (end - start) * value);

                //var newColorEntry = Color.FromArgb(
                //    Lerp(originalColorEntry.Color.A, targetColor.Color.A),
                //    Lerp(originalColorEntry.Color.R, targetColor.Color.R),
                //    Lerp(originalColorEntry.Color.G, targetColor.Color.G),
                //    Lerp(originalColorEntry.Color.B, targetColor.Color.B)
                //);

                var originalColorText = Themes.Text;

                var newColorText = Color.FromArgb(
                    Lerp(originalColorText.Color.A, targetColor.Color.A),
                    Lerp(originalColorText.Color.R, targetColor.Color.R),
                    Lerp(originalColorText.Color.G, targetColor.Color.G),
                    Lerp(originalColorText.Color.B, targetColor.Color.B)
                );

                if (Entry != null) {
                    // changing the color of the background is better but Avilonia doesnt document the c#
                    // part of there code look into making your own text entry at  this point, prefarably
                    // in the near future switch to the orginal sloution

                    //Entry.Background = new SolidColorBrush(newColorEntry);
                    Entry.Foreground = new SolidColorBrush(newColorText);
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
        public void OnIncomingConnection() { 
            // coming from <MessageMangaer Thread>

            if (SharedData.IncomingConnection.Message != null){
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

                    Show();
                }); 

            }
        }

        private void OnEnetryKeyDown(object? sender, KeyEventArgs? e){
            // this will  detect when  a key  is pressed  for the entery
            // this will be used to detect when the enter key is pressed
            // to trigger the Accept function

            if (e == null) return;

            if (e.Key == Key.Enter){
                OnClickAcceptButton(null, null); // simulate clicking the accept button
            }
        }


        private void OnClickAcceptButton(object? sender, object? e){

            if (Entry != null && 
                Entry.Text != null &&
                SharedData.IncomingConnection.Message != null&&
                SharedData.IncomingConnection.Message.Text != null) 
            {
                
                var DecreaptedMessge = Encryptor.Decrypt(SharedData.IncomingConnection.Message.Text, new Structures.PasswordKey(Entry.Text));

                if (DecreaptedMessge == Connections.Constants.PassPhase){

                    SharedData.IncomingConnection.PasswordKey = new PasswordKey(Entry.Text);

                    if (TimeoutTimer != null)
                        TimeoutTimer.Pause();

                    HideRight();

                    var newConnection = Connections.Manager.AcceptIncomingConnection(SharedData.IncomingConnection.Message, // establish the connection
                                                                                     SharedData.IncomingConnection.PasswordKey);
                    



                    if (PublicWidgets.UIDevice != null) {

                        SharedData.TargetedDevice.Clear();
                        SharedData.TargetedDevice.Connection = newConnection;

                        SharedData.TargetedDevice.MacAddress = SharedData.IncomingConnection.Message.MacAddress;
                        SharedData.TargetedDevice.DeviceName = SharedData.IncomingConnection.Message.DeviceName;

                        PublicWidgets.TransitionForward(PublicWidgets.UIDevice);
                    }

                    SharedData.IncomingConnection.Clear();

                    if (Global.Overlay != null){
                        Global.Overlay.Hide();
                    }

                }
                else {
                    if (WrongTokenTranstion != null) {
                        WrongTokenTranstion.TranslateForward();
                    }
                }
            }

            if (PublicWidgets.UIConnections != null)
                PublicWidgets.UIConnections.Update();

            
        }

        private void OnCloseButton() {
            // this function is going to be wrapped around and feed into the popup base
            Hide(); // hide it again for any other function that is trying to simulate the Closing button functionality

            if (SharedData.IncomingConnection.Message != null){
                Connections.Manager.CloseIncomingConnection(SharedData.IncomingConnection.Message, "Decline"); // Decline the Connection
                SharedData.IncomingConnection.Clear(); // remove the the message
            }

            if (PublicWidgets.UIConnections != null)
                PublicWidgets.UIConnections.Update();
        }




    }
}
