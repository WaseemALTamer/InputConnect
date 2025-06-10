using InputConnect.Network;
using InputConnect.Setting;
using Avalonia.Threading;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia;



namespace InputConnect.UI.WindowPopup
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






        public ConnectionReplay(Canvas master) : base(master)
        {
            if (MainCanvas == null) return;

            // CloseButton.Trigger  // attach the trigger on the function

            WrongTokenTranstion = new Animations.Transations.Uniform{
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration,
                Trigger = TriggerWrongToken,
            };

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


            MainCanvas.SizeChanged += OnResize;

            CloseButtonTrigger += OnCloseButton;

            Child = MainCanvas;

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
                }
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


        private void OnClickAcceptButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e){

            if (Entry != null && 
                Entry.Text != null &&
                SharedData.IncomingConnection.Message != null&&
                SharedData.IncomingConnection.Message.Text != null) 
            {
                
                var DecreaptedMessge = Encryptor.Decrypt(SharedData.IncomingConnection.Message.Text, Entry.Text);

                if (DecreaptedMessge == Connections.Constants.PassPhase){

                    SharedData.IncomingConnection.Token = Entry.Text;
                    HideRight();

                    Connections.Manager.AcceptIncomingConnection(SharedData.IncomingConnection.Message, // establish the connection
                                                                 SharedData.IncomingConnection.Token);

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
        }

        private void OnCloseButton() {

            // this function is going to be wrapped around and feed into the popup base

            if (SharedData.IncomingConnection.Message != null){
                Connections.Manager.RejectIncomingConnection(SharedData.IncomingConnection.Message, "Decline"); // Decline the Connection
                SharedData.IncomingConnection.Clear(); // remove the the message
            }
        }




    }
}
