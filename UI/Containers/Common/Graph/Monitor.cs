using Avalonia.Controls;
using Avalonia.Input;
using InputConnect.Structures;
using System;




namespace InputConnect.UI.Containers.Common
{
    class Monitor : Border, IGraphObject
    {
        public double? X { get; set; }
        public double? Y { get; set; }

        public double? GraphWidth { get; set; }
        public double? GraphHeight { get; set; }

        public Graph? MasterGraph { get; set; }


        public Bounds? ScreenBounds;

        private Canvas? _MainCanvas;
        public Canvas? MainCanvas{
            get { return _MainCanvas; }
            set { _MainCanvas = value; }
        }

        private bool? _IsLocked = true;
        public bool? IsLocked{
            get { return _IsLocked; }
            set { _IsLocked = value; }
        }

        public Border? LockImageBorder;
        public Image? LockImage;


        private Animations.Transations.EaseInOut? WarningTranstion;
        public bool WarningState = false;
        public Border? WarningImageBorder;
        public Image? WarningImage;



        private int MointorNumber = 0;
        private TextBlock? MointorTextBlock;

        

        private double EdgesSize = 0.15; // edges size for the sides this is in reliative to the height


        private Animations.Transations.EaseInOut? ClickTrnastion;

        private Animations.Transations.EaseInOut? ShowHideTransition;


        public Connection? Connection;

        public Monitor(double x, 
                       double y, 
                       double graphWidth, 
                       double graphHeight,
                       int mointorNumber,
                       Graph masterGraph)
        {

            X = x;
            Y = y;
            GraphWidth = graphWidth;
            GraphHeight = graphHeight;

            MointorNumber = mointorNumber;
            MasterGraph = masterGraph;


            Width = graphWidth; 
            Height = graphHeight;



            Background = Setting.Themes.MonitorEdges;

            MainCanvas = new Canvas{
                Background = Setting.Themes.Backgrounds[MointorNumber - 1],
                Width = Width - EdgesSize, Height = Height - EdgesSize,
            };

            LockImageBorder = new Border{
                Width = MainCanvas.Height,
                Height = MainCanvas.Height,
                IsVisible = false,
                Opacity = 0.3,
            };

            LockImage = new Image{
                Stretch = Avalonia.Media.Stretch.Uniform,
            };
            LockImageBorder.Child = LockImage;
            Assets.AddAwaitedAction(() => {
                LockImage.Source = Assets.LockBitmap;
            });
            MainCanvas.Children.Add(LockImageBorder);


            WarningImageBorder = new Border{
                Width = MainCanvas.Height,
                Height = MainCanvas.Height,
                IsVisible = false,
                Opacity = 0,
            };

            WarningImage = new Image{
                Stretch = Avalonia.Media.Stretch.Uniform,
            };
            WarningImageBorder.Child = WarningImage;
            Assets.AddAwaitedAction(() => {
                WarningImage.Source = Assets.WarningBitmap;
            });
            MainCanvas.Children.Add(WarningImageBorder);


            WarningTranstion = new Animations.Transations.EaseInOut{
                StartingValue = 0,
                EndingValue = 0.9,
                Duration = Setting.Config.TransitionDuration,
                Trigger = SetWarningOpacity
            };


            MointorTextBlock = new TextBlock { 
                Text = $"{MointorNumber}",
                FontSize = Setting.Config.FontSize,
                Width = Setting.Config.FontSize,
                Height = Setting.Config.FontSize,
                TextAlignment = Avalonia.Media.TextAlignment.Center,
            };

            MainCanvas.Children.Add(MointorTextBlock);

            PointerPressed += OnPointerPressed;

            SizeChanged += OnSizeChaged;
            OnSizeChaged();


            MasterGraph.PointerMoved += OnPointerMoved;
            PointerReleased += OnPointerReleased;
            PointerWheelChanged += (s, e) => { 
                if (IsDraging)
                    e.Handled = true; 
            };

            ClickTrnastion = new Animations.Transations.EaseInOut{
                StartingValue = 1,
                EndingValue = 0.5,
                Duration = Setting.Config.TransitionDuration,
                Trigger = SetOpacity
            };


            ShowHideTransition = new Animations.Transations.EaseInOut
            {
                StartingValue = 0,
                EndingValue = 1,
                Duration = Setting.Config.TransitionDuration,
                Trigger = SetOpacity
            };


            

            Child = MainCanvas;


        }


        public void SetLock(bool state) {
            if (LockImageBorder == null) return;
            LockImageBorder.IsVisible = state;
            IsLocked = state;
        }


