using InputConnect.Structures;
using System.Text.Json;
using SharpHook;
using System;
using InputConnect.Network;
using System.Threading;
using Avalonia;






namespace InputConnect.Controllers.Mouse
{
    public static class GlobalMouse
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


        public static double CriticalRegionSize = 2;
        public static double CursorThrow = CriticalRegionSize + 1;
        public static Bounds? CurrentScreenBounds;


        public static Action? OnVirtualMointorEnter;
        public static Action? OnVirtualMointorExit;

        public static bool IsMouseTracking = true; // when setting this to false make sure you track the mouse thruogh
                                                   // a window at that point and only the window  can set this back to
                                                   // true


        //public static double MouseSkipPosX = 0;
        //public static double MouseSkipPosY = 0;
        private static System.Timers.Timer? skipResetTimer;
        public static bool SkipNextMouseHook = false; // this will be used to skip the next mouse detection incase you want
                                                      // to move the mouse through code and dont want it to be detected

        public static int SaftyNet = 300; // if the mouse suddnly goes more than 100 pixles at a time we most likely have an error
                                          // which we need to correct before we can move forward



        // this Start function should be ran after the hook is ran
        public static bool Start()
        {



            OnVirtualMointorEnter += () => IsMouseTracking = false;
            OnVirtualMointorExit += () => IsMouseTracking = true;



            Hook.MouseMoved += OnMoveAction;
            Hook.MouseDragged += OnMoveAction;


            Hook.MousePressed += (_, e) =>
            {
                OnMousePress?.Invoke((int)e.Data.Button);
            };

            Hook.MouseReleased += (_, e) =>
            {
                OnMouseRealse?.Invoke((int)e.Data.Button);
            };

            Hook.MouseWheel += (_, e) =>
            {
                OnMouseScroll?.Invoke(e.Data.Delta * e.Data.Rotation / 360);
            };


            OnMove += TrackMouse;

            MessageManager.OnCommandMouse += RecieveMouseCommnad;



            return true;
        }


        private static void OnMoveAction(object? sender, MouseHookEventArgs e)
        {
            if (IsMouseTracking == false) return;

            DeltaX = e.Data.X - CurrentPositionX ?? 0;
            DeltaY = e.Data.Y - CurrentPositionY ?? 0;

            CurrentPositionX = e.Data.X;
            CurrentPositionY = e.Data.Y;

            if (SkipNextMouseHook)
            {
                return;
            }

            OnMove?.Invoke(e.Data.X, e.Data.Y);
        }

        public static void MoveMouse(double x, double y)
        {
            EventSimulator.SimulateMouseMovement((short)x, (short)y);
        }

        public static void PressMouse(int button)
        {
            // => 0:None -> 1:Left -> 2:Right -> 3:Middle -> 4:Back -> 5:Forward
            EventSimulator.SimulateMousePress((SharpHook.Data.MouseButton)button);
        }

        public static void ReleaseMouse(int button)
        {
            // => 0:None -> 1:Left -> 2:Right -> 3:Middle -> 4:Back -> 5:Forward
            EventSimulator.SimulateMouseRelease((SharpHook.Data.MouseButton)button);
        }

        public static void ScrollMouse(double delta)
        {
            EventSimulator.SimulateMouseWheel((short)delta);
        }



