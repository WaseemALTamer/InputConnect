using InputConnect.UI.InWindowPopup;
using InputConnect.UI.Containers;
using System.Collections.Generic;
using InputConnect.Structures;
using Avalonia.Interactivity;
using InputConnect.Network;
using Avalonia.Threading;
using Avalonia.Controls;
using Avalonia;




namespace InputConnect.UI.Pages
{
    public class Connections : Base
    {



        private List<ConnectedDevice> _Devices = new List<ConnectedDevice>();
        public List<ConnectedDevice> Devices
        {
            get { return _Devices; }
            set { _Devices = value; }
        }

        private int AdsPaddyY = 10;
        private int YPosOffset = 80;


        private NoConnectorMessage? noConnectorMessage;


        private Button? AddConnectionButton;
        private AddConnection? AddConnectionPopUP;


        public Connections(Canvas? master) : base(master){
            // we ensure that it runs on the main thread because we are working with the ui

            InputConnect.Connections.Manager.OnConnectedConnectionAdded += () => { Update(); };
            InputConnect.Connections.Manager.OnConnectionClosed += () => { Update(); };


            MessageManager.OnConnect += (message) => { Update();};
            MessageManager.OnAccept += (message) => { Update();};
            MessageManager.OnDecline += (message) => { Update();};



            if (MainCanvas == null) return;

            AddConnectionButton = new Button{
                Content = "Add Connection",
                Width = 200,
                Height = 50,
                Background = InputConnect.Setting.Themes.Button,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                FontSize = InputConnect.Setting.Config.FontSize,
                CornerRadius = new CornerRadius(InputConnect.Setting.Config.CornerRadius)
            };
            MainCanvas.Children.Add(AddConnectionButton);
            AddConnectionButton.Click += OnClickAddConnectionButton;


            if (PublicWidgets.Master != null)
            {
                AddConnectionPopUP = new AddConnection(PublicWidgets.Master);
            }

            



            MainCanvas.SizeChanged += OnSizeChanged;

            noConnectorMessage = new NoConnectorMessage(MainCanvas);
            MainCanvas.Children.Add(noConnectorMessage);
            noConnectorMessage.Show();


            Update();

            //SizeChanged += (s, e) => PlaceConnections(); // uncomment this later for redundency after testing
            OnShow += () => {Update(null, null);}; // update the size first
            OnShow += PlaceConnections; // then update the placement

            if (Master == null) return;

            

        }


        public void OnSizeChanged(object? sender = null, SizeChangedEventArgs? e = null){

            if (MainCanvas == null)
                return;

            if (noConnectorMessage != null) {
                Canvas.SetLeft(noConnectorMessage, (MainCanvas.Width - noConnectorMessage.Width) / 2);
                Canvas.SetTop(noConnectorMessage, ((MainCanvas.Height - noConnectorMessage.Height) / 2) + 150);
            }

            if (AddConnectionButton != null)
            {
                Canvas.SetLeft(AddConnectionButton, (MainCanvas.Width - AddConnectionButton.Width) / 2);
                Canvas.SetTop(AddConnectionButton, 20);
            }


            // this is a fix for a bug, when switching to a different page  changing the  size of the window and then
            // going back to this window we find that the connection containers are offseted, note that when resizing
            // it is not as smooth as the advertisement resize not instant, come back to  this  issue but  it is none
            // critical 
            foreach (var _connection in Devices)
            {
                if (_connection != null)
                {
                    Canvas.SetLeft(_connection, (MainCanvas.Width - _connection.Width) / 2);
                }
            }


        }




        public void Update(object? sender = null, object? e = null){

            Dispatcher.UIThread.Post(() => { 
                for (int i = 0; i < InputConnect.Connections.Devices.ConnectionList.Count; i++){
                    var _found = false; // this will be used to indecated if we found the device responsible for the message
                    var device = InputConnect.Connections.Devices.ConnectionList[i];
                    for (int j = 0; j < Devices.Count; j++){
                        var UIobject = Devices[j];
                        if (UIobject == null || UIobject.Device == null) continue;
                        if (UIobject.Device.MacAddress == device.MacAddress){
                            UIobject.Device = device;
                            UIobject.Update(); // update it for values inside of it
                            _found = true;
                            break;
                        }
                    }
                    if (_found) continue;

                    Add(device);
                }

                // now we can check for any devices that we have that are not in the advertisement
                for (int i = Devices.Count - 1; i >= 0; i--){
                    var _found = false;
                    var connection = Devices[i];
                    if (connection == null || connection.Device == null) continue;
                    for (int j = 0; j < InputConnect.Connections.Devices.ConnectionList.Count; j++){
                        var _device = InputConnect.Connections.Devices.ConnectionList[j];
                        if (connection.Device == _device){
                            _found = true;
                            break;
                        }
                    }
                    if (_found) continue;
                    connection.Kill();
                    Devices.Remove(connection);
                }

                PlaceConnections();


                if (noConnectorMessage != null) {
                    if (Devices.Count == 0){
                        noConnectorMessage.Show();
                    }
                    else{
                        noConnectorMessage.Hide();
                    }
                }

            });
        }


        public void Add(Connection device){
            if (Devices != null){
                var _ad = new ConnectedDevice(MainCanvas, device);
                Devices.Add(_ad);
                if (MainCanvas != null)
                {
                    MainCanvas.Children.Add(_ad);
                }
                PlaceConnections();
            }
        }

        private void PlaceConnections()
        {
            if (Devices == null || MainCanvas == null) return;

            int _index = 0;
            int _lostindex = 0;
            foreach (var _connection in Devices)
            {
                if (_connection != null)
                {
                    //Canvas.SetRight(_connection, (MainCanvas.Width - _connection.Width) / 2);
                    //Canvas.SetTop(_connection, AdsPaddyY + (_connection.Height + AdsPaddyY) * (_index - _lostindex));

                    _connection.SetPostionTranslate((MainCanvas.Width - _connection.Width) / 2, 
                                                        (AdsPaddyY + (_connection.Height + AdsPaddyY) * (_index - _lostindex)) + YPosOffset);

                    var height = AdsPaddyY + (_connection.Height + AdsPaddyY) * (_index - _lostindex) + (_connection.Height + AdsPaddyY);
                    if (height >= Height){
                        MainCanvas.Height = height;
                    }
                    else{
                        MainCanvas.Height = Height;
                    }
                }
                else
                {
                    _lostindex++;
                }
                _index++;
            }
            ShowOnlyVissibleConnections();
        }



        // this function sill needs more work as of now it shows everything that is in the array
        // you should later on make it only show ones that are displayed on screen
        private void ShowOnlyVissibleConnections()
        {
            if (Devices == null) return;

            int _index = 0;
            int _lostindex = 0;
            foreach (var _advertisement in Devices)
            {
                if (_advertisement != null)
                {
                    _advertisement.Show();
                }
                else
                {
                    _lostindex++;
                }
                _index++;
            }
        }


        private void OnClickAddConnectionButton(object? sender = null, RoutedEventArgs? e = null)
        {
            if (AddConnectionPopUP == null) return;

            AddConnectionPopUP.Show();

        }

    }
}
