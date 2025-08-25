using InputConnect.Structures;
using System.Text.Json;
using SharpHook;
using System;
using InputConnect.Network;
using System.Threading;
using Avalonia.Platform;
using System.Reflection.Metadata.Ecma335;
using Avalonia.Rendering.Composition;
using System.Security.Cryptography.X509Certificates;
using Avalonia;
using Tmds.DBus.Protocol;
using Avalonia.Controls;






namespace InputConnect.Controllers
{
    public static class Mouse
    {
        // this file is going to be responisble to tracking down the mouse knows where it is
        // and checks if the mouse postion should be tracked or not and try to send the data
        // to the relivent devices


        // this will make a refrence of the GlobalHook The global hook
        public static TaskPoolGlobalHook Hook = Controllers.Hook.GlobalHook;
        public static EventSimulator EventSimulator = Controllers.Hook.GlobalEventSimulator;


        public static Action<double, double>? OnMove; // => <x, y>
        public static Action<int>? OnMousePress; // => 0:None -> 1:Left -> 2:Right -> 3:Middle -> 4:Back -> 5:Forward
        public static Action<int>? OnMouseRealse; // => 0:None -> 1:Left -> 2:Right -> 3:Middle -> 4:Back -> 5:Forward
        public static Action<double>? OnMouseScroll; // => <delta>


        public static double? VirtualPositionX;
        public static double? VirtualPositionY;


        public static double? CurrentPositionX;
        public static double? CurrentPositionY;
        public static double? DeltaX;
        public static double? DeltaY;


        public static double CriticalRegionSize = 1;
        public static double CursorThrow = CriticalRegionSize * 2;
        public static bool IsVirtualCoordinates = false;
        public static Bounds? CurrentScreenBounds;


        public static Action? OnVirtualMointorEnter;
        public static Action? OnVirtualMointorExit;

        public static bool MouseLock = false;


        //public static double MouseSkipPosX = 0;
        //public static double MouseSkipPosY = 0;
        public static bool SkipNextMouseHook = false; // this will be used to skip the next mouse detection incase you want
                                                      // to move the mouse through code and dont want it to be detected

        public static int SaftyNet = 100; // if the mouse suddnly goes more than 100 pixles at a time we most likely have an error
                                             // which we need to correct before we can move forward



        // this Start function should be ran after the hook is ran
        public static bool Start() {
            Hook.MouseMoved += OnMoveAction;
            Hook.MouseDragged += OnMoveAction;



            Hook.MousePressed += (_, e) => {
                OnMousePress?.Invoke((int)e.Data.Button);
            };

            Hook.MouseReleased += (_, e) => {
                OnMouseRealse?.Invoke((int)e.Data.Button);
            };

            Hook.MouseWheel += (_, e) => {
                OnMouseScroll?.Invoke(e.Data.Delta * e.Data.Rotation / 360);
            };


            OnMove += TrackMouse;
            //OnMove += TransmitMouseMovement;

            //OnMousePress += TransmitMousePressButtons;
            //OnMouseRealse += TransmitMouseReleaseButtons;
            //OnMouseScroll += TransmitMouseScroll;


            MessageManager.OnCommandMouse += RecieveMouseCommnad;



            return true;
        }


        private static void OnMoveAction(object? sender, MouseHookEventArgs e) {

            if (MouseLock) return;

            DeltaX = (e.Data.X - CurrentPositionX) ?? 0;
            DeltaY = (e.Data.Y - CurrentPositionY) ?? 0;

            CurrentPositionX = e.Data.X;
            CurrentPositionY = e.Data.Y;

            if (SkipNextMouseHook){
                return;
            }

            //TrackMouse(e.Data.X, e.Data.Y);

            //Console.WriteLine($"{DeltaX}, {DeltaY}");
            OnMove?.Invoke(e.Data.X, e.Data.Y);
        }

        public static void MoveMouse(double x, double y) {
            EventSimulator.SimulateMouseMovement((short)x, (short)y);
        }

        public static void PressMouse(int button) {
            // => 0:None -> 1:Left -> 2:Right -> 3:Middle -> 4:Back -> 5:Forward
            EventSimulator.SimulateMousePress((SharpHook.Data.MouseButton)button);
        }

