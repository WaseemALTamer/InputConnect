using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia;
using System;



namespace InputConnect.UI.Containers.Common
{
    public class TriStateToggle : Border
    {

        // unlike the universal buttons like back button and closing button this is not
        // universal and you are  required  to  handle  the  postion  and  size  of the
        // size of the button this button will also be drawn from  scratch so no Bitmap
        // involved


        private int _State = 1; // this could either be <0,1,2>
        public int State
        {
            get { return _State; }
            set { _State = value; }
        }

        private Action<int>? _Trigger;
        public Action<int>? Trigger
        {
            get { return _Trigger; }
            set { _Trigger = value; }
        }


        private Canvas? _MainCanvas;
        public Canvas? MainCanvas
        {
            get { return _MainCanvas; }
            set { _MainCanvas = value; }
        }

        private Border? Ball;
        private Animations.Transations.EaseInOut? BallPostionTranslation;

        private Animations.Transations.Uniform? OnHover;

        // this will only display the lock icone it is your responsiblity to not allow the button
        // to go anywhere by setting it back to what it was this is done to  help you detect when
        // the user is trying to change the state of the button
        private bool IsLocked = false;
        private Border? BorderLockImage;
        private Image? LockImage;

        private Animations.Transations.EaseInOut? LockShowHideAnimation;


        public TriStateToggle()
        {

            Background = new SolidColorBrush(Color.FromUInt32(0x7f1f1f1f));


            BorderBrush = new LinearGradientBrush{
                StartPoint = new RelativePoint(0, 0.5, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 0.5, RelativeUnit.Relative),
                GradientStops = new GradientStops{
                    new GradientStop(Colors.Transparent, 0),
                    new GradientStop(Color.FromUInt32(0x7f1f1f1f), 0.5),
                    new GradientStop(Colors.Transparent, 1)
                }
            };



            BorderThickness = new Thickness(3);

            CornerRadius = new CornerRadius(100);
            //BorderThickness = new Thickness(1);

            ClipToBounds = true;

            Width = 120;
            Height = 40;

            MainCanvas = new Canvas
            {
                Width = Width,
                Height = Height,
                ClipToBounds = true,
            };

            Ball = new Border{
                CornerRadius = new CornerRadius(100),
                Width = Height / 1.25,
                Height = Height / 1.25,
                Background = new SolidColorBrush(Colors.White)
            };


            MainCanvas.Children.Add(Ball);

            Canvas.SetLeft(Ball, (MainCanvas.Width - Ball.Width) / 2);
            Canvas.SetTop(Ball, (MainCanvas.Height - Ball.Height) / 2);

            Ball.PointerMoved += OnPointerMoveBall;
            Ball.PointerPressed += OnPointerPressBall;



            OnHover = new Animations.Transations.Uniform{
                StartingValue = 1,
                EndingValue = 0.5,
                Duration = Setting.Config.TransitionHover,
                Trigger = SetBallOpacity
            };


            PointerReleased += OnPointerReleased;
            PointerEntered += OnPointerEntered;
            PointerExited += OnPointerExited;


            BorderLockImage = new Border
            {
                CornerRadius = new CornerRadius(100),
                Width = Height / 1.25,
                Height = Height / 1.25,
                Background = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0)),
                Opacity = 0
            };
            Ball.Child = BorderLockImage;

            LockImage = new Image{
                Stretch = Stretch.Uniform,
                Width = Ball.Width * 0.8,
                Height = Ball.Height * 0.8,
            };
            BorderLockImage.Child = LockImage;
            Assets.AddAwaitedAction(() => {
                LockImage.Source = Assets.MiniLockBitmap;
            });

            LockShowHideAnimation = new Animations.Transations.EaseInOut
            {
                StartingValue = 0,
                EndingValue = 1,
                Duration = Setting.Config.TransitionDuration,
                Trigger = SetLockOpacity
            };

