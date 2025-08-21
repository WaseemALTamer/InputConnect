using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;
using InputConnect.Setting;
using SkiaSharp;
using System;

namespace InputConnect.UI.Containers
{
    public class Timer : Border
    {

        //private Canvas? Master;
        
        private CircleControl? circle;
        private Animations.Transations.Uniform? TimerAnimation;
        private Animations.Transations.Uniform? HoverTranstion;



        private Action? _Trigger;
        public Action? Trigger{
            get { return _Trigger; }
            set { _Trigger = value; }
        }


        private double _Thickness = 10;
        public double Thickness{
            get => _Thickness;
            set{
                if (_Thickness != value){
                    _Thickness = value;
                    OnSizeChanged(); // trigger your function here
                }
            }
        }



        public Timer()
        {
            IsHitTestVisible = true;
            Background = Brushes.Transparent; // this is to make it hitable


            if (double.IsNaN(Width)){
                Width = 50;
            }

            if (double.IsNaN(Height)){
                Height = 50;
            }

            CornerRadius = new CornerRadius(Math.Min(Width, Height));

            circle = new CircleControl{
                Width = Width,
                Height = Height,
            };


            HoverTranstion = new Animations.Transations.Uniform
            {
                StartingValue = 1,
                EndingValue = 0.5,
                Duration = Config.TransitionHover,
                Trigger = SetOpacity
            };

            PointerEntered += HoverTranstion.TranslateForward;
            PointerExited += HoverTranstion.TranslateBackward;

            SizeChanged += OnSizeChanged;

            PointerReleased += OnClick;


            Child = circle;
        }

        private void SetOpacity(double value) {
            Opacity = value;
            IsVisible = value != 0;
        }







        private void OnSizeChanged(object? sender = null, object? e = null) {

            CornerRadius = new CornerRadius(Math.Min(Width, Height));

            if (circle != null) {
                circle.Width = Width;
                circle.Height = Height;
                circle.Thinkness = Thickness;
            }
        }


        // this will keep track if the timer is running
        private bool _Running = false;
        public bool Running{
            get { return _Running; }
            set { _Running = value; }
        }

        private void AnimationTrigger(double value) {
            SetValue(value * 360);

            if (TimerAnimation != null &&
                TimerAnimation.FunctionRunning == false &&
                Trigger != null)
            {
                Trigger.Invoke();
                Running = false;
            }
        }



        public void StartTimer(double time) {

            if (TimerAnimation != null) {
                TimerAnimation.Reset();
            }


            // this function takes time as of int in ms
            TimerAnimation = new Animations.Transations.Uniform{
                StartingValue = 0,
                EndingValue = 1,
                Duration = time,
                Trigger = AnimationTrigger
            };

            TimerAnimation.TranslateForward();
            Running = true;
        }

        public void Pause() {
            if (TimerAnimation == null) return;
            TimerAnimation.Pause();
            Running = false;
        }

        public void Resume() {
            if (TimerAnimation == null) return;
            TimerAnimation.Resume();
            Running = true;
        }


        public void SetValue(double dgree){
            // this will set the angle for you and render the circule
            // it is adviced you dont run this  when you  are running
            // the timer function
            if (circle == null) return;
            circle.SweepAngle = dgree;
            circle.InvalidateVisual();
        }


        private void OnClick(object? sender, PointerReleasedEventArgs e){

            e.Handled = true;
            if (e.GetCurrentPoint(null).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased){
                if (sender is Control control){
                    var pointerPosition = e.GetPosition(control);
                    if (pointerPosition.X < 0 || pointerPosition.Y < 0) return;
                    if (pointerPosition.X > Width || pointerPosition.Y > Height) return;

                    if (Running){
                        Pause();
                    }
                    else {
                        Resume();
                    }
                }
            }
        }

        private class CircleControl : Control
        {
            public double StartAngle = -90;
            public double SweepAngle = 0;
            public double Thinkness = 10;

            public override void Render(DrawingContext context)
            {
                base.Render(context);

                var center = new Point(Bounds.Width / 2, Bounds.Height / 2);
                var radius = (Math.Min(Bounds.Width, Bounds.Height) - Thinkness) / 2;





                var arcPen = new Pen(Themes.Timer, Thinkness);

                if (SweepAngle >= 360){ 
                    // this ensures effecincy
                    context.DrawEllipse(null, arcPen, center, radius, radius);
                }

                var fadedColor = Color.FromArgb((byte)(
                    Themes.Timer.Color.A * 0.2),
                    Themes.Timer.Color.R,
                    Themes.Timer.Color.R,
                    Themes.Timer.Color.R);

                var fadedBrush = new SolidColorBrush(fadedColor);

                var fullCirclePen = new Pen(fadedBrush, Thinkness);

                context.DrawEllipse(null, fullCirclePen, center, radius, radius);




                double startRad = StartAngle * Math.PI / 180;
                double endRad = (StartAngle + SweepAngle) * Math.PI / 180;

                var startPoint = new Point(
                    center.X + radius * Math.Cos(startRad),
                    center.Y + radius * Math.Sin(startRad));

                var endPoint = new Point(
                    center.X + radius * Math.Cos(endRad),
                    center.Y + radius * Math.Sin(endRad));

                var arcSegment = new ArcSegment
                {
                    Point = endPoint,
                    Size = new Size(radius, radius),
                    RotationAngle = 0,
                    IsLargeArc = SweepAngle > 180,
                    SweepDirection = SweepDirection.Clockwise
                };

                var pathFigure = new PathFigure
                {
                    StartPoint = startPoint,
                    Segments = new PathSegments { arcSegment },
                    IsClosed = false
                };

                var geometry = new PathGeometry
                {
                    Figures = new PathFigures { pathFigure }
                };

                context.DrawGeometry(null, arcPen, geometry);
            }
        }
    }

}
