using InputConnect.Structures;
using InputConnect.SharedData;
using Avalonia.Interactivity;
using InputConnect.Setting;
using Avalonia.Controls;
using Avalonia;
using Avalonia.Media;
using InputConnect.Network;
using Avalonia.Input;








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
        private TextBox? Entry;


        private Animations.Transations.Uniform? WrongTokenTranstion;

        public Connector(Canvas? master) {

            Master = master;

            Opacity = 1;
            ClipToBounds = true;
            Background = Themes.Holder;
            CornerRadius = new CornerRadius(Config.CornerRadius);
            MinHeight = 150; MinWidth = 350;
            MaxHeight = 150; MaxWidth = 350;

            Width = 350;
            Height = 150;


            MainCanvas = new Canvas{
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
            Entry.KeyDown += OnEnetryKeyDown;


            WrongTokenTranstion = new Animations.Transations.Uniform{
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration,
                Trigger = TriggerWrongToken,
            };




            OnSizeChanged();

            if (Master != null){
                Master.PropertyChanged += OnSizeChanged;
            }

        }

        public void Update() {
            if (Entry == null) return; 

            if (TargetedDevice.Connection != null) {
                Entry.Text = TargetedDevice.Connection.Token;

            }
            else{
                Entry.Text = "";
            }
        
        }


        public void OnSizeChanged(object? sender = null, object? e = null){

            
            if (Master == null) return;


            //Width = Master.Width * 0.35;
            //Height = Master.Height * 0.25;

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


                if (Entry != null){
                    Canvas.SetLeft(Entry, 20);
                    Canvas.SetTop(Entry, (MainCanvas.Height - Entry.Height) / 3);
                }
            }

            
        }


        private void TriggerWrongToken(double value)
        {
            if (Background is SolidColorBrush solidBrush)
            {
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

                if (Entry != null)
                {
                    // changing the color of the background is better but Avilonia doesnt document the c#
                    // part of there code look into making your own text entry at  this point, prefarably
                    // in the near future switch to the orginal sloution

                    //Entry.Background = new SolidColorBrush(newColorEntry);
                    Entry.Foreground = new SolidColorBrush(newColorText);
                }

                if (WrongTokenTranstion != null &&
                    WrongTokenTranstion.FunctionRunning == false &&
                    value == 1)
                { // loop around and go back to normal
                    WrongTokenTranstion.TranslateBackward();
                }
            }
        }


        private void OnEnetryKeyDown(object? sender, KeyEventArgs? e){
            // this will  detect when  a key  is pressed  for the entery
            // this will be used to detect when the enter key is pressed
            // to trigger the Accept function

            if (e == null) return;

            if (e.Key == Key.Enter){
                OnClickConnectButton(null, null); // simulate clicking the accept button
            }
        }


        private void OnClickConnectButton(object? sender = null, RoutedEventArgs? e = null){
            if (TargetedDevice.MacAddress != null)
            {
                // only the ip address and the token are needed  MacAdress IP address  and also the token
                // the MacAddress is a must and ip address is used to send message, in thory you can have
                // the MacAddress anything but we  wont be  gettinga response  for the  message since the
                // user is going to resposnd with there own mac address

                if (Entry == null || Entry.Text == null || Entry.Text == "") {
                    
                    if (WrongTokenTranstion != null) 
                        WrongTokenTranstion.TranslateForward();

                    return;
                }


                string token = Entry.Text;
                var newConnection = Connections.Manager.EstablishConnection(MessageManager.MacToIP[TargetedDevice.MacAddress],
                                                        token,
                                                        TargetedDevice.MacAddress,
                                                        TargetedDevice.DeviceName);

                TargetedDevice.Connection = newConnection;

                if(Events.TargetDeviceConnectionChanged != null)
                    Events.TargetDeviceConnectionChanged.Invoke();

                if (PublicWidgets.UIConnections != null)
                    PublicWidgets.UIConnections.Update();

            }
        }

    }
}
