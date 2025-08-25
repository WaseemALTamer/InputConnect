using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using InputConnect.Commands;
using System;
using System.Diagnostics.Metrics;
using System.Reflection;


namespace InputConnect.UI.OutWindowPopup
{
    public class MouseController
    {
        private readonly InvisiableOverlay MasterWindow;

        private bool _IsHidden = true;
        public bool IsHidden{
            get { return _IsHidden; }
            set { _IsHidden = value; }
        }

        public MouseController(InvisiableOverlay masterWindow)
        {
            MasterWindow = masterWindow;
            MasterWindow.PointerMoved += OnPointerMoved;
            MasterWindow.PointerPressed += OnPointerPressed;
            MasterWindow.PointerReleased += OnPointerReleased;
            MasterWindow.PointerWheelChanged += OnPointerWheelChanged;


            MasterWindow.OnShow += CenterCursor;
            MasterWindow.OnHide += () => {

                if (Controllers.Mouse.VirtualPositionX == null || 
                   (Controllers.Mouse.VirtualPositionY == null) )
                    return;

                Controllers.Mouse.MoveMouse((double)Controllers.Mouse.VirtualPositionX,
                                            (double)Controllers.Mouse.VirtualPositionY);
            };


            UpdateCenter();
        }




        private PixelPoint _centerPos;
        private void UpdateCenter()
        {
            var screen = MasterWindow.Screens.Primary; // or pick the right screen

            if (screen == null) return;

            var bounds = screen.Bounds;

            _centerPos = new PixelPoint(
                bounds.X + bounds.Width / 2,
                bounds.Y + bounds.Height / 2
            );
        }



        public void CenterCursor(){
            Controllers.Mouse.MoveMouse(_centerPos.X, _centerPos.Y);
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e){
            if (IsHidden) return;

            var pos = e.GetPosition(MasterWindow);
            //Console.WriteLine($"{(double)Controllers.Mouse.VirtualPositionX}, {(double)Controllers.Mouse.VirtualPositionY}");


            var screenPos = e.GetPosition(null);

            // Calculate distance from center
            var dxToCenter = screenPos.X - _centerPos.X;
            var dyToCenter = screenPos.Y - _centerPos.Y;

            // Check if cursor is outside 100px "safe zone"
            if (Math.Abs(dxToCenter) > 100 || Math.Abs(dyToCenter) > 100)
            {
                // Run your logic
                CenterCursor();
                return;
            }



            var dx = pos.X - (MasterWindow.Bounds.Width / 2);
            var dy = pos.Y - (MasterWindow.Bounds.Height / 2);

            if (dx != 0 || dy != 0){

                if (Controllers.Mouse.VirtualPositionX == null ||
                    Controllers.Mouse.VirtualPositionY == null){ 
                    return;
                }

                double Xpos = (double)Controllers.Mouse.VirtualPositionX + dx;
                double Ypos = (double)Controllers.Mouse.VirtualPositionY + dy;


                var result = Controllers.Mouse.ValidatePos(Xpos, Ypos);

                //Console.WriteLine(result);

                if (result){
                    Controllers.Mouse.VirtualPositionX += dx;
                    Controllers.Mouse.VirtualPositionY += dy;
                    //Console.WriteLine($"{Controllers.Mouse.VirtualPositionX}, {Controllers.Mouse.VirtualPositionY}");
                    Controllers.Mouse.TransmitMouseMovement((double)Controllers.Mouse.VirtualPositionX,
                                                            (double)Controllers.Mouse.VirtualPositionY);

                }
                else {
                    CenterCursor();
                    return;
                }

                result = Controllers.Mouse.IsPosOnPhysicalOnScreen(Xpos, Ypos);

                if (result) {

                    Controllers.Mouse.IsVirtualCoordinates = false;
                    Controllers.Mouse.MouseLock = false;
                    IsHidden = true;

                    MasterWindow.Hide();

                    return;
                }



                //Console.WriteLine($"{Controllers.Mouse.VirtualPositionX}, {Controllers.Mouse.VirtualPositionY}");
            }

            CenterCursor();
        }