        public static void ReleaseMouse(int button) {
            // => 0:None -> 1:Left -> 2:Right -> 3:Middle -> 4:Back -> 5:Forward
            EventSimulator.SimulateMouseRelease((SharpHook.Data.MouseButton)button);
        }

        public static void ScrollMouse(double delta) {
            EventSimulator.SimulateMouseWheel((short)(delta));
        }



        public static void TrackMouse(double x, double y) {
            // this function will track the mouse through the virtual boarders and 
            // through the physical ones as well this will store the values of the
            // virtual pos
            bool result = ValidatePos(x, y);
            if (!result) {
                //Console.WriteLine($"Wrong Result");
                return;
            }

            if (VirtualPositionX == null || VirtualPositionY == null) {
                VirtualPositionX = x;
                VirtualPositionY = y;
            }

            double oldVritualPosX = (double)VirtualPositionX;
            double oldVritualPosY = (double)VirtualPositionY;

            // check the crtical regions and send the mouse to the secreen if possible
            bool touchedBoarder = false;
            foreach (var connection in Connections.Devices.ConnectionList) {
                
                if (connection != null &&
                    connection.VirtualScreens != null &&
                    connection.MouseState == Connections.Constants.Transmit)
                {
                    
                    
                    foreach (var screen in connection.VirtualScreens) {
                        if (Math.Abs((decimal)(screen.X - VirtualPositionX)) < (decimal)CriticalRegionSize) {
                            VirtualPositionX = screen.X + CursorThrow;
                            touchedBoarder = true;
                            break;
                        }

                        if (Math.Abs((decimal)((screen.X + screen.Width) - VirtualPositionX)) < (decimal)CriticalRegionSize) {
                            VirtualPositionX = (screen.X + screen.Width) - CursorThrow;
                            touchedBoarder = true;
                            break;
                        }

                        if (Math.Abs((decimal)(screen.Y - VirtualPositionY)) < (decimal)CriticalRegionSize) {
                            VirtualPositionY = screen.Y + CursorThrow;
                            touchedBoarder = true;
                            break;
                        }

                        if (Math.Abs((decimal)((screen.Y + screen.Height) - VirtualPositionY)) < (decimal)CriticalRegionSize) {
                            VirtualPositionY = (screen.Y + screen.Height) - CursorThrow;
                            touchedBoarder = true;
                            break;
                        }

                        if (touchedBoarder) break;

                    }
                }
            }


            result = ValidatePos(VirtualPositionX, VirtualPositionY); // this will check our postion if it exist through both
                                                                      // virtual and physical screens
            //Console.WriteLine($"VirtualPos => {VirtualPositionX}, {VirtualPositionY}");

            if (result){

                if (IsVirtualCoordinates &&
                    OnVirtualMointorEnter != null)
                {
                    if (SharedData.Device.Screens == null) return;

                    var bounds = SharedData.Device.Screens[0].Bounds;

                    

                    OnVirtualMointorEnter();
                }
                else {

                    if (Math.Abs((decimal)(VirtualPositionX - x)) > SaftyNet || 
                        Math.Abs((decimal)(VirtualPositionY - y)) > SaftyNet) 
                    { 
                        MoveMouse((double)VirtualPositionX, (double)VirtualPositionY);
                        return;
                    }


                    VirtualPositionX = x; 
                    VirtualPositionY = y;
                }

            }
            else {
                VirtualPositionX  = oldVritualPosX;
                VirtualPositionY  = oldVritualPosY;
            }
        }



        private static bool HasCrossedValue(
            double newValue,
            double delta,
            double target,
            bool direction)
        {
            double oldValue = newValue - delta;

            if (direction && delta > 0)
            {
                // Moving right: did we cross from < target to inside [target, target + region]?
                return (oldValue < target && newValue >= target) ||
                       (oldValue < target + CriticalRegionSize && newValue >= target + CriticalRegionSize);
            }
            else if (!direction && delta < 0)
            {
                // Moving left: did we cross from > target to inside [target - region, target]?
                return (oldValue > target && newValue <= target) ||
                       (oldValue > target - CriticalRegionSize && newValue <= target - CriticalRegionSize);
            }

            return false;
        }