        public static void TrackMouse(double x, double y)
        {

            // this function will track the mouse through the virtual boarders and 
            // through the physical ones as well this will store the values of the
            // virtual pos

            bool result = ValidatePos(x, y);
            if (!result)
            {
                //Console.WriteLine($"Wrong Result");
                var correctedPos = CorrectPos(x, y);
                if (correctedPos == null) return;

                x = correctedPos.Value.X;
                y = correctedPos.Value.Y;

            }

            if (VirtualPositionX == null || VirtualPositionY == null)
            {
                VirtualPositionX = x;
                VirtualPositionY = y;
            }

            double oldVritualPosX = (double)VirtualPositionX;
            double oldVritualPosY = (double)VirtualPositionY;

            // check the crtical regions and send the mouse to the secreen if possible
            bool touchedBoarder = false;
            foreach (var connection in Connections.Devices.ConnectionList)
            {

                if (connection != null &&
                    connection.VirtualScreens != null &&
                    connection.MouseState == Connections.Constants.Transmit)
                {


                    foreach (var screen in connection.VirtualScreens){

                        if (screen.Y <= VirtualPositionY && screen.Y + screen.Height >= VirtualPositionY){
                            if (Math.Abs((decimal)(screen.X - VirtualPositionX)) < (decimal)CriticalRegionSize){
                                VirtualPositionX = screen.X + CursorThrow;
                                touchedBoarder = true;
                                break;
                            }

                            if (Math.Abs((decimal)(screen.X + screen.Width - VirtualPositionX)) < (decimal)CriticalRegionSize){
                                VirtualPositionX = screen.X + screen.Width - CursorThrow;
                                touchedBoarder = true;
                                break;
                            }
                        }


                        if (screen.X <= VirtualPositionX && screen.X + screen.Width >= VirtualPositionX){
                            if (Math.Abs((decimal)(screen.Y - VirtualPositionY)) < (decimal)CriticalRegionSize){
                                VirtualPositionY = screen.Y + CursorThrow;
                                touchedBoarder = true;
                                break;
                            }

                            if (Math.Abs((decimal)(screen.Y + screen.Height - VirtualPositionY)) < (decimal)CriticalRegionSize){
                                VirtualPositionY = screen.Y + screen.Height - CursorThrow;
                                touchedBoarder = true;
                                break;
                            }
                        }
                        if (touchedBoarder) break;

                    }
                }
            }



            result = ValidatePos(VirtualPositionX, VirtualPositionY); // this will check our postion if it exist through both
                                                                      // virtual and physical screens

            if (IsPosOnVirtualScreen((double)VirtualPositionX, (double)VirtualPositionY))
                touchedBoarder = true;

            if (!touchedBoarder){

                if (Math.Abs((decimal)(VirtualPositionX - x)) > SaftyNet ||
                    Math.Abs((decimal)(VirtualPositionY - y)) > SaftyNet){
                    MoveMouse((double)VirtualPositionX, (double)VirtualPositionY);
                    return;
                }

                VirtualPositionX = x;
                VirtualPositionY = y;
            }
            else
            {


                OnVirtualMointorEnter?.Invoke();
            }

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
                            Controllers.Hook.TargetConnection = connection;

                            if (Controllers.Hook.OnTargetConnectionChange != null)
                                Controllers.Hook.OnTargetConnectionChange.Invoke();

                            return true;
                        }
                    }
                }
            }

            if (SharedData.Device.Screens == null) return false;

            foreach (var screen in SharedData.Device.Screens)
            {
                if (screen.Bounds.X <= X && screen.Bounds.X + screen.Bounds.Width >= X &&
                    screen.Bounds.Y <= Y && screen.Bounds.Y + screen.Bounds.Height >= Y)
                {
                    Controllers.Hook.TargetConnection = null;
                    return true;
                }
            }


            return false;

        }


        public static Point? CorrectPos(double x, double y)
        {
            double correctX = x;
            double correctY = y;

            if (SharedData.Device.Screens == null) return null;
            if (Connections.Devices.ConnectionList == null) return null;

            double minDistance = double.MaxValue;
            Point? closestPoint = null;

            // Check physical screens
            foreach (var screen in SharedData.Device.Screens)
            {
                var bounds = screen.Bounds;

                // Clamp the point inside this screen's bounds
                double clampedX = Math.Clamp(x, bounds.X, bounds.X + bounds.Width - 1);
                double clampedY = Math.Clamp(y, bounds.Y, bounds.Y + bounds.Height - 1);

                double dx = clampedX - x;
                double dy = clampedY - y;
                double distance = dx * dx + dy * dy;

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPoint = new Point((int)clampedX, (int)clampedY);
                }
            }

            // Check virtual screens from all connections
            foreach (var connection in Connections.Devices.ConnectionList)
            {
                if (connection.VirtualScreens == null) continue;

                foreach (var screen in connection.VirtualScreens)
                {
                    var bounds = screen;

                    double clampedX = Math.Clamp(x, bounds.X, bounds.X + bounds.Width - 1);
                    double clampedY = Math.Clamp(y, bounds.Y, bounds.Y + bounds.Height - 1);

                    double dx = clampedX - x;
                    double dy = clampedY - y;
                    double distance = dx * dx + dy * dy;

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestPoint = new Point((int)clampedX, (int)clampedY);
                    }
                }
            }

            return closestPoint;
        }


        public static bool IsPosOnPhysicalScreen(double X, double Y)
        {
            if (SharedData.Device.Screens == null) return false;

            foreach (var screen in SharedData.Device.Screens)
            {
                if (screen.Bounds.X + CriticalRegionSize <= X && screen.Bounds.X + screen.Bounds.Width - CriticalRegionSize >= X &&
                    screen.Bounds.Y <= Y + CriticalRegionSize && screen.Bounds.Y + screen.Bounds.Height - CriticalRegionSize >= Y)
                {
                    Controllers.Hook.TargetConnection = null;
                    return true;
                }
            }

            return false;
        }


        public static bool IsPosOnVirtualScreen(double X, double Y){
            foreach (var connection in Connections.Devices.ConnectionList){
                if (connection != null &&
                    connection.VirtualScreens != null &&
                    connection.MouseState == Connections.Constants.Transmit)
                {
                    foreach (var screen in connection.VirtualScreens){
                        if (screen.X <= X && screen.X + screen.Width >= X &&
                            screen.Y <= Y  && screen.Y + screen.Height >= Y)
                        {
                            Controllers.Hook.TargetConnection = connection;
                            return true;
                        }
                    }
                }
            }

            return false;
        }


        public static void SkipNextExecutions(int durationMs)
        {
            SkipNextMouseHook = true;

            if (skipResetTimer == null)
            {
                skipResetTimer = new System.Timers.Timer(durationMs);
                skipResetTimer.AutoReset = false; // trigger once
                skipResetTimer.Elapsed += (s, e) =>
                {
                    SkipNextMouseHook = false;
                };
            }
            else
            {
                skipResetTimer.Interval = durationMs; // update duration
            }

            skipResetTimer.Stop();
            skipResetTimer.Start();
        }

        public static void TransmitMouseMovement(double x, double y)
        {
            // this function will only ransmit the movment if you are at
            // the right cordinates for the screen

            var xPos = x;
            var yPos = y;


            foreach (var connection in Connections.Devices.ConnectionList)
            {
                if (connection.MouseState == Connections.Constants.Transmit)
                {

                    if (connection.VirtualScreens == null) return;

                    for (int i = 0; i < connection.VirtualScreens.Count; i++)
                    {

                        var screen = connection.VirtualScreens[i];

                        if (connection.Screens == null || connection.Screens[i] == null) continue;

                        var physicalScreen = connection.Screens[i]; // this is the other device actual screen which
                                                                    // contain the coordinates and size

                        if (screen.X <= xPos && screen.X + screen.Width >= xPos &&
                            screen.Y <= yPos && screen.Y + screen.Height >= yPos)
                        {


                            double realPosX = (double)(physicalScreen.X + xPos - screen.X);
                            double realPosY = (double)(physicalScreen.Y + yPos - screen.Y);

                            var _command = new Commands.Mouse
                            {
                                X = realPosX,
                                Y = realPosY,
                            };

                            var _commandMessage = new MessageCommand
                            {
                                Type = Commands.Constants.CommandTypes.Mouse,
                                SequenceNumber = connection.SequenceNumber + 1,
                                Command = JsonSerializer.Serialize(_command)
                            };
                            connection.SequenceNumber += 1;

                            var messageudp = new MessageUDP
                            {
                                MessageType = Network.Constants.MessageTypes.Command,
                                Text = Encryptor.Encrypt(JsonSerializer.Serialize(_commandMessage), connection.PasswordKey),
                                IsEncrypted = true
                            };

                            if (connection.MacAddress != null)
                            {
                                ConnectionUDP.Send(MessageManager.MacToIP[connection.MacAddress], messageudp);
                            }

                            //Console.WriteLine($"{realPosX}, {realPosY}");
                        }
                    }

                }
            }
        }



        public static void TransmitMousePressButtons(double x, double y, int Button)
        {
            // note that the x and y pos is not used for telling the other device to go to 
            // the postion it simply there to see which screen it should send the click to


            var xPos = x;
            var yPos = y;


            foreach (var connection in Connections.Devices.ConnectionList)
            {
                if (connection.MouseState == Connections.Constants.Transmit)
                {

                    if (connection.VirtualScreens == null) return;

                    foreach (var screen in connection.VirtualScreens)
                    {

                        if (screen.X <= xPos && screen.X + screen.Width >= xPos &&
                            screen.Y <= yPos && screen.Y + screen.Height >= yPos)
                        {


                            var _command = new Commands.Mouse
                            {
                                MouseButtonPress = Button,
                            };

                            var _commandMessage = new MessageCommand
                            {
                                Type = Commands.Constants.CommandTypes.Mouse,
                                SequenceNumber = connection.SequenceNumber + 1,
                                Command = JsonSerializer.Serialize(_command)
                            };
                            connection.SequenceNumber += 1;

                            var messageudp = new MessageUDP
                            {
                                MessageType = Network.Constants.MessageTypes.Command,
                                Text = Encryptor.Encrypt(JsonSerializer.Serialize(_commandMessage), connection.PasswordKey),
                                IsEncrypted = true
                            };

                            if (connection.MacAddress != null)
                            {
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


            foreach (var connection in Connections.Devices.ConnectionList)
            {
                if (connection.MouseState == Connections.Constants.Transmit)
                {

                    if (connection.VirtualScreens == null) return;

                    foreach (var screen in connection.VirtualScreens)
                    {

                        if (screen.X <= xPos && screen.X + screen.Width >= xPos &&
                            screen.Y <= yPos && screen.Y + screen.Height >= yPos)
                        {


                            var _command = new Commands.Mouse
                            {
                                MouseButtonRelease = Button,
                            };

                            var _commandMessage = new MessageCommand
                            {
                                Type = Commands.Constants.CommandTypes.Mouse,
                                SequenceNumber = connection.SequenceNumber + 1,
                                Command = JsonSerializer.Serialize(_command)
                            };
                            connection.SequenceNumber += 1;

                            var messageudp = new MessageUDP
                            {
                                MessageType = Network.Constants.MessageTypes.Command,
                                Text = Encryptor.Encrypt(JsonSerializer.Serialize(_commandMessage), connection.PasswordKey),
                                IsEncrypted = true
                            };

                            if (connection.MacAddress != null)
                            {
                                ConnectionUDP.Send(MessageManager.MacToIP[connection.MacAddress], messageudp);
                            }

                        }
                    }

                }
            }

        }




        public static void TransmitMouseScroll(double x, double y, double delta)
        {


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

                        if (screen.X <= xPos && screen.X + screen.Width >= xPos &&
                            screen.Y <= yPos && screen.Y + screen.Height >= yPos)
                        {


                            var _command = new Commands.Mouse
                            {
                                ScrollDelta = delta,
                            };

                            var _commandMessage = new MessageCommand
                            {
                                Type = Commands.Constants.CommandTypes.Mouse,
                                SequenceNumber = connection.SequenceNumber + 1,
                                Command = JsonSerializer.Serialize(_command)
                            };
                            connection.SequenceNumber += 1;

                            var messageudp = new MessageUDP
                            {
                                MessageType = Network.Constants.MessageTypes.Command,
                                Text = Encryptor.Encrypt(JsonSerializer.Serialize(_commandMessage), connection.PasswordKey),
                                IsEncrypted = true
                            };

                            if (connection.MacAddress != null)
                            {
                                ConnectionUDP.Send(MessageManager.MacToIP[connection.MacAddress], messageudp);
                            }

                        }
                    }

                }
            }

        }







        public static void RecieveMouseCommnad(Commands.Mouse? command)
        {
            if (command == null) return;

            if (command.X != null && command.Y != null)
            {
                SkipNextExecutions(500);
                MoveMouse((double)command.X, (double)command.Y);
            }

            if (command.MouseButtonPress != null)
            {
                PressMouse((int)command.MouseButtonPress);
            }

            if (command.MouseButtonRelease != null)
            {
                ReleaseMouse((int)command.MouseButtonRelease);
            }

            if (command.ScrollDelta != null)
            {
                ScrollMouse((double)command.ScrollDelta);
            }
        }
    }
}