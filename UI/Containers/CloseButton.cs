using System.Threading.Tasks;
using InputConnect.Setting;
using Avalonia.Controls;
using Avalonia.Input;
using System;


namespace InputConnect.UI.Containers
{
    public class CloseButton : Border
    {

        private Canvas? Master;
        private Image? ButtonImage;

        private Animations.Transations.Uniform? HoverTranstion;
        private Animations.Transations.EaseOut? ShowHideTransation;


        private Action? _Trigger;
        public Action? Trigger{
            get { return _Trigger; }
            set { _Trigger = value; }
        }

        public CloseButton(Canvas? master = null)
        {
            Master = master;
            Focusable = true;

            //Background = Themes.Buttons;

            ShowHideTransation = new Animations.Transations.EaseOut
            {
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration,
                Damping = 2,
                Trigger = ShowHideTrigger
            };

            HoverTranstion = new Animations.Transations.Uniform
            {
                StartingValue = 1,
                EndingValue = 0.5,
                Duration = Config.TransitionHover,
                Trigger = SetOpacity
            };

            ButtonImage = new Image
            {
                Stretch = Avalonia.Media.Stretch.Uniform,
            };
            Assets.AddAwaitedAction(() => {
                ButtonImage.Source = Assets.CloseButtonBitmap;
            });

            IsHitTestVisible = true;
            Width = 55; Height = 30; // the Size is static
            Child = ButtonImage;
            



            if (Master != null){
                OnSizeChanged();
                Master.SizeChanged += OnSizeChanged;
            }

            PointerReleased += OnClick;
            PointerEntered += HoverTranstion.TranslateForward;
            PointerExited += HoverTranstion.TranslateBackward;
        }




        public void OnSizeChanged(object? sender = null, SizeChangedEventArgs? e = null){
            if (Master != null){
                if (ShowHideTransation != null &&
                    ShowHideTransation.FunctionRunning == false)
                {
                    if (IsShowing){
                        Canvas.SetLeft(this, (Master.Width - Width));
                        Canvas.SetTop(this, 15);
                    }
                    else{
                        Canvas.SetLeft(this, Master.Width);
                        Canvas.SetTop(this, 15);
                    }
                }
            }
        }



        private bool IsShowing = false;
        public void Show(){
            if (ShowHideTransation == null) return;
            IsShowing = true;
            ShowHideTransation.TranslateForward();
        }

        public void Hide(){
            if (ShowHideTransation == null) return;
            IsShowing = false;
            ShowHideTransation.TranslateBackward();
        }

        public void ShowHideTrigger(double Value){
            if (Master == null) return;
            Canvas.SetLeft(this, Master.Width - (Width * Value));
            IsVisible = Value != 0;
        }

        public void SetOpacity(double Value){
            Opacity = Value;
            IsVisible = Value != 0;
        }


        private void OnClick(object? sender, PointerReleasedEventArgs e){

            e.Handled = true;
            if (e.GetCurrentPoint(null).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased){
                if (sender is Control control){
                    var pointerPosition = e.GetPosition(control);
                    if (pointerPosition.X < 0 || pointerPosition.Y < 0) return;
                    if (pointerPosition.X > Width || pointerPosition.Y > Height) return;

                    if (ShowHideTransation != null && ShowHideTransation.FunctionRunning == true) return;
                    if (Trigger != null) Trigger();

                    //HideShowAnimation();
                }
            }
        }

        public async void HideShowAnimation(){
            Hide();
            await Task.Delay(Config.TransitionDuration);
            Show();
        }
    }
}