        private void OnPointerPressed(object? sender, PointerEventArgs e) {

            var point = e.GetCurrentPoint(MasterWindow);


            // Sharp Hook info "// => 0:None -> 1:Left -> 2:Right -> 3:Middle -> 4:Back -> 5:Forward"


            if (Controllers.Mouse.VirtualPositionX == null ||
                Controllers.Mouse.VirtualPositionY == null) 
                return;


            //Console.WriteLine(point.Properties.IsLeftButtonPressed);

            if (point.Properties.IsLeftButtonPressed)
            {
                Controllers.Mouse.TransmitMousePressButtons(
                    (double)Controllers.Mouse.VirtualPositionX, 
                    (double)Controllers.Mouse.VirtualPositionY, 1);
            }
            else if (point.Properties.IsRightButtonPressed)
            {
                Controllers.Mouse.TransmitMousePressButtons(
                    (double)Controllers.Mouse.VirtualPositionX, 
                    (double)Controllers.Mouse.VirtualPositionY, 2);
            }
            else if (point.Properties.IsMiddleButtonPressed)
            {
                Controllers.Mouse.TransmitMousePressButtons(
                    (double)Controllers.Mouse.VirtualPositionX, 
                    (double)Controllers.Mouse.VirtualPositionY, 3);
            }
            else if (point.Properties.IsXButton1Pressed) // usually "Back" button
            {
                Controllers.Mouse.TransmitMousePressButtons(
                    (double)Controllers.Mouse.VirtualPositionX, 
                    (double)Controllers.Mouse.VirtualPositionY, 4);
            }
            else if (point.Properties.IsXButton2Pressed) // usually "Forward" button
            {
                Controllers.Mouse.TransmitMousePressButtons(
                    (double)Controllers.Mouse.VirtualPositionX, 
                    (double)Controllers.Mouse.VirtualPositionY, 5);
            }

        }



        private void OnPointerReleased(object? sender, PointerEventArgs e)
        {

            var point = e.GetCurrentPoint(MasterWindow);


            // Sharp Hook info "// => 0:None -> 1:Left -> 2:Right -> 3:Middle -> 4:Back -> 5:Forward"


            if (Controllers.Mouse.VirtualPositionX == null ||
                Controllers.Mouse.VirtualPositionY == null)
                return;

            switch (point.Properties.PointerUpdateKind)
            {
                case PointerUpdateKind.LeftButtonReleased:
                    Controllers.Mouse.TransmitMouseReleaseButtons(
                        (double)Controllers.Mouse.VirtualPositionX,
                        (double)Controllers.Mouse.VirtualPositionY, 1);
                    break;

                case PointerUpdateKind.RightButtonReleased:
                    Controllers.Mouse.TransmitMouseReleaseButtons(
                        (double)Controllers.Mouse.VirtualPositionX,
                        (double)Controllers.Mouse.VirtualPositionY, 2);
                    break;

                case PointerUpdateKind.MiddleButtonReleased:
                    Controllers.Mouse.TransmitMouseReleaseButtons(
                        (double)Controllers.Mouse.VirtualPositionX,
                        (double)Controllers.Mouse.VirtualPositionY, 3);
                    break;

                case PointerUpdateKind.XButton1Released: // Back button
                    Controllers.Mouse.TransmitMouseReleaseButtons(
                        (double)Controllers.Mouse.VirtualPositionX,
                        (double)Controllers.Mouse.VirtualPositionY, 4);
                    break;

                case PointerUpdateKind.XButton2Released: // Forward button
                    Controllers.Mouse.TransmitMouseReleaseButtons(
                        (double)Controllers.Mouse.VirtualPositionX,
                        (double)Controllers.Mouse.VirtualPositionY, 5);
                    break;
            }
        }


        private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {

            e.Handled = true; //OverRiding the scrolling function
            double delta = e.Delta.Y * Setting.Config.MouseScrollStrength;

            //Console.WriteLine(delta);


            if (Controllers.Mouse.VirtualPositionX == null ||
                Controllers.Mouse.VirtualPositionY == null)
                    return;




            Controllers.Mouse.TransmitMouseScroll(
                (double)Controllers.Mouse.VirtualPositionX,
                (double)Controllers.Mouse.VirtualPositionY,
                delta);


        }


    }
}
