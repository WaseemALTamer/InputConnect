using Avalonia.Interactivity;
using InputConnect.Setting;
using Avalonia.Controls;
using Avalonia;







namespace InputConnect.UI.Pages
{
    // this file will only provide the structure of the canvase and how it looks like
    // it will also provide the tools to add and remove  the  children  of it none of
    // its logic will be ran through it

    // this file ended also running the logic as well

    public class Device : Base
    {



        private Button? ConnectButton;

        public Device(Canvas? master) : base(master)
        {

            if (MainCanvas == null) return;


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





            OnResize(); // trigger the function to set the sizes

            if (Master != null) {
                Master.SizeChanged += OnResize;
            }
        }


        public void OnResize(object? sender = null, SizeChangedEventArgs? e = null){

            if (MainCanvas != null){
                MainCanvas.Width = Width; 
                MainCanvas.Height = Height;

                if (ConnectButton != null) {
                    Canvas.SetLeft(ConnectButton, (MainCanvas.Width - ConnectButton.Width) - 10);
                    Canvas.SetTop(ConnectButton, (MainCanvas.Height - ConnectButton.Height) - 10);
                }
            }
        }



        private void OnClickConnectButton(object? sender = null, RoutedEventArgs? e = null) {

            if (SharedData.TargetedDevice.IP != null) {
                // only the ip address and the token are needed  MacAdress IP address  and also the token
                // the MacAddress is a must and ip address is used to send message, in thory you can have
                // the MacAddress anything but we  wont be  gettinga response  for the  message since the
                // user is going to resposnd with there own mac address
                InputConnect.Connections.Manager.EstablishConnection(SharedData.TargetedDevice.IP, 
                                                                    "<Token>", 
                                                                    SharedData.TargetedDevice.MacAddress,
                                                                    SharedData.TargetedDevice.DeviceName);
            }
        }

    }
}
