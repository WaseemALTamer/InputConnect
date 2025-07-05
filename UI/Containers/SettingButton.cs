using System.Threading.Tasks;
using InputConnect.Setting;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System;


namespace InputConnect.UI.Containers
{
    public class SettingButton : Border
    {

        private Canvas? Master;
        private Image? ButtonImage;

        private Animations.Transations.Uniform? HoverTranstion;
        private Animations.Transations.EaseOut? ShowHideTransation;
        private Animations.Transations.Uniform? RotationAnimation;  


        private Action? _Trigger;
        public Action? Trigger{
            get { return _Trigger; }
            set { _Trigger = value; }
        }

        public SettingButton(Canvas? master = null){
            Master = master;
            Focusable = true;
            //Background = Themes.Buttons;

            ShowHideTransation = new Animations.Transations.EaseOut{
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration,
                Damping = 2,
                Trigger = ShowHideTrigger
            };

            HoverTranstion = new Animations.Transations.Uniform{
                StartingValue = 1,
                EndingValue = 0.5,
                Duration = Config.TransitionHover,
                Trigger = SetOpacity
            };

            RotationAnimation = new Animations.Transations.Uniform{
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration,
                Trigger = SetRotation
            };



            ButtonImage = new Image{
                Stretch = Stretch.Uniform,
            };
            Assets.AddAwaitedAction(() => {
                ButtonImage.Source = Assets.SettingButtonBitmap;
            });

            Child = ButtonImage;
            Width = 60; Height = 45; // the Size is static



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
                    ShowHideTransation.FunctionRunning == false){
                    if (IsShowing){
                        Canvas.SetLeft(this, Master.Width - Width);
                        Canvas.SetTop(this, Master.Height - Height - 15);
                    }
                    else{
                        Canvas.SetLeft(this, Master.Width - Width);
                        Canvas.SetTop(this, Master.Height - Height - 15);
                    }
                }
            }
        }



        bool IsShowing = false;
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
            Canvas.SetLeft(this, Master.Width - ((Width) * Value));
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
                }
            }
        }



        public void RotateAntiClockwise() {
            if (RotationAnimation == null) return;
            RotationAnimation.TranslateForward();
        }

        public void RotateClockwise(){
            if (RotationAnimation == null) return;
            RotationAnimation.TranslateBackward();
        }

        public void SetRotation(double value) { 
            RenderTransform = new RotateTransform {
                Angle = -90 * value,            // Degrees to rotate
            };
        }

        public async void HideShowAnimation(){
            Hide();
            await Task.Delay(300);
            Show();
        }
    }
}
