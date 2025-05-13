using InputConnect.Setting;
using Avalonia.Controls;


namespace InputConnect.UI.WindowPopup
{
    public class Overlay : Border
    {
        // this class will be used to dim the content below the popups atleast for most cases


        private Canvas? _Master;
        public Canvas? Master
        {
            get { return _Master; }
            set { _Master = value; }
        }

        private Canvas? _MainCanvas;
        public Canvas? MainCanvas
        {
            get { return _MainCanvas; }
            set { _MainCanvas = value; }
        }

        private bool _IsDisplayed = false;
        public bool IsDisplayed
        {
            get { return _IsDisplayed; }
            set { _IsDisplayed = value; }
        }

        private Animations.Transations.Uniform? ShowHideTransition;

        public Overlay(Canvas master)
        {

            Master = master;

            if (Master != null) { 
                Master.Children.Add(this);
            }

            Opacity = 0;
            IsVisible = false;
            ClipToBounds = true;
            IsHitTestVisible = true;
            Background = Themes.DimOverlay;
            ZIndex = 5;


            ShowHideTransition = new Animations.Transations.Uniform
            {
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration,
                Trigger = SetOpeicity,
            };



            if (Master != null)
            {
                OnResize(); // trigger the function to set the sizes
                Master.SizeChanged += OnResize;
            }

        }

        public void OnResize(object? sender = null, SizeChangedEventArgs? e = null)
        {
            if (Master != null)
            {
                Width = Master.Width;
                Height = Master.Height;
            }
        }


        public void Show(){
            IsDisplayed = true;
            if (ShowHideTransition != null)
                ShowHideTransition.TranslateForward();
        }


        public void Hide()
        {
            IsDisplayed = false;
            if (ShowHideTransition != null)
                ShowHideTransition.TranslateBackward();
        }


        public void SetOpeicity(double Value)
        {
            Opacity = Value;
            IsVisible = Opacity != 0; // reducndency line for the Avoilonia Thread to ignore the window when not needed
        }

    }
}
