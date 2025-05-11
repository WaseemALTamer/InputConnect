using InputConnect.Structures;
using InputConnect.Setting;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia;
using System;





namespace InputConnect.UI.Containers
{

    // this file will not change anything it is simply just for decoration and user interaction
    // to change the config property value you should look at  SettingButton.cs  because  it is
    // the file responsiable for changing all the values of the config file

    public class ConfigProperty : Border
    {


        private Canvas? Master;
        private Canvas? MainCanvas;

        private Animations.Transations.Uniform? ShowHideTransition;
        private Animations.Transations.Uniform? PostionTranslation;
        private Animations.Transations.Uniform? HoverTranslation;


        private TextBlock? _Text;
        public TextBlock? Text{
            get { return _Text; }
            set { _Text = value; }
        }

        private TextBox? _Entery;

        public TextBox? Entery{
            get { return _Entery; }
            set { _Entery = value; }
        }


        public ConfigProperty(Canvas? master = null, string? Label = null)
        {
            Master = master;
            if (Master != null) Master.Children.Add(this);

            ClipToBounds = true;
            IsVisible = false;
            Opacity = 0;

            MainCanvas = new Canvas();

            ShowHideTransition = new Animations.Transations.Uniform{
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration,
                Trigger = ShowHideSetOpeicity,
            };

            if (Master != null){
                Update(); // trigger the on resize so we can set the dimensions
                Master.SizeChanged += OnSizeChanged;
                CornerRadius = new CornerRadius(Config.CornerRadius);
            }

            HoverTranslation = new Animations.Transations.Uniform{
                StartingValue = 0.5,
                EndingValue = 0.3,
                CurrentValue = 1,
                Duration = Config.TransitionHover,
                Trigger = OnHoverSetBackground
            };

            PointerEntered += HoverTranslation.TranslateForward;
            PointerExited += HoverTranslation.TranslateBackward;


            Text = new TextBlock{
                Text = $"{Label}",
                Width = 400,
                Height = 20,
                FontSize = Config.FontSize
            };

            Entery = new TextBox{
                Width = 200,
                Height = 40,
                FontSize = Config.FontSize,
                CornerRadius = new CornerRadius(Config.CornerRadius),
                Background = Themes.Entry,
            };


            if (MainCanvas != null)
            {
                MainCanvas.Children.Add(Text);
                MainCanvas.Children.Add(Entery);
                Child = MainCanvas;
            }

            Background = Themes.ConfigProperty;
        }


        public string GetValue() {
            if (Entery == null || Entery.Text == null) return "0";
            return Entery.Text; // this will return the value in a string look at the DataType True being int and change it your self
        }

        public void SetValue(string value) {
            if (Entery == null) return;
            Entery.Text = value;
        }


        public void OnSizeChanged(object? sender = null, SizeChangedEventArgs? e = null)
        {
            if (Master != null)
            {
                Width = Master.Width - 100;
                Height = 75;

                if (MainCanvas != null)
                {
                    MainCanvas.Width = Width;
                    MainCanvas.Height = Height;

                    if (Text != null){
                        Canvas.SetLeft(Text, 10);
                        Canvas.SetTop(Text, (MainCanvas.Height - Text.Height) / 2);
                    }
                    if (Entery != null)
                    {
                        Canvas.SetLeft(Entery, MainCanvas.Width - Entery.Width - 20);
                        Canvas.SetTop(Entery, (MainCanvas.Height - Entery.Height) / 2);
                    }

                }

            }
        }

        public void Update()
        {
            OnSizeChanged(); // update the size of it just in case

            if (MainCanvas == null) return;

            // we can update more things if we need to here other than the size the size is seperate now


        }

        public void Show()
        {
            if (Opacity == 1) return;

            if (ShowHideTransition != null)
                ShowHideTransition.TranslateForward();
        }

        public void Hide()
        {
            if (Opacity == 0) return;
            if (ShowHideTransition != null)
                ShowHideTransition.TranslateBackward();
        }

        private bool IsKill = false;
        public void Kill(){
            if (ShowHideTransition != null){
                ShowHideTransition.TranslateBackward();
                IsKill = true;
            }
        }

        public void ShowHideSetOpeicity(double Value)
        {

            //if (HoverTranslation != null &&
            //    Value <= HoverTranslation.CurrentValue) {
            //    Opacity = Value;
            //}

            Opacity = Value;

            IsVisible = Opacity != 0; // reducndency line for the Avoilonia Thread to ignore the window when not needed
            if (Opacity == 0 &&
                IsKill == true &&
                ShowHideTransition != null &&
                ShowHideTransition.FunctionRunning == false &&
                Master != null)
            { // this if statment just checks if the user want to kill this object and removes it
              // from its parent

                Master.Children.Remove(this);

            }
        }

        public void OnHoverSetBackground(double Value)
        {


            if (Background is SolidColorBrush solidBrush)
            {
                var originalColor = solidBrush.Color;
                byte newAlpha = (byte)(255 * Value);
                var fadedColor = Color.FromArgb(newAlpha, originalColor.R, originalColor.G, originalColor.B);
                Background = new SolidColorBrush(fadedColor);
            }

            //if (ShowHideTransition != null && ShowHideTransition.FunctionRunning == false) {
            //    Opacity = Value;
            //    return;
            //}
            //if (HoverTranslation != null) HoverTranslation.Reset();
        }


        private Vector InitialPos;
        private Vector FinalPos;
        public void SetPostionTranslate(double Xpos, double Ypos)
        {

            if (PostionTranslation != null) PostionTranslation.FunctionRunning = false;

            PostionTranslation = new Animations.Transations.Uniform
            {
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration,
                Trigger = SetPostionTrigger
            };

            if (double.IsNaN(Canvas.GetRight(this)))
            { // this will only occures on start up
                Canvas.SetRight(this, Xpos);
                Canvas.SetTop(this, Ypos);
                return;
            }

            InitialPos = new Vector(Canvas.GetRight(this), Canvas.GetTop(this));
            FinalPos = new Vector(Xpos, Ypos);

            PostionTranslation.TranslateForward();
        }

        private void SetPostionTrigger(double value)
        {
            Canvas.SetRight(this, InitialPos.X + (FinalPos.X - InitialPos.X) * value);
            Canvas.SetTop(this, InitialPos.Y + (FinalPos.Y - InitialPos.Y) * value);
        }
    }
}