        public static bool ValidatePos(double? X, double? Y)
        {

            if (X == null || Y == null) return false;

            // this is for the virtual display
            foreach (var connection in Connections.Devices.ConnectionList)
            {
                if (connection != null &&
                    connection.VirtualScreens != null &&
                    connection.MouseState == Connections.Constants.Transmit)
                {
                    foreach (var screen in connection.VirtualScreens)
                    {
                        //Console.WriteLine($"{screen.Y}, {screen.Y + screen.Height}, {Y},");
                        if (screen.X <= X && screen.X + screen.Width >= X &&
                            screen.Y <= Y && screen.Y + screen.Height >= Y)
                        {
                            IsVirtualCoordinates = true; // once this is set to be true the only way to make it false
                                                         // is through the avalonia fraome work through the ui

                            MouseLock = true; // same with this varable

                            Controllers.Hook.TargetConnection = connection;
                            return true;
                        }
                    }
                }
            }

            if (SharedData.Device.Screens == null) return false;

            foreach (var screen in SharedData.Device.Screens){
                if (screen.Bounds.X <= X && screen.Bounds.X + screen.Bounds.Width >= X &&
                    screen.Bounds.Y <= Y && screen.Bounds.Y + screen.Bounds.Height >= Y)
                {
                    Controllers.Hook.TargetConnection = null;
                    return true;
                }
            }


            return false;

        }


        public static bool IsPosOnPhysicalOnScreen(double X, double Y) {
            if (SharedData.Device.Screens == null) return false;

            foreach (var screen in SharedData.Device.Screens){
                if (screen.Bounds.X <= X && screen.Bounds.X + screen.Bounds.Width >= X &&
                    screen.Bounds.Y <= Y && screen.Bounds.Y + screen.Bounds.Height >= Y)
                {
                    Controllers.Hook.TargetConnection = null;
                    return true;
                }
            }

            return false;
        }


        public static void TransmitMouseMovement(double x, double y) {
            // this function will only ransmit the movment if you are at
            // the right cordinates for the screen

            var xPos = x;
            var yPos = y;

            if (IsVirtualCoordinates == false) return;

            foreach (var connection in Connections.Devices.ConnectionList) {
                if (connection.MouseState == Connections.Constants.Transmit) {

                    if (connection.VirtualScreens == null) return;

                    for (int i = 0; i < connection.VirtualScreens.Count; i++){

                        var screen = connection.VirtualScreens[i];

                        if (connection.Screens == null || connection.Screens[i] == null) continue;

                        var physicalScreen = connection.Screens[i]; // this is the other device actual screen which
                                                                    // contain the coordinates and size

                        if (screen.X <= xPos && (screen.X + screen.Width) >= xPos &&
                            screen.Y <= yPos && (screen.Y + screen.Height) >= yPos)
                        {


                            double realPosX = (double)(physicalScreen.X + xPos - screen.X);
                            double realPosY = (double)(physicalScreen.Y + yPos - screen.Y);

                            var _command = new Commands.Mouse{
                                X = realPosX,
                                Y = realPosY,
                            };

                            var _commandMessage = new MessageCommand{
                                Type = Commands.Constants.CommandTypes.Mouse,
                                SequenceNumber = connection.SequenceNumber + 1,
                                Command = JsonSerializer.Serialize(_command)
                            };
                            connection.SequenceNumber += 1;

                            var messageudp = new MessageUDP{
                                MessageType = Network.Constants.MessageTypes.Command,
                                Text = Encryptor.Encrypt(JsonSerializer.Serialize(_commandMessage), connection.Token),
                                IsEncrypted = true
                            };

                            if (connection.MacAddress != null){
                                ConnectionUDP.Send(MessageManager.MacToIP[connection.MacAddress], messageudp);
                            }

                            //Console.WriteLine($"{realPosX}, {realPosY}");
                        }
                    }

                }
            }
        }



