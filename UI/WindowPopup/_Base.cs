using InputConnect.UI.Containers;
using InputConnect.Setting;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia;
using System;



namespace InputConnect.UI.WindowPopup
{
    public class Base : Border
    {
        // this is the file that all the popups will use, just a general border of a base
        // you can use it for anything as long as it is a inside window popup

        private Canvas? _Master;
        public Canvas? Master {
            get { return _Master; }
            set { _Master = value; }
        }

        private Canvas? _MainCanvas;
        public Canvas? MainCanvas{
            get { return _MainCanvas; }
            set { _MainCanvas = value; }
        }

        private Action? _CloseButtonTrigger;
        public Action? CloseButtonTrigger{
            get { return _CloseButtonTrigger; }
            set { _CloseButtonTrigger = value; }
        }


        private bool _IsDisplayed = false;
        public bool IsDisplayed{
            get { return _IsDisplayed; }
            set { _IsDisplayed = value; }
        }

        private Animations.Transations.EaseInOut? PostionTranslation;
        private CloseButton? CloseButton;


        // this will be the midel of the screen
        private double XPos = 0.25;  // between 0 - 1 zero  being in the midel and 1 being at the end
        private double YPos = 0.325; // the anochor point is top left  and the cords are  already set
                                     // for the middle, do the calculation before you change them
                                     // the two var will only be used for draging perposes and
                                     // setting the postion of the on screen





        public Base(Canvas master) {


            Master = master;

            if (Master != null){
                Master.Children.Add(this);

                if (Global.Overlay == null){
                    Global.Overlay = new Overlay(master); // create the public varable for the other popups
                }
            }




            // those Vars are changable
            Opacity = 1;
            IsVisible = false;
            ClipToBounds = true;
            IsHitTestVisible = false;
            CornerRadius = new CornerRadius(Config.CornerRadius);
            Background = Themes.InWindowPopup;
            ZIndex = 6; // this makes sure it sets on top of everything

            
            MinWidth = 500; 
            MinHeight = 210;

            MaxWidth = 715;
            MaxHeight = 250;



            PointerPressed += OnPointerPressed;
            PointerReleased += OnPointerReleased;
            PointerMoved += OnPointerMoved;



            MainCanvas = new Canvas{
                IsHitTestVisible = true,
            };


            if (Master != null){
                OnResize(); // trigger the function to set the sizes
                Master.SizeChanged += OnResize;
            }

            // create the button after setting the size
            CloseButton = new CloseButton(MainCanvas);
            MainCanvas.Children.Add(CloseButton);
            CloseButton.Show();

            CloseButton.Trigger += OnClickClosingButton;



            Child = MainCanvas;
        }

        private void OnResize(object? sender = null, SizeChangedEventArgs? e = null)
        {
            if (Master != null){
                Width = Master.Width * 0.50;
                Height = Master.Height * 0.35;

                // also the ui will stop at the given min width our Width and height will still be
                // assigned and it wont correct it self this if  statment  will attempt to correct
                // it maually

                if (Width < MinWidth) Width = MinWidth; // width corrected for Min
                if (Height < MinHeight) Height = MinHeight; // height corrected for Min

                if (Width > MaxWidth) Width = MaxWidth; // width corrected for Max
                if (Height > MaxHeight) Height = MaxHeight; // height corrected for Max


                Canvas.SetTop(this, Master.Height * YPos); // we can set the Ypos because it is not effected by the animation
                if (PostionTranslation != null &&
                    PostionTranslation.FunctionRunning == false &&
                    _IsDisplayed)
                {
                    Canvas.SetLeft(this, Master.Width * XPos);
                }


                if (MainCanvas != null){
                    MainCanvas.Width = Width;
                    MainCanvas.Height = Height;
                }
            }
        }



