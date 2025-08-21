using InputConnect.Setting;
using Avalonia.Controls;
using Avalonia.Input;
using InputConnect.Structures;
using System;
using Avalonia.Remote.Protocol.Input;



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



        private int MointorNumber = 0;
        private TextBlock? MointorTextBlock;

        

        private double EdgesSize = 0.15; // edges size for the sides this is in reliative to the height


        private Animations.Transations.EaseInOut? ClickTrnastion;

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

            Background = Themes.MonitorEdges;

            MainCanvas = new Canvas{
                Background = Themes.Backgrounds[MointorNumber - 1],
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

            MointorTextBlock = new TextBlock { 
                Text = $"{MointorNumber}",
                FontSize = Config.FontSize,
                Width = Config.FontSize,
                Height = Config.FontSize,
                TextAlignment = Avalonia.Media.TextAlignment.Center,
            };

            MainCanvas.Children.Add(MointorTextBlock);

            PointerPressed += OnPointerPressed;

            SizeChanged += OnSizeChaged;
            OnSizeChaged();


            MasterGraph.PointerMoved += OnPointerMoved;
            PointerReleased += OnPointerReleased;


            ClickTrnastion = new Animations.Transations.EaseInOut{
                StartingValue = 1,
                EndingValue = 0.5,
                Duration = Config.TransitionDuration,
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


            e.Handled = true;
            IsDraging = true;
            if (ClickTrnastion != null)
                ClickTrnastion.TranslateForward();

            var pos = e.GetPosition(this);

            XposMointor = pos.X;
            YposMointer = pos.Y;
        }


        private void OnPointerReleased(object? sender, PointerEventArgs e) {
            IsDraging = false;
            if (ClickTrnastion != null)
                ClickTrnastion.TranslateBackward();
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e) {
            if (!IsDraging || ScreenBounds == null) return;
            if (MasterGraph == null || MasterGraph.MainCanvas == null) return;


            var MousePos = e.GetPosition(MasterGraph.MainCanvas);

            double posX = (((MousePos.X - XposMointor) / MasterGraph.MainCanvas.Width) * MasterGraph.GraphWidth) - (MasterGraph.GraphWidth / 2);
            double posY = ((((MousePos.Y - YposMointer + Height) / MasterGraph.MainCanvas.Height) * MasterGraph.GraphHeight) - (MasterGraph.GraphHeight / 2));

            ScreenBounds.X = (int)posX;
            ScreenBounds.Y = (int)posY;

            X = (int)posX;
            Y = (int)posY;
            MasterGraph.Update();
        }



        private void SetOpacity(double value) {
            Opacity = value;
        }
        



    }
}
