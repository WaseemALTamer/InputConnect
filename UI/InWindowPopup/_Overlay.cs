using Avalonia.Controls;


namespace InputConnect.UI.InWindowPopup
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


        private int OverlayNumKeep = 0;  // this will track how many popup will need the overlay
                                         // in such a way if there is  two at  the same time the
                                         // and you close  one popup  it will stay  until all of 
                                         // them are closed 

        public Overlay(Canvas master)
        {

            Master = master;
            Focusable = true;

            if (Master != null) { 
                Master.Children.Add(this);
            }

            Opacity = 0;
            IsVisible = false;
            ClipToBounds = true;
            IsHitTestVisible = true;
            Background = Setting.Themes.DimOverlay;
            ZIndex = 5;


            ShowHideTransition = new Animations.Transations.Uniform
            {
                StartingValue = 0,
                EndingValue = 1,
                Duration = Setting.Config.TransitionDuration,
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
            
            OverlayNumKeep += 1;
            if (IsDisplayed == false) {
                IsDisplayed = true;
                if (ShowHideTransition != null)
                    ShowHideTransition.TranslateForward();

            }
        }


        public void Hide(){
            OverlayNumKeep -= 1;
            if (OverlayNumKeep == 0) {
                IsDisplayed = false;
                if (ShowHideTransition != null)
                    ShowHideTransition.TranslateBackward();

            }

        }


        public void SetOpeicity(double Value)
        {
            Opacity = Value;
            IsVisible = Opacity != 0; // reducndency line for the Avoilonia Thread to ignore the window when not needed
        }

    }
}