        private Vector InitialPos;
        private Vector FinalPos;
        public void SetPostionTranslate(double Xpos, double Ypos)
        {


            if (Master == null) return;

            if (PostionTranslation != null) PostionTranslation.Reset();


            PostionTranslation = new Animations.Transations.EaseInOut{
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration * 2,
                Trigger = SetPostionTrigger
            };


            if (double.IsNaN(Canvas.GetLeft(this))){ // this will only occures on start up
                Canvas.SetLeft(this, Xpos);
                Canvas.SetTop(this, Ypos);
                return;
            }


            InitialPos = new Vector(Canvas.GetLeft(this) / Master.Width, Canvas.GetTop(this) / Master.Height);
            FinalPos = new Vector(Xpos / Master.Width, Ypos / Master.Height);

            PostionTranslation.TranslateForward();

        }

        private void SetPostionTrigger(double value){
            if (Master == null) return;

            var _XPos = (InitialPos.X + (FinalPos.X - InitialPos.X) * value) * Master.Width;
            var _YPos = (InitialPos.Y + (FinalPos.Y - InitialPos.Y) * value) * Master.Height;
            Canvas.SetLeft(this, _XPos);
            Canvas.SetTop(this, _YPos);

            // update the global postion 



            if (Canvas.GetLeft(this) >= Master.Width || Canvas.GetLeft(this) <= -(Width) ||
                Canvas.GetTop(this) >= Master.Height || Canvas.GetTop(this) <= -(Height))
            {
                IsVisible = false;
                IsHitTestVisible = false;
            }
            else{
                IsVisible = true;
                IsHitTestVisible = true;
            }

        }

        public void Show(){
            if (Master == null) return;
            Canvas.SetLeft(this, -Width); // just incase it is not already set

            _IsDisplayed = true;
            SetPostionTranslate(Master.Width * XPos, Canvas.GetTop(this));
            if (Global.Overlay != null){
                Global.Overlay.Show();
            }
        }

        public void Hide(){
            if (Master == null) return;
            _IsDisplayed = false;
            SetPostionTranslate(-Width, Canvas.GetTop(this));
            if (Global.Overlay != null){
                Global.Overlay.Hide();
            }
        }


        public void HideRight(){
            if (Master == null) return;
            _IsDisplayed = false;
            SetPostionTranslate(Master.Width, Canvas.GetTop(this));
            if (Global.Overlay != null){
                Global.Overlay.Hide();
            }
        }


        public void OnClickClosingButton(){
            Hide();
            
            if (CloseButtonTrigger != null) 
                CloseButtonTrigger.Invoke();
        }


        private bool IsDraging = false;
        private double XPosMouseRelitaveToThis = 0;
        private double YPosMouseRelitaveToThis = 0;
        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {

            if (CloseButton != null)
            {
                var _posToButton = e.GetPosition(CloseButton);
                if (_posToButton.X >= 0 && _posToButton.X < CloseButton.Width + 10 &&
                     _posToButton.Y >= 0 && _posToButton.Y < CloseButton.Height + 10) // i added 10 for safty 
                {
                    return; // this means we clicked on the closing button
                }
            }

            var pos = e.GetPosition(this);
            if (pos.Y < Height * 0.2)
            {
                XPosMouseRelitaveToThis = pos.X;
                YPosMouseRelitaveToThis = pos.Y;
                IsDraging = true;
            }
        }

        private void OnPointerReleased(object? sender, PointerEventArgs e)
        {
            IsDraging = false;
        }


        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (Master == null) return;
            if (!IsDraging) return;


            var currentPosition = e.GetPosition(Master);
            //if (currentPosition.X >= Master.Width || currentPosition.X <= 0 ||
            //    currentPosition.Y >= Master.Height || currentPosition.Y <= 0) 
            //        return;

            if (currentPosition.X <= Master.Width && currentPosition.X >= 0)
            {
                Canvas.SetLeft(this, currentPosition.X - XPosMouseRelitaveToThis);
                XPos = (currentPosition.X - XPosMouseRelitaveToThis) / Master.Width;
            }

            if (currentPosition.Y <= Master.Height && currentPosition.Y >= 0)
            {
                Canvas.SetTop(this, currentPosition.Y - YPosMouseRelitaveToThis);
                YPos = (currentPosition.Y - YPosMouseRelitaveToThis) / Master.Height;
            }
        }
    }
}
