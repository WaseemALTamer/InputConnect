using InputConnect.Structures;
using System.Text.Json;
using SharpHook;
using System;
using InputConnect.Network;
using System.Threading;






namespace InputConnect.Controllers
{
    public static class Mouse
    {
        // this file is going to be responisble to tracking down the mouse knows where it is
        // and checks if the mouse postion should be tracked or not and try to send the data
        // to the relivent devices


        public static TaskPoolGlobalHook Hook = new TaskPoolGlobalHook();
        public static EventSimulator EventSimulator = new EventSimulator();


        public static Action<double, double>? OnMove; // => <x, y>
        public static Action<int>? OnMousePress; // => 0:None -> 1:Left -> 2:Right -> 3:Middle -> 4:Back -> 5:Forward
        public static Action<int>? OnMouseRealse; // => 0:None -> 1:Left -> 2:Right -> 3:Middle -> 4:Back -> 5:Forward
        public static Action<double>? OnMouseScroll; // => <delta>


        public static double? CurrentPositionX;
        public static double? CurrentPositionY;
        public static double DeltaX = 0;
        public static double DeltaY = 0;

        public static bool BorderHit = false;

        // those will be used to create something like a virtual screen
        // so you can control connected other screens
        public static double OffsetPositionX = 0;
        public static double OffsetPositionY = 0;

        public static double CriticalRegionSize = 1;
        public static double CursorThrow = 1;
        public static bool OnVirtualDisplay = false;
        public static Bounds? CurrentScreenBounds;


        public static Action? OnVirtualMointorEnter;
        public static Action? OnVirtualMointorExit;





        public static bool StartHook(){
            Hook.MouseMoved += (_, e) =>{
                DeltaX = (e.Data.X - CurrentPositionX) ?? 0;
                DeltaY = (e.Data.Y - CurrentPositionY) ?? 0;

                if ((Math.Abs(DeltaX) >= 720 || Math.Abs(DeltaY) >= 720) && !BorderHit) {
                    DeltaX = 0; DeltaY = 0;
                    if (CurrentPositionX != null && CurrentPositionY != null)
                        MoveMouse((double)CurrentPositionX, (double)CurrentPositionY);
                    return;
                }

                BorderHit = false;
                CurrentPositionX = e.Data.X;
                CurrentPositionY = e.Data.Y;
                OnMove?.Invoke(e.Data.X, e.Data.Y);
            };

            Hook.MouseDragged += (_, e) =>{
                DeltaX = (e.Data.X - CurrentPositionX) ?? 0;
                DeltaY = (e.Data.Y - CurrentPositionY) ?? 0;

                if ((Math.Abs(DeltaX) >= 720 || Math.Abs(DeltaY) >= 720) && !BorderHit) {
                    DeltaX = 0; DeltaY = 0;
                    if (CurrentPositionX != null && CurrentPositionY != null)
                        MoveMouse((double)CurrentPositionX, (double)CurrentPositionY);
                    return;
                }

                BorderHit = false;
                CurrentPositionX = e.Data.X;
                CurrentPositionY = e.Data.Y;
                OnMove?.Invoke(e.Data.X, e.Data.Y);
            };



            Hook.MousePressed += (_, e) =>{
                OnMousePress?.Invoke((int)e.Data.Button);
            };

            Hook.MouseReleased += (_, e) => {
                OnMouseRealse?.Invoke((int)e.Data.Button);
            };

            Hook.MouseWheel += (_, e) =>{
                OnMouseScroll?.Invoke(e.Data.Delta * e.Data.Rotation / 360);
            };


            OnMove += TrackMouseVirtualBorders;
            OnMove += TransmitMouseMovement;

            OnMousePress += TransmitMousePressButtons;
            OnMouseRealse += TransmitMouseReleaseButtons;
            OnMouseScroll += TransmitMouseScroll;


            MessageManager.OnCommandMouse += RecieveMouseCommnad;

            new Thread(() =>{
                Hook.RunAsync();
            }).Start();

            return true;
        }




        public static void MoveMouse(double x, double y) {
            EventSimulator.SimulateMouseMovement((short)x, (short)y);
        }

