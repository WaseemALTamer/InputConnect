using System.Collections.Generic;
using InputConnect.UI.Animations;
using Avalonia.Controls.Shapes;
using InputConnect.Setting;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia;
using System;









namespace InputConnect.UI.Containers.Common
{
    public class Graph : Border
    {


        private Canvas? _MasterCanvas;
        public Canvas? MasterCanvas{
            get { return _MasterCanvas; }
            set { _MasterCanvas = value; }
        }

        private Canvas? _MainCanvas;
        public Canvas? MainCanvas{
            get { return _MainCanvas; }
            set { _MainCanvas = value; }
        }



        private Ellipse? _Povit;
        public Ellipse? Povit{
            get { return _Povit; }
            set { _Povit = value; }
        }


        private double _MaxZoomInRatio = 10;
        public double MaxZoomInRatio{
            get { return _MaxZoomInRatio; }
            set { _MaxZoomInRatio = value; }
        }

        private SmoothScrolling? ScrollingAnimation;

        private List<Line> gridLines = new List<Line>(); // this is for the canvase grid

        private const double TargetGridSpacingScreen = 64; // this is just a rough estimate it needs to be changed later for more
                                                            // accurate results


        private double _GraphWidth = 8000; // this will contain both positive and negative domains
        public double GraphWidth{
            get { return _GraphWidth; }
            set { _GraphWidth = value; }
        }

        private double _GraphHeight = 8000; // this will contain both positive and negative domains
        public double GraphHeight{
            get { return _GraphHeight; }
            set { _GraphHeight = value; }
        }



        private List<IGraphObject> GraphObjectsList = new List<IGraphObject>(); // this will hold the children that are plotted on the graph
        

        public Graph(double? width = null, 
                     double? height = null, 
                     double? graphWidth = null, 
                     double? graphHeight = null)
        {

            //CornerRadius = new CornerRadius(Config.CornerRadius);



            Width = 400;
            Height = 400;

            if (width != null) Width = (double)width;
            if (height != null) Height = (double)height;

            if (graphWidth != null) GraphWidth = (double)graphWidth;
            if (graphHeight != null) GraphHeight = (double)graphHeight;


            Background = Themes.Holder;
            ClipToBounds = true;

            MasterCanvas = new Canvas();
            MasterCanvas.Width = Width;
            MasterCanvas.Height = Height;
            MasterCanvas.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));


            MasterCanvas.PointerMoved += OnPointerMovedMainCanvas;
            MasterCanvas.PointerPressed += OnPointerPressedMainCanvas;
            MasterCanvas.PointerReleased += OnPointerReleasedMainCanvas;


            MainCanvas = new Canvas();
            //MainCanvas.Background = new SolidColorBrush(Color.FromArgb(0,0,0,0));
            //MainCanvas.Background = Themes.Backgruond;
            MainCanvas.Width = GraphWidth/10;
            MainCanvas.Height = GraphHeight/10;


            MasterCanvas.Children.Add(MainCanvas);


            Canvas.SetLeft(MainCanvas, (MasterCanvas.Width - MainCanvas.Width) / 2);
            Canvas.SetTop(MainCanvas, (MasterCanvas.Height - MainCanvas.Height) / 2);



            Povit = new Ellipse{
                Width = 10,
                Height = 10,
                Fill = Brushes.DarkRed,
                ZIndex = 1
            };



            // Add it to the canvas
            MainCanvas.Children.Add(Povit);


            //var monitor = new Monitor(500, 500, 1920, 1080);
            //AddGraphObject(monitor);


            ScrollingAnimation = new SmoothScrolling{
                Trigger = SmoothScrollerTrigger
            };


            PointerWheelChanged += OnPointerWheelChanged;
            MasterCanvas.PointerWheelChanged += OnPointerWheelChanged;



            Update();
            //SmoothScrollerTrigger(0); // apply the size of the grid





