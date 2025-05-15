using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using InputConnect.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputConnect.UI.Containers.Common
{
    public class SlideButton : Border
    {

        // unlike the universal buttons like back button and closing button this is not
        // universal and you are  required  to  handle  the  postion  and  size  of the
        // size of the button this button will also be drawn from  scratch so no Bitmap
        // involved


        private bool _State = false;
        public bool State{
            get { return _State; }
            set { _State = value; }
        }

        private Action? _Trigger;
        public Action? Trigger{
            get { return _Trigger; }
            set { _Trigger = value; }
        }


        private Canvas? _MainCanvas;
        public Canvas? MainCanvas{
            get { return _MainCanvas; }
            set { _MainCanvas = value; }
        }

        private Border? Ball;
        private Animations.Transations.EaseInOut? BallTrnasition;

        private Animations.Transations.Uniform? OnHover;


        public SlideButton()
        {

            Background = new SolidColorBrush(Color.FromUInt32(0x7f1f1f1f));
            //BorderBrush = new SolidColorBrush(Color.FromUInt32(0xff1f1f1f));

            CornerRadius = new CornerRadius(100);
            //BorderThickness = new Thickness(1);

            Width = 80;
            Height = 40;

            MainCanvas = new Canvas
            {
                Width = Width,
                Height = Height,
            };

            Ball = new Border
            {
                CornerRadius = new CornerRadius(100),
                Width = Height / 1.25,
                Height = Height / 1.25,
                Background = new SolidColorBrush(Colors.White)
            };
            MainCanvas.Children.Add(Ball);

            Canvas.SetLeft(Ball, (MainCanvas.Height - Ball.Height) / 2);
            Canvas.SetTop(Ball, (MainCanvas.Height - Ball.Height) / 2);


            BallTrnasition = new Animations.Transations.EaseInOut
            {
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration,
                Trigger = SetBallPostion
            };

            OnHover = new Animations.Transations.Uniform
            {
                StartingValue = 1,
                EndingValue = 0.5,
                Duration = Config.TransitionHover,
                Trigger = SetBallOpacity
            };


            PointerReleased += OnPointerReleased;
            PointerEntered += OnHover.TranslateForward;
            PointerExited += OnHover.TranslateBackward;

            Child = MainCanvas;

        }



        private void OnPointerReleased(object? sender, PointerEventArgs e){


            e.Handled = true;
            if (e.GetCurrentPoint(null).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased){
                if (sender is Control control){
                    var pointerPosition = e.GetPosition(control);
                    if (pointerPosition.X < 0 || pointerPosition.Y < 0) return;
                    if (pointerPosition.X > Width || pointerPosition.Y > Height) return;

                    if (Ball != null){
                        if (BallTrnasition != null){
                            if (State == false) BallTrnasition.TranslateForward();
                            if (State == true) BallTrnasition.TranslateBackward();
                        }
                        State = !State;

                        if (Trigger != null) Trigger.Invoke();
                    }
                }
            }
        }


        private void SetBallPostion(double value){
            if (Ball == null || MainCanvas == null) return;
            double Xpos = (MainCanvas.Height - Ball.Height) / 2 + value *
                (MainCanvas.Width - (MainCanvas.Height - Ball.Height) - Ball.Width);
            Canvas.SetLeft(Ball, Xpos);
            Canvas.SetTop(Ball, (MainCanvas.Height - Ball.Height) / 2);

        }

        private void SetBallOpacity(double value){
            if (Ball == null || MainCanvas == null) return;
            Ball.Opacity = value;

        }



    }
}
