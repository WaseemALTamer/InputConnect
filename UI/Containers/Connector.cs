using InputConnect.Structures;
using InputConnect.SharedData;
using Avalonia.Interactivity;
using InputConnect.Setting;
using Avalonia.Controls;
using Avalonia;
using Avalonia.Media;








namespace InputConnect.UI.Containers
{




    public class Connector : Border
    {


        private Canvas? Master;

        private Canvas? _MainCanvas;
        public Canvas? MainCanvas{
            get { return _MainCanvas; }
            set { _MainCanvas = value; }
        }





        private Button? ConnectButton;
        private TextBox? TokenEntry;


        private Animations.Transations.Uniform? WrongTokenTranstion;

        public Connector(Canvas? master) {

            Master = master;

            Opacity = 1;
            ClipToBounds = true;
            Background = Themes.Holders;
            CornerRadius = new CornerRadius(Config.CornerRadius);
            MinHeight = 150; MinWidth = 350;
            MaxHeight = 250; MaxWidth = 450;


            MainCanvas = new Canvas
            {
                ClipToBounds=true,

            };
            Child = MainCanvas;

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


            TokenEntry = new TextBox{
                Width = 300,
                Height = 40,
                FontSize = Config.FontSize,
                CornerRadius = new CornerRadius(Config.CornerRadius),
                Watermark = "Token",
                PasswordChar = char.Parse("*"),
                Background = Themes.Entry,
            };
            MainCanvas.Children.Add(TokenEntry);



            WrongTokenTranstion = new Animations.Transations.Uniform{
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration,
                Trigger = TriggerWrongToken,
            };




            OnSizeChanged();

            if (Master != null){
                Master.SizeChanged += OnSizeChanged;
            }

        }

        public void OnSizeChanged(object? sender = null, SizeChangedEventArgs? e = null){

            if (Master == null) return;

            Width = Master.Width * 0.35;
            Height = Master.Height * 0.25;

            // check the boundries and ensure they dont go byoned
            if (Width < MinWidth)
                Width = MinWidth;
            if (Height < MinHeight)
                Height = MinHeight;

            if (Width > MaxWidth)
                Width = MaxWidth;
            if (Height > MaxHeight)
                Height = MaxHeight;


            if (MainCanvas != null){
                MainCanvas.Width = Width;
                MainCanvas.Height = Height;

                if (ConnectButton != null){
                    Canvas.SetLeft(ConnectButton, (MainCanvas.Width - ConnectButton.Width) - 10);
                    Canvas.SetTop(ConnectButton, (MainCanvas.Height - ConnectButton.Height) - 10);
                }


                if (TokenEntry != null){
                    Canvas.SetLeft(TokenEntry, 20);
                    Canvas.SetTop(TokenEntry, (MainCanvas.Height - TokenEntry.Height) / 3);
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
                if (TokenEntry != null)
                {
                    TokenEntry.Background = new SolidColorBrush(newColor);
                }

                if (WrongTokenTranstion != null &&
                    WrongTokenTranstion.FunctionRunning == false &&
                    value == 1)
                { // loop around and go back to normal
                    WrongTokenTranstion.TranslateBackward();
                }
            }
        }

        private void OnClickConnectButton(object? sender = null, RoutedEventArgs? e = null){

            if (TargetedDevice.IP != null)
            {
                // only the ip address and the token are needed  MacAdress IP address  and also the token
                // the MacAddress is a must and ip address is used to send message, in thory you can have
                // the MacAddress anything but we  wont be  gettinga response  for the  message since the
                // user is going to resposnd with there own mac address

                if (TokenEntry == null || TokenEntry.Text == null) {
                    
                    if (WrongTokenTranstion != null) 
                        WrongTokenTranstion.TranslateForward();

                    return;
                }


                string token = TokenEntry.Text;
                var newConnection = new Connection
                {
                    State = Connections.Constants.StatePending,
                    DeviceName = TargetedDevice.DeviceName,
                    MacAddress = TargetedDevice.MacAddress,
                    SequenceNumber = 0,
                    Token = token,
                };

                TargetedDevice.Connection = newConnection;

                Connections.Devices.ConnectionList.Add(newConnection); // maually add the connection to the connection list to keep track of it
                Connections.Manager.EstablishConnection(TargetedDevice.IP,
                                                        token,
                                                        TargetedDevice.MacAddress,
                                                        TargetedDevice.DeviceName);
                if (PublicWidgets.UIConnections != null)
                    PublicWidgets.UIConnections.Update();

            }
        }

    }
}
