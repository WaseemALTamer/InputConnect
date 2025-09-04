using Avalonia.Controls;





namespace InputConnect.UI.Containers
{
    public class NoConnectorMessage: Border
    {


        private Canvas? Master;

        private Canvas? _MainCanvas;


        private Border? ConnectorImageBoarder;
        private Image? ConnectorImage;

        private TextBlock? ConnectorMessage;


        public Canvas? MainCanvas
        {
            get { return _MainCanvas; }
            set { _MainCanvas = value; }
        }


        private Animations.Transations.Uniform? ShowHideTransition;


        public NoConnectorMessage(Canvas? master){
            Master = master;

            Width = 500;
            Height = 300;

            IsVisible = false;
            Opacity = 0;

            MainCanvas = new Canvas{
                Width = Width,
                Height = Height
            };

            ConnectorImageBoarder = new Border
            {
                Width = MainCanvas.Width / 2,
                Height = MainCanvas.Height / 2,
            };
            MainCanvas.Children.Add(ConnectorImageBoarder);

            ConnectorImage = new Image
            {
                Stretch = Avalonia.Media.Stretch.Uniform,
            };

            Assets.AddAwaitedAction(() => {
                ConnectorImage.Source = Assets.NoConnectorBitmap;
            });
            ConnectorImageBoarder.Child = ConnectorImage;

            ConnectorMessage = new TextBlock{
                Text = "No connections available",
                FontSize = Setting.Config.FontSize,
                Width = 250,
                Height = 50
            };
            MainCanvas.Children.Add(ConnectorMessage);


            ShowHideTransition = new Animations.Transations.Uniform
            {
                StartingValue = 0,
                EndingValue = 0.7,
                Duration = Setting.Config.TransitionDuration,
                Trigger = SetOpacity,
            };





            SizeChanged += OnSizeChanged;



            Child = MainCanvas;
        }

        public void OnSizeChanged(object? sender, object? e)
        {

            if (MainCanvas == null) return;


            if (ConnectorImageBoarder != null)
            {
                Canvas.SetLeft(ConnectorImageBoarder, (MainCanvas.Width - ConnectorImageBoarder.Width) / 2);
                Canvas.SetTop(ConnectorImageBoarder, 0);
            }

            if (ConnectorMessage != null)
            {
                Canvas.SetLeft(ConnectorMessage, (MainCanvas.Width - ConnectorMessage.Width) / 2);
                Canvas.SetTop(ConnectorMessage, -ConnectorMessage.Height);
            }


        }

        bool hidden = true; // this is used  because of  the  TranslateForward TraslateBackward bug
                            // which needs to be fixed where if the translation  is not running and
                            // you try to translate it will shift it back to have the oposite value
                            // and then translate it to the target value
        public void Show(){

            if (hidden == false) return;



            ShowHideTransition?.TranslateForward();
            hidden = false;
        }

        public void Hide(){

            if (hidden == true) return;
            ShowHideTransition?.TranslateBackward();
            hidden = true;
        }


        public void SetOpacity(double value)
        {
            Opacity = value;
            IsVisible = Opacity != 0; // reducndency line for the Avoilonia Thread to ignore the window when not needed
        }



    }
}