            Child = MainCanvas;

        }



        private void OnPointerEntered(object? sender, PointerEventArgs e) {
            if (IsLocked) return;

            if (OnHover == null) return;
            OnHover.TranslateForward();
        }

        private void OnPointerExited(object? sender, PointerEventArgs e){
            if (IsLocked) return;

            if (OnHover == null) return;
            OnHover.TranslateBackward();
        }

        private bool BallPressed = false;
        private double InitialMousePosX = 0; // this will be relitive to the ball
        private void OnPointerPressBall(object? sender, PointerEventArgs e) { 
            BallPressed = true;
            var pointerPosition = e.GetPosition(Ball);
            InitialMousePosX = pointerPosition.X;
            if (BallPostionTranslation != null){
                BallPostionTranslation.Pause();
                BallPostionTranslation.Reset();
            }
        }

        
        private void OnPointerMoveBall(object? sender, PointerEventArgs e) {
            if (BallPressed && Ball != null && MainCanvas != null) {
                var pointerPosition = e.GetPosition(MainCanvas);
                
                
                var posX = pointerPosition.X - InitialMousePosX;
                if (posX > MainCanvas.Width - Ball.Width)
                        posX = MainCanvas.Width - Ball.Width;
                if (posX < 0) posX = 0;



                Canvas.SetLeft(Ball, posX);
            }
        }


        private void OnPointerReleased(object? sender, PointerEventArgs e){
            e.Handled = true;
            
            if (e.GetCurrentPoint(null).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased){
                if (sender is Control control){
                    var pointerPosition = e.GetPosition(control);


                    if ((pointerPosition.X < 0 || pointerPosition.Y < 0) && !BallPressed) return;
                    if ((pointerPosition.X > Width || pointerPosition.Y > Height) && !BallPressed) return;

                    if (Ball != null){
                        var state = (int)(pointerPosition.X / (Width / 3));
                        if (state < 0) state = 0;
                        if (state > 2) state = 2;
                        SetState(state);
                        if (Trigger != null) Trigger.Invoke(State);
                    }
                    BallPressed = false;
                }
            }
        }


        private Vector InitialPos;
        private Vector FinalPos;
        public void SetBallPostionTranslate(double Xpos, double Ypos){
            if (Ball == null) return;
            if (BallPostionTranslation != null) {
                BallPostionTranslation.Pause();
                BallPostionTranslation.Reset();
            }

            BallPostionTranslation = new Animations.Transations.EaseInOut
            {
                StartingValue = 0,
                EndingValue = 1,
                Duration = Setting.Config.TransitionDuration,
                Trigger = SetBallPostionTrigger
            };

            if (double.IsNaN(Canvas.GetLeft(Ball)))
            { // this will only occures on start up
                Canvas.SetLeft(Ball, Xpos);
                Canvas.SetTop(Ball, Ypos);
                return;
            }

            InitialPos = new Vector(Canvas.GetLeft(Ball), Canvas.GetTop(Ball));
            FinalPos = new Vector(Xpos, Ypos);

            BallPostionTranslation.TranslateForward();
            
        }

        private void SetBallPostionTrigger(double value)
        {
            if (Ball != null){
                Canvas.SetLeft(Ball, InitialPos.X + (FinalPos.X - InitialPos.X) * value);
                Canvas.SetTop(Ball, InitialPos.Y + (FinalPos.Y - InitialPos.Y) * value);
            }
        }




        private void SetBallOpacity(double value){
            if (Ball == null || MainCanvas == null) return;
            Ball.Opacity = value;
        }

        private void SetLockOpacity(double value){
            if (BorderLockImage == null || MainCanvas == null) return;
            BorderLockImage.Opacity = value;
        }


        public void SetLock(bool state) {

            if (IsLocked == state) return;

            IsLocked = state;

            if (LockShowHideAnimation != null) {
                if (state)
                    LockShowHideAnimation.TranslateForward();
                else 
                    LockShowHideAnimation.TranslateBackward();
            }
        }

        public void SetState(int state){
            // we dont invoke the Trigger for this function because we already know the
            // state so we let the user trigger whatever they trying to run if the user
            // clicked the button then  the  <OnPointerReleased>  is  responsiable  for
            // triggering the state this also avoid a infinite looping in some cases


            if (state > 2 || state < 0) return;
            

            if (MainCanvas != null && 
                Ball != null)
            {
                double Xpos = (MainCanvas.Height - Ball.Height) / 2 + ((double)state/2) *
                    (MainCanvas.Width - (MainCanvas.Height - Ball.Height) - Ball.Width);

                double Ypos = (MainCanvas.Height - Ball.Height) / 2;

                SetBallPostionTranslate(Xpos, Ypos);
                
            }
            
            State = state;
        }
    }
}
