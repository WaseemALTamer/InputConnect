using Avalonia.Controls;
using System;




namespace InputConnect.UI.Containers
{
    public class SearchSpinnerMessage : Border
    {


        private Canvas? Master;

        private Canvas? _MainCanvas;


        private Border? SearchImageBoarder;
        private Image? SearchImage;

        private TextBlock? SearchMessage;
        

        public Canvas? MainCanvas{
            get { return _MainCanvas; }
            set { _MainCanvas = value; }
        }


        private Animations.Transations.EaseInOut? ShowHideTransition;
        private Animations.Transations.EaseInOut? SpinTransition;


        public SearchSpinnerMessage(Canvas? master)
        {
            Master = master;

            Width = 200;
            Height = 200;

            IsVisible = false;
            Opacity = 0;

            MainCanvas = new Canvas {
                Width = Width,
                Height = Height
            };

            SearchImageBoarder = new Border {
                Width = MainCanvas.Width / 2,
                Height = MainCanvas.Height / 2,
            };
            MainCanvas.Children.Add(SearchImageBoarder);

            SearchImage = new Image {
                Stretch = Avalonia.Media.Stretch.Uniform,
            };

            Assets.AddAwaitedAction(() => {
                SearchImage.Source = Assets.SearchBitmap;
            });
            SearchImageBoarder.Child = SearchImage;

            SearchMessage = new TextBlock{
                Text = "Looking for other devices",
                FontSize = Setting.Config.FontSize,
                Width = 250,
                Height = 50
            };
            MainCanvas.Children.Add(SearchMessage);


            ShowHideTransition = new Animations.Transations.EaseInOut{
                StartingValue = 0,
                EndingValue = 0.7,
                Duration = Setting.Config.TransitionDuration,
                Trigger = SetOpacity,
            };

            SpinTransition = new Animations.Transations.EaseInOut{
                StartingValue= 0,
                EndingValue = 1,
                Duration = 2000,
                Trigger = SetSpinPos
            };

            

            SizeChanged += OnSizeChanged;

            

            Child = MainCanvas;
        }

        public void OnSizeChanged(object? sender, object? e) {

            if (MainCanvas == null) return;


            if (SearchImageBoarder != null) {
                Canvas.SetLeft(SearchImageBoarder, (MainCanvas.Width - SearchImageBoarder.Width) / 2);
                Canvas.SetTop(SearchImageBoarder, 0);
            }

            if (SearchMessage != null) {
                Canvas.SetLeft(SearchMessage, (MainCanvas.Width - SearchMessage.Width) / 2);
                Canvas.SetTop(SearchMessage, -SearchMessage.Height);
            }


        }

        bool hidden = true; // this is used  because of  the  TranslateForward TraslateBackward bug
                            // which needs to be fixed where if the translation  is not running and
                            // you try to translate it will shift it back to have the oposite value
                            // and then translate it to the target value
        public void Show() {
            
            if (hidden == false) return;

            SpinTransition?.TranslateForward(); // this will start the spin transition for us

            ShowHideTransition?.TranslateForward();
            hidden = false;
        }

        public void Hide() {

            if (hidden == true) return;

            SpinTransition?.Reset(); // this will pause the spin and reset it but it will keep it in the same pos
            ShowHideTransition?.TranslateBackward();
            hidden = true;
        }


        public void SetOpacity(double value) {
            Opacity = value;
            IsVisible = Opacity != 0; // reducndency line for the Avoilonia Thread to ignore the window when not needed
        }



        public void SetSpinPos(double value) {
            if (SpinTransition == null) 
                return;

            if (SearchImageBoarder == null || 
                MainCanvas == null)
                return;

            

            double angle = value * 2 * Math.PI;

            double centerX = (MainCanvas.Width - SearchImageBoarder.Width) / 2;
            double centerY = (MainCanvas.Height - SearchImageBoarder.Height) / 2;
            double radiusX = centerX;
            double radiusY = centerY;
            Canvas.SetLeft(SearchImageBoarder, centerX + Math.Sin(angle) * radiusX);
            Canvas.SetTop(SearchImageBoarder, centerY + -Math.Cos(angle) * radiusY);


            if (SpinTransition != null &&
                SpinTransition.FunctionRunning == false)
            {
                if (SpinTransition.CurrentValue <= 0.5) {
                    SpinTransition.TranslateForward();
                }
                else{
                    SpinTransition.TranslateBackward();
                }

            }

        }


    }
}