        private void OnSizeChaged(object? sender = null, object? e = null) {

            if (MainCanvas != null) {

                MainCanvas.Width = Width - (EdgesSize * Height);
                MainCanvas.Height = Height - (EdgesSize * Height);

                Canvas.SetLeft(MainCanvas, (Width - MainCanvas.Width)/ 2);
                Canvas.SetTop(MainCanvas, (Height - MainCanvas.Height)/ 2);

                if (LockImageBorder != null) {
                    LockImageBorder.Width = MainCanvas.Height;
                    LockImageBorder.Height = MainCanvas.Height;

                    Canvas.SetLeft(LockImageBorder, (MainCanvas.Width - LockImageBorder.Width) / 2);
                    Canvas.SetTop(LockImageBorder, (MainCanvas.Height - LockImageBorder.Height) / 2);

                }


                if (WarningImageBorder != null)
                {
                    WarningImageBorder.Width = MainCanvas.Height;
                    WarningImageBorder.Height = MainCanvas.Height;

                    Canvas.SetLeft(WarningImageBorder, (MainCanvas.Width - WarningImageBorder.Width) / 2);
                    Canvas.SetTop(WarningImageBorder, (MainCanvas.Height - WarningImageBorder.Height) / 2);

                }


                if (MointorTextBlock != null) {
                    Canvas.SetLeft(MointorTextBlock, (MainCanvas.Width - MointorTextBlock.Width) / 2);
                    Canvas.SetTop(MointorTextBlock, (MainCanvas.Height - MointorTextBlock.Height) / 2);
                }

            }

        }


        private double XposMointor = 0;
        private double YposMointer = 0;

        private bool IsDraging;

        private void OnPointerPressed(object? sender, PointerEventArgs e) {
            if (IsLocked == true) return;

            if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
                return;


            // we stop the Mouse connection is errors might arrise when
            // moving the mointor 
            if (Connection != null && 
                Connection.MouseState == Connections.Constants.Transmit) 
            {
                Connection.MouseState = null;
                SharedData.Events.OnMovingVirtualScreens?.Invoke(); // notify the other parts of the code
            }
                
            


            e.Handled = true;
            IsDraging = true;
            if (ClickTrnastion != null)
                ClickTrnastion.TranslateForward();

            var pos = e.GetPosition(this);

            XposMointor = pos.X;
            YposMointer = pos.Y;
        }


        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e) {


            if (MouseButton.Left != e.InitialPressMouseButton)
                return;

            IsDraging = false;
            if (ClickTrnastion != null && IsLocked == false)
                ClickTrnastion.TranslateBackward();

            SharedData.Events.OnSetVirtualScreensPos?.Invoke();

            // AppData.SaveConnections(); // we save the last pos of the mointor postion
                                          // consider changing this  to trigger an event
                                          // in the shared event, <COMPLETED>
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e) {
            if (!IsDraging || ScreenBounds == null) return;
            if (MasterGraph == null || MasterGraph.MainCanvas == null) return;

            ValidatePos();

            var MousePos = e.GetPosition(MasterGraph.MainCanvas);

            double posX = (((MousePos.X - XposMointor) / MasterGraph.MainCanvas.Width) * MasterGraph.GraphWidth) - (MasterGraph.GraphWidth / 2);
            double posY = ((((MousePos.Y - YposMointer + Height) / MasterGraph.MainCanvas.Height) * MasterGraph.GraphHeight) - (MasterGraph.GraphHeight / 2));

            ScreenBounds.X = (int)posX;
            ScreenBounds.Y = (int)posY;

            X = (int)posX;
            Y = (int)posY;
            MasterGraph.Update();
        }


        private void ValidatePos(){

            if (ScreenBounds == null) return;
            
            foreach (var connection in Connections.Devices.ConnectionList){
                if (connection.VirtualScreens == null)
                    continue;

                

                foreach (var screen in connection.VirtualScreens){
                    if (screen == ScreenBounds) continue;

                    if (IsIntersecting(ScreenBounds, screen)){
                        SetWarningOpacity(true);
                        return;
                    }

                }




            }


            if (SharedData.Device.Screens == null) return;

            foreach (var screen in SharedData.Device.Screens)
            {
                
                if (IsIntersecting(ScreenBounds, new Bounds(screen.Bounds.X,
                                                            screen.Bounds.Y,
                                                            screen.Bounds.Width,
                                                            screen.Bounds.Height))) 
                {
                    SetWarningOpacity(true);
                    return;
                }   
            }

            SetWarningOpacity(false);
        }


        private bool IsIntersecting(Bounds a, Bounds b){
            bool noOverlap =
                a.X + a.Width <= b.X ||  // a is left of b
                a.X >= b.X + b.Width ||  // a is right of b
                a.Y + a.Height <= b.Y || // a is above b
                a.Y >= b.Y + b.Height;   // a is below b

            return !noOverlap;
        }


        private void SetOpacity(double value) {
            Opacity = value;
            if (value > 0)
                IsVisible = true;
            else
                IsVisible = false;
        }


        private void SetWarningOpacity(double value){

            if(WarningImageBorder != null){
                WarningImageBorder.Opacity = value;

                if (value > 0) 
                    WarningImageBorder.IsVisible = true;
                else 
                    WarningImageBorder.IsVisible = false;
            }
        }


        public void SetWarningOpacity(bool state) {
            if (WarningState == state) return;
            WarningState = state;

            if (state){
                WarningTranstion?.TranslateForward();
            }
            else {
                WarningTranstion?.TranslateBackward();
            }
        }



        public void Show(){
            if (Opacity == 1) return;
            if (ShowHideTransition != null)
                ShowHideTransition.TranslateForward();
        }

        public void Hide(){
            if (Opacity == 0) return;
            if (ShowHideTransition != null)
                ShowHideTransition.TranslateBackward();
        }



    }
}
