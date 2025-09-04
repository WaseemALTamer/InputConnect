using InputConnect.Structures;
using InputConnect.Network;
using Avalonia.Threading;
using System.Text.Json;
using Avalonia.Input;
using System;




namespace InputConnect.Controllers
{
    public static class ClipBoard
    {

        // this is going to be used with the avalonia framework to detect
        // and sync the clipBoard of your Device with  other  device that
        // the cursor is connected to, note that this will  only sync the
        // text giving by windows, file sync should  be done over TCP for
        // a later update, the TCP is giong to be  established  with  our
        // current UDP connection

        // this file will be build on top of the mouse Connection, and it
        // is going to work  for both Transmitting part and Reciving part
        // regradless of what your mouse connection type is



        // we will use the avalonia frame work in  order to interact with
        // the clipboard if this changes and you move  to  another  frame 
        // work  then  most  of  the  logic  for  this needs to change to
        // accomodate the output that comes out of this file


        public static bool ActiveTransmition = true;
        public static bool ActiveReceiving = true;

        private static DispatcherTimer? dispatcherTimer;

        public static void Start() {


            Network.MessageManager.OnCommandClipBoard += OnReceiveCommand;

            /// create the dispach timer
            dispatcherTimer = new DispatcherTimer{
                Interval = TimeSpan.FromMilliseconds(Setting.Config.Tick)
            };

            dispatcherTimer.Tick += Update;

            dispatcherTimer.Start();

        }


        // this needs to be attacked to the Global Updator if the Global Updator idea
        // got pushed in for now triggere the function  manually and run it through a
        // while loop with







        public static string? LatestClipBoardMessage = null;
        public static void Update(object? sender, EventArgs? e) {


            if (SharedData.Device.TopLevel == null || SharedData.Device.TopLevel.Clipboard == null) return;

            if (ActiveTransmition == false) return;



            Dispatcher.UIThread.Post(async () => {
                var text = await SharedData.Device.TopLevel.Clipboard.GetTextAsync();
                if (text != null && LatestClipBoardMessage != text){
                    LatestClipBoardMessage = text;
                    TransmitClipBoard(text);
                }
            });


        }

        

        public static void AddToClipBoard(string text) {
            if (SharedData.Device.TopLevel == null || SharedData.Device.TopLevel.Clipboard == null) return;

            if (ActiveTransmition == false) return;

            Dispatcher.UIThread.Post(async () => {
                var dataObject = new DataObject();
                dataObject.Set(DataFormats.Text, text);
                await SharedData.Device.TopLevel.Clipboard.SetDataObjectAsync(dataObject);
                LatestClipBoardMessage = text;
                TransmitClipBoard(text); // transmit the clipboard back to the other devices that can be connected
                                         // and the device that sent the clipboard will ignore the clipboard
            });
        }


        public static void TransmitClipBoard(string text) {
            // this function is going to transmit a text to the other devices to store it in
            // the clipboard


            if (ActiveTransmition == false) return;

            foreach (var connection in Connections.Devices.ConnectionList) {

                if (connection.MouseState == null) continue;

                var _command = new Commands.ClipBoard{
                    Text = text
                };

                var _commandMessage = new MessageCommand{
                    Type = Commands.Constants.CommandTypes.ClipBoard,
                    SequenceNumber = connection.SequenceNumber + 1,
                    Command = JsonSerializer.Serialize(_command)
                };
                connection.SequenceNumber += 1;

                var messageudp = new MessageUDP{
                    MessageType = Network.Constants.MessageTypes.Command,
                    Text = Encryptor.Encrypt(JsonSerializer.Serialize(_commandMessage), connection.PasswordKey),
                    IsEncrypted = true
                };

                if (connection.MacAddress != null &&
                    MessageManager.MacToIP.TryGetValue(connection.MacAddress, out var ip))
                {
                    ConnectionUDP.Send(ip, messageudp);
                }
            }

        }


        public static void OnReceiveCommand(Commands.ClipBoard command) {

            if (ActiveReceiving == false) return;
            if (command.Text == null) return;

            if (LatestClipBoardMessage == command.Text)
                return;

            AddToClipBoard(command.Text);

        }


    }
}
