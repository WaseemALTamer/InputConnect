using InputConnect.UI.OutWindowPopup;






namespace InputConnect.Controllers.Mouse
{
    public class InWindowMouse
    {
        // when transmiting the mouse movement move to a tick based system that works in 
        // a queue and takes the latest transtion rather than every time the mouse moves



        private readonly InvisiableOverlaySDL MasterWindow;



        public InWindowMouse(InvisiableOverlaySDL masterWindow)
        {
            MasterWindow = masterWindow;
            MasterWindow.OnMouseMove += OnPointerMoved;
            MasterWindow.OnMouseButtonPress += OnPointerPressed;
            MasterWindow.OnMouseButtonRelease += OnPointerReleased;
            MasterWindow.OnMouseScroll += OnPointerWheelChanged;


            MasterWindow.OnHide += () =>
            {

                if (GlobalMouse.VirtualPositionX == null ||
                    GlobalMouse.VirtualPositionY == null)
                    return;

                GlobalMouse.MoveMouse((double)GlobalMouse.VirtualPositionX,
                                      (double)GlobalMouse.VirtualPositionY);

                GlobalMouse.IsMouseTracking = true;

            };



        }




        private void OnPointerMoved(int dx, int dy){

            if (dx != 0 || dy != 0){
                if (GlobalMouse.VirtualPositionX == null ||
                    GlobalMouse.VirtualPositionY == null)
                {
                    return;
                }

                double Xpos = (double)GlobalMouse.VirtualPositionX + dx;
                double Ypos = (double)GlobalMouse.VirtualPositionY + dy;


                //Console.WriteLine($"{Xpos}, {Ypos}");

                var result = GlobalMouse.ValidatePos(Xpos, Ypos);

                if (!result){
                    //Console.WriteLine($"Wrong Result");
                    var correctedPos = GlobalMouse.CorrectPos(Xpos, Ypos);
                    if (correctedPos == null){
                        return;
                    }

                    Xpos = correctedPos.Value.X;
                    Ypos = correctedPos.Value.Y;


                }


                //Console.WriteLine(result);


                GlobalMouse.VirtualPositionX = Xpos;
                GlobalMouse.VirtualPositionY = Ypos;
                //Console.WriteLine($"{Controllers.Mouse.VirtualPositionX}, {Controllers.Mouse.VirtualPositionY}");
                GlobalMouse.TransmitMouseMovement((double)GlobalMouse.VirtualPositionX,
                                                  (double)GlobalMouse.VirtualPositionY);



                result = GlobalMouse.IsPosOnPhysicalScreen(Xpos, Ypos);

                if (result){
                    GlobalMouse.IsMouseTracking = true;
                    MasterWindow.Hide();
                    return;
                }



                //Console.WriteLine($"{Controllers.Mouse.VirtualPositionX}, {Controllers.Mouse.VirtualPositionY}");
            }

        }



        private void OnPointerPressed(int button)
        {




            // Sharp Hook info "// => 0:None -> 1:Left -> 2:Right -> 3:Middle -> 4:Back -> 5:Forward"


            if (GlobalMouse.VirtualPositionX == null ||
                GlobalMouse.VirtualPositionY == null)
                return;


            //Console.WriteLine(point.Properties.IsLeftButtonPressed);

            if (button == 1)
            {
                GlobalMouse.TransmitMousePressButtons(
                    (double)GlobalMouse.VirtualPositionX,
                    (double)GlobalMouse.VirtualPositionY, 1);
            }
            else if (button == 2)
            {
                GlobalMouse.TransmitMousePressButtons(
                    (double)GlobalMouse.VirtualPositionX,
                    (double)GlobalMouse.VirtualPositionY, 3);
            }
            else if (button == 3)
            {
                GlobalMouse.TransmitMousePressButtons(
                    (double)GlobalMouse.VirtualPositionX,
                    (double)GlobalMouse.VirtualPositionY, 2);
            }
            else if (button == 4)
            {
                GlobalMouse.TransmitMousePressButtons(
                    (double)GlobalMouse.VirtualPositionX,
                    (double)GlobalMouse.VirtualPositionY, 4);
            }
            else if (button == 5)
            {
                GlobalMouse.TransmitMousePressButtons(
                    (double)GlobalMouse.VirtualPositionX,
                    (double)GlobalMouse.VirtualPositionY, 5);
            }

        }



        private void OnPointerReleased(int button)
        {

            // Sharp Hook info "// => 0:None -> 1:Left -> 2:Right -> 3:Middle -> 4:Back -> 5:Forward"


            if (GlobalMouse.VirtualPositionX == null ||
                GlobalMouse.VirtualPositionY == null)
                return;

            

            if (button == 1)
            {
                GlobalMouse.TransmitMouseReleaseButtons(
                    (double)GlobalMouse.VirtualPositionX,
                    (double)GlobalMouse.VirtualPositionY, 1);
            }
            else if (button == 2)
            {
                GlobalMouse.TransmitMouseReleaseButtons(
                    (double)GlobalMouse.VirtualPositionX,
                    (double)GlobalMouse.VirtualPositionY, 3);
            }
            else if (button == 3)
            {
                GlobalMouse.TransmitMouseReleaseButtons(
                    (double)GlobalMouse.VirtualPositionX,
                    (double)GlobalMouse.VirtualPositionY, 2);
            }
            else if (button == 4)
            {
                GlobalMouse.TransmitMouseReleaseButtons(
                    (double)GlobalMouse.VirtualPositionX,
                    (double)GlobalMouse.VirtualPositionY, 4);
            }
            else if (button == 5)
            {
                GlobalMouse.TransmitMouseReleaseButtons(
                    (double)GlobalMouse.VirtualPositionX,
                    (double)GlobalMouse.VirtualPositionY, 5);
            }
        }


        private void OnPointerWheelChanged(int dx, int dy)
        {

            double delta = dy * Setting.Config.MouseScrollStrength;



            if (GlobalMouse.VirtualPositionX == null ||
                GlobalMouse.VirtualPositionY == null)
                return;




            GlobalMouse.TransmitMouseScroll(
                (double)GlobalMouse.VirtualPositionX,
                (double)GlobalMouse.VirtualPositionY,
                delta);


        }


    }
}