        public static void TransmitMousePressButtons(double x, double y, int Button){
            // note that the x and y pos is not used for telling the other device to go to 
            // the postion it simply there to see which screen it should send the click to


            var xPos = x;
            var yPos = y;


            foreach (var connection in Connections.Devices.ConnectionList){
                if (connection.MouseState == Connections.Constants.Transmit){

                    if (connection.VirtualScreens == null) return;

                    foreach (var screen in connection.VirtualScreens){

                        if (screen.X <= xPos && (screen.X + screen.Width) >= xPos &&
                            screen.Y <= yPos && (screen.Y + screen.Height) >= yPos)
                        {


                            var _command = new Commands.Mouse{
                                MouseButtonPress = Button,
                            };

                            var _commandMessage = new MessageCommand{
                                Type = Commands.Constants.CommandTypes.Mouse,
                                SequenceNumber = connection.SequenceNumber + 1,
                                Command = JsonSerializer.Serialize(_command)
                            };
                            connection.SequenceNumber += 1;

                            var messageudp = new MessageUDP{
                                MessageType = Network.Constants.MessageTypes.Command,
                                Text = Encryptor.Encrypt(JsonSerializer.Serialize(_commandMessage), connection.Token),
                                IsEncrypted = true
                            };

                            if (connection.MacAddress != null){
                                ConnectionUDP.Send(MessageManager.MacToIP[connection.MacAddress], messageudp);
                            }

                        }
                    }

                }
            }

        }


        public static void TransmitMouseReleaseButtons(double x, double y, int Button)
        {
            // note that the x and y pos is not used for telling the other device to go to
            // the postion it simply there to see which screen it should send the click to

            var xPos = x;
            var yPos = y;


            foreach (var connection in Connections.Devices.ConnectionList){
                if (connection.MouseState == Connections.Constants.Transmit){

                    if (connection.VirtualScreens == null) return;

                    foreach (var screen in connection.VirtualScreens){

                        if (screen.X <= xPos && (screen.X + screen.Width) >= xPos &&
                            screen.Y <= yPos && (screen.Y + screen.Height) >= yPos)
                        {


                            var _command = new Commands.Mouse{
                                MouseButtonRelease = Button,
                            };

                            var _commandMessage = new MessageCommand{
                                Type = Commands.Constants.CommandTypes.Mouse,
                                SequenceNumber = connection.SequenceNumber + 1,
                                Command = JsonSerializer.Serialize(_command)
                            };
                            connection.SequenceNumber += 1;

                            var messageudp = new MessageUDP{
                                MessageType = Network.Constants.MessageTypes.Command,
                                Text = Encryptor.Encrypt(JsonSerializer.Serialize(_commandMessage), connection.Token),
                                IsEncrypted = true
                            };

                            if (connection.MacAddress != null){
                                ConnectionUDP.Send(MessageManager.MacToIP[connection.MacAddress], messageudp);
                            }

                        }
                    }

                }
            }

        }




        public static void TransmitMouseScroll(double x, double y, double delta){


            // x and y are there to check if you are on the right coordinates


            var xPos = x;
            var yPos = y;


            foreach (var connection in Connections.Devices.ConnectionList)
            {
                if (connection.MouseState == Connections.Constants.Transmit)
                {

                    if (connection.VirtualScreens == null) return;

                    foreach (var screen in connection.VirtualScreens)
                    {

                        if (screen.X <= xPos && (screen.X + screen.Width) >= xPos &&
                            screen.Y <= yPos && (screen.Y + screen.Height) >= yPos)
                        {


                            var _command = new Commands.Mouse{
                                ScrollDelta = delta,
                            };

                            var _commandMessage = new MessageCommand{
                                Type = Commands.Constants.CommandTypes.Mouse,
                                SequenceNumber = connection.SequenceNumber + 1,
                                Command = JsonSerializer.Serialize(_command)
                            };
                            connection.SequenceNumber += 1;

                            var messageudp = new MessageUDP{
                                MessageType = Network.Constants.MessageTypes.Command,
                                Text = Encryptor.Encrypt(JsonSerializer.Serialize(_commandMessage), connection.Token),
                                IsEncrypted = true
                            };

                            if (connection.MacAddress != null){
                                ConnectionUDP.Send(MessageManager.MacToIP[connection.MacAddress], messageudp);
                            }

                        }
                    }

                }
            }

        }







        public static void RecieveMouseCommnad(Commands.Mouse? command) {

            if (command == null) return;

            if (command.X != null && command.Y != null){
                SkipNextMouseHook = true;
                MoveMouse((double)command.X, (double)command.Y);
            }

            if (command.MouseButtonPress != null) {
                PressMouse((int)command.MouseButtonPress);
            }


            if (command.MouseButtonRelease != null){
                ReleaseMouse((int)command.MouseButtonRelease);
            }


            if (command.ScrollDelta != null){
                ScrollMouse((double)command.ScrollDelta);
            }

        }

    }
}