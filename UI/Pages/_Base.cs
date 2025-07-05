using InputConnect.UI.Animations;
using Avalonia.Interactivity;
using InputConnect.Setting;
using Avalonia.Controls;
using Avalonia;
using System;







namespace InputConnect.UI.Pages
{
    public class Base : Border
    {
        private Canvas? _Master;
        public Canvas? Master{
            get { return _Master; }
            set { _Master = value; }
        }

        private Canvas? _MainCanvas;
        public Canvas? MainCanvas{
            get { return _MainCanvas; }
            set { _MainCanvas = value; }
        }


        private bool _IsDisplayed = false;
        public bool IsDisplayed{
            get { return _IsDisplayed; }
            set { _IsDisplayed = value; }
        }


        private Action? _OnShow;
        public Action? OnShow{
            get { return _OnShow; }
            set { _OnShow = value; }
        }


        private Action? _OnHide;
        public Action? OnHide{
            get { return _OnHide; }
            set { _OnHide = value; }
        }



        private ScrollViewer? ScrollViewer;
        private SmoothScrolling? ScrollingAnimation;



        private Animations.Transations.Uniform? ShowHideTransition;


        public Base(Canvas? master){
            Master = master;
            Focusable = true;

            if (Master != null){
                Master.Children.Add(this);
            }


            Opacity = 0;
            IsVisible = false;
            ClipToBounds = true;
            IsHitTestVisible = true;
            CornerRadius = new CornerRadius(Config.CornerRadius);
            Background = Themes.Page;


            ShowHideTransition = new Animations.Transations.Uniform{
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration,
                Trigger = SetOpeicity,
            };

            ScrollViewer = new ScrollViewer{
                IsHitTestVisible = true,
                VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            };

            ScrollingAnimation = new Animations.SmoothScrolling{
                Trigger = SmoothVerticalScrollerTrigger
            };
            ScrollViewer.AddHandler(PointerWheelChangedEvent, ScrollingAnimation.OnPointerWheelChanged, RoutingStrategies.Tunnel);

            MainCanvas = new Canvas{
                IsHitTestVisible = true,
                ClipToBounds = true
            };

            ScrollViewer.Content = MainCanvas;
            MainCanvas.PointerWheelChanged += ScrollingAnimation.OnPointerWheelChanged;


            if (Master != null){
                OnResize(); // trigger the function to set the sizes
                Master.SizeChanged += OnResize;
            }

            Child = ScrollViewer;
        }

        private void OnResize(object? sender = null, SizeChangedEventArgs? e = null){
            if (Master != null){
                Width = Master.Width * 0.85;
                Height = Master.Height - 100;

                if (MainCanvas != null){
                    MainCanvas.Width = Width;
                    MainCanvas.MinHeight = Height;

                    if (double.IsNaN(MainCanvas.Height)) 
                        MainCanvas.Height = Height; // this fixes a bug that i had where other containers cant detect the MainCanvas height even when minHeight is set


                }

                if (ScrollViewer != null){
                    ScrollViewer.Width = Width;
                    ScrollViewer.Height = Height;
                }

                Canvas.SetRight(this, (Master.Width - Width) / 2);
                Canvas.SetTop(this, 80);
            }
        }

        private void SetOpeicity(double Value){
            Opacity = Value;
            IsVisible = Opacity != 0; // reducndency line for the Avoilonia Thread to ignore the window when not needed
        }

        private void SmoothVerticalScrollerTrigger(double Value){
            if (ScrollViewer != null){
                ScrollViewer.Offset = new Vector(ScrollViewer.Offset.X, ScrollViewer.Offset.Y + Value);
            }
        }



        public void Show(){
            IsDisplayed = true;
            if (ShowHideTransition != null)
                ShowHideTransition.TranslateForward();

            if (OnShow != null) OnShow.Invoke();
        }

        public void Hide(){
            IsDisplayed = false;
            if (ShowHideTransition != null)
                ShowHideTransition.TranslateBackward();

            if (OnHide != null) OnHide.Invoke();
        }

    }
}
