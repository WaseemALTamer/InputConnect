using Avalonia.Controls;
using Avalonia.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace InputConnect.UI.Containers.Common
{
    class TrashBinButton: Border
    {
        // later on adapt this class to have a shaking bin cover later on

        public Action? Trigger;


        private Border? TrashBinBorder;
        private Image? TrashBinImage;



        private Animations.Transations.EaseInOut? HoverTranslation;



        public TrashBinButton() {

            Width = 60; Height = 90;

            Opacity = 0.7;

            Background = Setting.Themes.TrashBin;

            CornerRadius = new Avalonia.CornerRadius(Setting.Config.CornerRadius);



            TrashBinBorder = new Border{
                Width = Width - 10,
                Height = Height - 10,
                Opacity = 0.7
            };

            TrashBinImage = new Image{
                Stretch = Avalonia.Media.Stretch.Uniform,
            };
            Assets.AddAwaitedAction(() => {
                TrashBinImage.Source = Assets.TrashBinBitmap;
            });

            TrashBinBorder.Child = TrashBinImage;

            Child = TrashBinBorder;


            SizeChanged += OnSizeChange;



            HoverTranslation = new Animations.Transations.EaseInOut{
                StartingValue = 0.7,
                EndingValue = 1,
                CurrentValue = 1,
                Duration = Setting.Config.TransitionHover * 2,
                Trigger = SetOpacity
            };

            PointerEntered += HoverTranslation.TranslateForward;
            PointerExited += HoverTranslation.TranslateBackward;
            PointerReleased += OnClick;

        }



        private void OnSizeChange(object? sender, object? e) {

            if (TrashBinBorder != null){
                Canvas.SetLeft(TrashBinBorder, (Width - TrashBinBorder.Width) / 2);
                Canvas.SetTop(TrashBinBorder, (Height - TrashBinBorder.Height) / 2);
            }

        }


        private void SetOpacity(double value) {
            Opacity = value;


        
        
        }



        private void OnClick(object? sender, PointerReleasedEventArgs e){

            e.Handled = true;
            if (e.GetCurrentPoint(null).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased){
                if (sender is Control control)
                {
                    var pointerPosition = e.GetPosition(control);
                    if (pointerPosition.X < 0 || pointerPosition.Y < 0) return;
                    if (pointerPosition.X > Width || pointerPosition.Y > Height) return;

                    if (Trigger != null) Trigger();
                }
            }
        }







    }
}