        public static void PressMouse(int button) {
            // => 0:None -> 1:Left -> 2:Right -> 3:Middle -> 4:Back -> 5:Forward
            EventSimulator.SimulateMousePress((SharpHook.Data.MouseButton)button);
        }

        public static void ReleaseMouse(int button){
            // => 0:None -> 1:Left -> 2:Right -> 3:Middle -> 4:Back -> 5:Forward
            EventSimulator.SimulateMouseRelease((SharpHook.Data.MouseButton)button);
        }

        public static void ScrollMouse(double delta) {
            EventSimulator.SimulateMouseWheel((short)(delta));
        }



        public static void TrackMouseVirtualBorders(double x, double y) {

            double PosX = x + OffsetPositionX;
            double PosY = y + OffsetPositionY;

            //Console.WriteLine($"{DeltaX} , {DeltaY}");

            // =============================
            // Virtual screens
            // =============================
            if (!OnVirtualDisplay) { 
            
            
                foreach (var connection in Connections.Devices.ConnectionList)
                {
                    if (connection != null &&
                        connection.Bounds != null &&
                        connection.MouseState == Connections.Constants.Transmit)
                    {
                        var bounds = connection.Bounds;

                        if (bounds == CurrentScreenBounds) continue;



                        // Entering left
                        if ((HasCrossedValue(PosX, DeltaX, (bounds.X - CriticalRegionSize), true)) &&
                            (bounds.Y <= PosY && PosY <= bounds.Y + bounds.Height))
                        {
                            //Console.WriteLine("TouchedBorder3");
                            //Console.WriteLine($" => {PosX} , {PosY}");
                            OffsetPositionX = bounds.X;
                            CurrentScreenBounds = bounds;
                            OnVirtualDisplay = true;
                            BorderHit = true;

                            if (OnVirtualMointorEnter != null) OnVirtualMointorEnter.Invoke();
                            MoveMouse(CriticalRegionSize + CursorThrow, y);
                            return;
                        }

                        // Entering right
                        if ((HasCrossedValue(PosX, DeltaX, (bounds.X + bounds.Width + CriticalRegionSize), false)) &&
                            (bounds.Y <= PosY && PosY <= bounds.Y + bounds.Height))
                        {
                            //Console.WriteLine("TouchedBorder4");
                            //Console.WriteLine($" => {PosX} , {PosY}");
                            OffsetPositionX = bounds.X + bounds.Width;
                            CurrentScreenBounds = bounds;
                            OnVirtualDisplay = true;
                            BorderHit = true;
                            if (OnVirtualMointorEnter != null) OnVirtualMointorEnter.Invoke();
                            MoveMouse(bounds.Width - CriticalRegionSize - CursorThrow, y);
                            return;
                        }
                    }
                }
            }

            // =============================
            // Physical screens
            // =============================
            if (SharedData.Device.Screens != null && OnVirtualDisplay)
            {
                foreach (var screen in SharedData.Device.Screens)
                {
                    var bounds = new Bounds(screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width, screen.Bounds.Height);

                    if (CurrentScreenBounds != null &&
                        CurrentScreenBounds.X == bounds.X &&
                        CurrentScreenBounds.Y == bounds.Y &&
                        CurrentScreenBounds.Width == bounds.Width &&
                        CurrentScreenBounds.Height == bounds.Height) 
                    {
                        continue;
                    }


                    // Left edge
                    if ((HasCrossedValue(PosX, DeltaX, (bounds.X - CriticalRegionSize), true)) &&
                        (bounds.Y <= PosY && PosY <= bounds.Y + bounds.Height))
                    {
                        CurrentScreenBounds = bounds;
                        OffsetPositionX = 0;
                        OnVirtualDisplay = false;
                        BorderHit = true;
                        if (OnVirtualMointorExit != null) OnVirtualMointorExit.Invoke();
                        MoveMouse(CriticalRegionSize + CursorThrow, y);
                        return;
                    }

                    // Right edge
                    if ((HasCrossedValue(PosX, DeltaX, (bounds.X + bounds.Width + CriticalRegionSize), false)) &&
                        (bounds.Y <= PosY && PosY <= bounds.Y + bounds.Height))
                    {
                        //Console.WriteLine(bounds.X + bounds.Width + CriticalRegionSize);
                        CurrentScreenBounds = bounds;
                        OffsetPositionX = 0;
                        OnVirtualDisplay = false;
                        BorderHit = true;
                        if (OnVirtualMointorExit != null) OnVirtualMointorExit.Invoke();
                        MoveMouse(bounds.X + bounds.Width - CriticalRegionSize - CursorThrow, y);
                        return;
                    }
                }
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



        public static void TransmitMouseMovement(double x, double y) {
            // this function will only ransmit the movment if you are at
            // the right cordinates for the screen

            var xPos = x + OffsetPositionX;
            var yPos = y + OffsetPositionY;

            if (OnVirtualDisplay == false) return;

            foreach (var connection in Connections.Devices.ConnectionList) {
                if (connection.MouseState == Connections.Constants.Transmit) {

                    if (connection.Bounds == null) continue;

                    if (connection.Bounds.X <= xPos && (connection.Bounds.X + connection.Bounds.Width) >= xPos &&
                        connection.Bounds.Y <= yPos && (connection.Bounds.Y + connection.Bounds.Height) >= yPos)
                    {
                        var _command = new Commands.Mouse{
                            X = x,
                            Y = y,
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


        public static void TransmitMousePressButtons(int Button) {
            // x and y are there to check if you are on the right coordinates

            var xPos = CurrentPositionX + OffsetPositionX;
            var yPos = CurrentPositionY + OffsetPositionY;

            if (OnVirtualDisplay == false) return;

            foreach (var connection in Connections.Devices.ConnectionList)
            {
                if (connection.MouseState == Connections.Constants.Transmit)
                {

                    if (connection.Bounds == null) continue;

                    if (connection.Bounds.X <= xPos && (connection.Bounds.X + connection.Bounds.Width) >= xPos &&
                        connection.Bounds.Y <= yPos && (connection.Bounds.Y + connection.Bounds.Height) >= yPos)
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
                            Text = Encryptor.Encrypt(JsonSerializer.Serialize(_commandMessage), connection.Token),
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


        public static void TransmitMouseReleaseButtons(int Button)
        {
            // x and y are there to check if you are on the right coordinates

            var xPos = CurrentPositionX + OffsetPositionX;
            var yPos = CurrentPositionY + OffsetPositionY;

            if (OnVirtualDisplay == false) return;

            foreach (var connection in Connections.Devices.ConnectionList)
            {
                if (connection.MouseState == Connections.Constants.Transmit)
                {

                    if (connection.Bounds == null) continue;

                    if (connection.Bounds.X <= xPos && (connection.Bounds.X + connection.Bounds.Width) >= xPos &&
                        connection.Bounds.Y <= yPos && (connection.Bounds.Y + connection.Bounds.Height) >= yPos)
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
                            Text = Encryptor.Encrypt(JsonSerializer.Serialize(_commandMessage), connection.Token),
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



        public static void TransmitMouseScroll(double delta){


            // x and y are there to check if you are on the right coordinates


            var xPos = CurrentPositionX + OffsetPositionX;
            var yPos = CurrentPositionY + OffsetPositionY;

            if (OnVirtualDisplay == false) return;

            foreach (var connection in Connections.Devices.ConnectionList)
            {
                if (connection.MouseState == Connections.Constants.Transmit)
                {

                    if (connection.Bounds == null) continue;

                    if (connection.Bounds.X <= xPos && (connection.Bounds.X + connection.Bounds.Width) >= xPos &&
                        connection.Bounds.Y <= yPos && (connection.Bounds.Y + connection.Bounds.Height) >= yPos)
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
                            Text = Encryptor.Encrypt(JsonSerializer.Serialize(_commandMessage), connection.Token),
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





        public static void RecieveMouseCommnad(Commands.Mouse command) {

            if (command == null) return;

            if (command.X != null && command.Y != null){
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