            Child = MasterCanvas;
        }



        public void Update() {

            if (MainCanvas != null){
                double minPixelSize = Math.Min(Width, Height);
                double maxPixelSizeX = GraphHeight * MaxZoomInRatio;
                double maxPixelSizeY = GraphHeight * MaxZoomInRatio;

                MainCanvas.Width = Math.Clamp(MainCanvas.Width, minPixelSize, maxPixelSizeX);
                MainCanvas.Height = Math.Clamp(MainCanvas.Height, minPixelSize, maxPixelSizeY);

            }



            foreach (var obj in GraphObjectsList){
                if (MainCanvas == null) continue;

                if (obj.X == null ||
                    obj.Y == null ||
                    obj.GraphWidth == null ||
                    obj.GraphHeight == null) continue;


                if (obj is Control visual){

                    visual.Width = ((double)obj.GraphWidth / GraphWidth) * MainCanvas.Width;
                    visual.Height = ((double)obj.GraphHeight / GraphHeight) * MainCanvas.Height;

                    double Xpos = ((double)obj.X / GraphWidth) * MainCanvas.Width;
                    double Ypos = -((double)obj.Y / GraphHeight) * MainCanvas.Height;


                    Canvas.SetLeft(visual, (MainCanvas.Width/2) + Xpos);
                    Canvas.SetTop(visual, ((MainCanvas.Height/2) - visual.Height) - Ypos);
                }
            }

            ApplyGrid();
        }
        
        public void AddGraphObject<ChildType>(ChildType child) where ChildType : Control, IGraphObject{
            if (MainCanvas == null) return;
            GraphObjectsList.Add(child);
            MainCanvas.Children.Add(child);
            Update();
        }

        public void ClearGraphObjects() {
            for (int i = GraphObjectsList.Count - 1; i >= 0; i--)
            {
                var obj = GraphObjectsList[i];
                var controlObject = obj as Control;
                if (controlObject != null && MainCanvas != null){
                    MainCanvas.Children.Remove(controlObject);
                }

                GraphObjectsList.RemoveAt(i);
            }
        }


        double MasterCanvasMousePosX = 0;
        double MasterCanvasMousePosY = 0;

        double MainCanvasRelativeMousePosX = 0;
        double MainCanvasRelativeMousePosY = 0;

        public void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {

            if (IsDraging) return;

            var pinterObjectMaster = e.GetCurrentPoint(MasterCanvas);

            MasterCanvasMousePosX = pinterObjectMaster.Position.X;
            MasterCanvasMousePosY = pinterObjectMaster.Position.Y;


            if (MainCanvas != null)
            {
                var pinterObjectMain = e.GetCurrentPoint(MainCanvas);
                MainCanvasRelativeMousePosX = pinterObjectMain.Position.X / MainCanvas.Width;
                MainCanvasRelativeMousePosY = pinterObjectMain.Position.Y / MainCanvas.Height;
            }



            if (ScrollingAnimation != null){
                ScrollingAnimation.OnPointerWheelChanged(sender, e);
            }
        }



        private double CurrentGridSpacingX = -1;
        private double CurrentGridSpacingY = -1;

        public void DrawGrid(Canvas canvas, double spacingX, double spacingY, double lineThickness, Rect region){
            CurrentGridSpacingX = spacingX;
            CurrentGridSpacingY = spacingY;

            ClearGrid(canvas); // Clear existing grid lines first

            double startX = region.X;
            double startY = region.Y;
            double width = region.Width;
            double height = region.Height;

            // Align the grid to world (absolute) coordinates
            double alignedStartX = startX - startX % spacingX;
            double alignedStartY = startY - startY % spacingY;

            // Draw vertical lines
            for (double x = alignedStartX; x <= startX + width; x += spacingX){
                var verticalLine = new Line{
                    StartPoint = new Point(x, startY),
                    EndPoint = new Point(x, startY + height),
                    Stroke = Brushes.LightGray,
                    StrokeThickness = lineThickness,
                    IsHitTestVisible = false,
                    Opacity = 0.3
                };
                gridLines.Add(verticalLine);
                canvas.Children.Add(verticalLine);
            }

            // Draw horizontal lines
            for (double y = alignedStartY; y <= startY + height; y += spacingY){
                var horizontalLine = new Line{
                    StartPoint = new Point(startX, y),
                    EndPoint = new Point(startX + width, y),
                    Stroke = Brushes.LightGray,
                    StrokeThickness = lineThickness,
                    IsHitTestVisible = false,
                    Opacity = 0.3
                };
                gridLines.Add(horizontalLine);
                canvas.Children.Add(horizontalLine);
            }
        }

        public void ClearGrid(Canvas canvas){
            foreach (var line in gridLines){
                canvas.Children.Remove(line);
            }
            gridLines.Clear();
        }







        private void SmoothScrollerTrigger(double Value){
            if (MainCanvas == null || MasterCanvas == null)
                return;

            if (IsDraging && ScrollingAnimation != null)
            {
                ScrollingAnimation.StopCurrentAnimation = true;
                return;
            }




            


            MainCanvas.Width -= Value * (MainCanvas.Height / MasterCanvas.Height);
            MainCanvas.Height -= Value * (MainCanvas.Height / MasterCanvas.Height);


            double minPixelSize = Math.Min(Width, Height);
            double maxPixelSizeX = GraphHeight * MaxZoomInRatio;
            double maxPixelSizeY = GraphHeight * MaxZoomInRatio;

            MainCanvas.Width = Math.Clamp(MainCanvas.Width, minPixelSize, maxPixelSizeX);
            MainCanvas.Height = Math.Clamp(MainCanvas.Height, minPixelSize, maxPixelSizeY);




            double calX = MainCanvasRelativeMousePosX * MainCanvas.Width - MasterCanvasMousePosX;
            double calY = MainCanvasRelativeMousePosY * MainCanvas.Height - MasterCanvasMousePosY;


            Canvas.SetLeft(MainCanvas, -calX);
            Canvas.SetTop(MainCanvas, -calY);

            Update();
        }



        private bool IsDraging = false;
        private double XPosMouseRelitaveToMainCanvase = 0;
        private double YPosMouseRelitaveToMainCanvase = 0;
        private void OnPointerPressedMainCanvas(object? sender, PointerPressedEventArgs e){

            var pos = e.GetPosition(MainCanvas);

            XPosMouseRelitaveToMainCanvase = pos.X;
            YPosMouseRelitaveToMainCanvase = pos.Y;
            IsDraging = true;

        }

        private void OnPointerReleasedMainCanvas(object? sender, PointerEventArgs e){
            IsDraging = false;
        }


        private void OnPointerMovedMainCanvas(object? sender, PointerEventArgs e){
            if (!IsDraging) return;
            if (MainCanvas == null || MasterCanvas == null) return;

            var currentPosition = e.GetPosition(MasterCanvas);

            Canvas.SetLeft(MainCanvas, currentPosition.X - XPosMouseRelitaveToMainCanvase);
            Canvas.SetTop(MainCanvas, currentPosition.Y - YPosMouseRelitaveToMainCanvase);

            Update();
        }




        private void ApplyGrid()
        {

            if (MainCanvas == null || MasterCanvas == null) return;

            double lineThickness = 1;

            int factor = (int)(MainCanvas.Width / MasterCanvas.Width);
            factor = Math.Max(1, factor);

            double TargedBoxes = Math.Pow(TargetGridSpacingScreen, 0.5);
            double zoomFactor = MainCanvas.Width / MasterCanvas.Width;

            double baseDivision = zoomFactor * TargedBoxes;
            double roundedDivision = Math.Pow(2, Math.Round(Math.Log(baseDivision, 2)));


            double spacingX = MainCanvas.Width / roundedDivision;
            double spacingY = MainCanvas.Height / roundedDivision;



            double canvasLeft = Canvas.GetLeft(MainCanvas);
            double canvasTop = Canvas.GetTop(MainCanvas);


            var visibleRegion = new Rect(
                -canvasLeft,
                -canvasTop,
                MasterCanvas.Width,
                MasterCanvas.Height);

            DrawGrid(MainCanvas, spacingX, spacingY, lineThickness, visibleRegion);


            if (Povit != null)
            {
                Canvas.SetLeft(Povit, (MainCanvas.Width - Povit.Width) / 2);
                Canvas.SetTop(Povit, (MainCanvas.Height - Povit.Height) / 2);
            }

        }

    }
}
