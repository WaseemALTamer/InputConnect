using Avalonia.Controls.Primitives;
using InputConnect.UI.Containers;
using System.Collections.Generic;
using InputConnect.UI.Animations;
using InputConnect.Structures;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using InputConnect.Setting;
using System.Reflection;
using Avalonia.Controls;
using Avalonia;
using System;






namespace InputConnect.UI.Holders
{
    // this file will only provide the structure of the canvase and how it looks like
    // it will also provide the tools to add and remove  the  children  of it none of
    // its logic will be ran through it

    // this file ended also running the logic as well

    public class DeviceSetting : Border, IDisplayable
    {

        private Canvas? _Master;
        public Canvas? Master{
            get { return _Master; }
            set { _Master = value; }
        }


        private bool _IsDisplayed = false;
        public bool IsDisplayed{
            get { return _IsDisplayed; }
            set { _IsDisplayed = value; }
        }

        private Animations.Transations.Uniform? ShowHideTransition;

        private ScrollViewer? _ScrollViewer;
        private SmoothScrolling? _ScrollingAnimation;

        private Canvas? _MainCanvas;
        public Canvas? MainCanvas{
            get { return _MainCanvas; }
            set { _MainCanvas = value; }
        }

        private List<ConfigProperty> _Properties = new List<ConfigProperty>();
        public List<ConfigProperty> Properties
        {
            get { return _Properties; }
            set { _Properties = value; }
        }


        private Button? ApplyButton;
        private Button? DefaultButton;

        public DeviceSetting(Canvas? master)
        {
            Master = master;

            Opacity = 0;
            IsVisible = true; // this is only true so we trick Avoliana frame work to loading the content of this class
            ClipToBounds = true;
            IsHitTestVisible = true;
            CornerRadius = new CornerRadius(Config.CornerRadius);
            Background = Themes.Holders;




            ShowHideTransition = new Animations.Transations.Uniform{
                StartingValue = 0,
                EndingValue = 1,
                Duration = Config.TransitionDuration,
                Trigger = SetOpeicity,
            };

            _ScrollViewer = new ScrollViewer{
                IsHitTestVisible = true
            };


            _ScrollingAnimation = new Animations.SmoothScrolling{
                Trigger = SmoothVerticalScrollerTrigger
            };


            MainCanvas = new Canvas{
                IsHitTestVisible = true,
            };
            MainCanvas.ClipToBounds = true;

            _ScrollViewer.Content = MainCanvas;
            _ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            _ScrollViewer.AddHandler(PointerWheelChangedEvent, _ScrollingAnimation.OnPointerWheelChanged, RoutingStrategies.Tunnel); // this methode is used as the file will be already loaded and the function will already have
                                                                                                                                     // functions that end the event totally
            
            PointerWheelChanged += _ScrollingAnimation.OnPointerWheelChanged;
            MainCanvas.PointerWheelChanged += _ScrollingAnimation.OnPointerWheelChanged;




            //<summery> 
            //
            // this is just a simple abbsorber Window
            //
            //var Test = new Window{
            //    WindowState = WindowState.FullScreen,
            //    Background = new SolidColorBrush(Avalonia.Media.Color.FromUInt32(0x00ffffff)),
            //    ShowInTaskbar = false,
            //    Title = "Abbsorber",
            //};
            //Test.Show();

            if (Master != null){
                OnResize(); // trigger the function to set the sizes
                Master.SizeChanged += OnResize;
            }

            // now we can add the children one by one maually since all of them have something unique to them
            // you can generally factorise all all the strings and all the ints  but  it  would  become  more
            // complixe for no reason so just do it maually for easier debugging


            double yPos = 0; // this is used to keep track and set each postion it is caluclated as we go along


            ApplyButton = new Button{
                Content = "Apply",
                Width = 150,
                Height = 50,
                Background = Themes.Button,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                FontSize = Config.FontSize,
                CornerRadius = new CornerRadius(Config.CornerRadius)
            };
            MainCanvas.Children.Add(ApplyButton);
            yPos += 10; // add a padding 
            Canvas.SetTop(ApplyButton, yPos);
            //yPos += ApplyButton.Height; // we dont add padding since next item is going to be on the same 
            ApplyButton.Click += OnClickApplyButton;


            DefaultButton = new Button{
                Content = "Defult",
                Width = 150,
                Height = 50,
                Background = Themes.Button,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                FontSize = Config.FontSize,
                CornerRadius = new CornerRadius(Config.CornerRadius)
            };
            MainCanvas.Children.Add(DefaultButton);
            //yPos += 10; // padding already added from before
            Canvas.SetTop(DefaultButton, yPos);
            yPos += DefaultButton.Height;
            DefaultButton.Click += OnClickDefaultButton;


            // note that they have to be in order other wise this will cause many unwanted issues later on

            // CONFIG.PROPERTY <DeviceName>
            AddConfigProperty("Device Name",ref yPos);

            // CONFIG.PROPERTY <ApplicationName>
            AddConfigProperty("Application Name -> (Restart)", ref yPos);

            // CONFIG.PROPERTY <BroadCastAddress>
            AddConfigProperty("Broadcast Address", ref yPos);

            // CONFIG.PROPERTY <Port>
            AddConfigProperty("Port", ref yPos);

            // CONFIG.PROPERTY <Tick>
            AddConfigProperty("Tick Interval (ms) -> (Restart)", ref yPos);

            // CONFIG.PROPERTY <AdvertisementInterval>
            AddConfigProperty("Advertise Interval (ms)", ref yPos);

            // CONFIG.PROPERTY <AdvertiseTimeSpan>
            AddConfigProperty("Advertise Timeout (s)", ref yPos);

            // CONFIG.PROPERTY <TransitionDuration>
            AddConfigProperty("Transition Duration (ms) -> (Restart)", ref yPos);

            // CONFIG.PROPERTY <CornerRadius>
            AddConfigProperty("Corner Radius -> (Restart)", ref yPos);

            // CONFIG.PROPERTY <TransitionHover>
            AddConfigProperty("Hover Duration (ms) -> (Restart)", ref yPos);

            // CONFIG.PROPERTY <FontSize>
            AddConfigProperty("Font Size -> (Restart)", ref yPos);



            MainCanvas.Height = yPos + 20; // set the final height

            IsVisible = false; // turn it back off
            Child = _ScrollViewer;
        }


        public void OnResize(object? sender = null, SizeChangedEventArgs? e = null){
            if (Master != null){
                Width = Master.Width * 0.85;
                Height = Master.Height * 0.85;

                if (MainCanvas != null){
                    MainCanvas.Width = Width;
                    //MainCanvas.MinHeight = Height; // we dont set the height so we dont disturb the scrolling
                    if (ApplyButton != null){
                        Canvas.SetLeft(ApplyButton, ((MainCanvas.Width - ApplyButton.Width) / 2) - (ApplyButton.Width/2 + 10));
    
                    }
                    if (DefaultButton != null){
                        Canvas.SetLeft(DefaultButton, ((MainCanvas.Width - DefaultButton.Width) / 2) + (DefaultButton.Width / 2 + 10));
                    }
                }

                if (_ScrollViewer != null){
                    _ScrollViewer.Width = Width;
                    _ScrollViewer.Height = Height;
                }



                Canvas.SetRight(this, (Master.Width - Width) / 2);
                Canvas.SetBottom(this, (Master.Height - Height) / 2);
            }
        }


        public async void Show(){
            IsDisplayed = true;
            if (ShowHideTransition != null)
                ShowHideTransition.TranslateForward();

            AppendSettingProperties();
            foreach (var Propertie in Properties){
                await Task.Delay(50); // this ensures everything loads up smoothly the first time round
                Propertie.Show();
            }
        }

        public void Hide(){
            IsDisplayed = false;
            if (ShowHideTransition != null)
                ShowHideTransition.TranslateBackward();

            foreach (var Propertie in Properties){
                Propertie.Hide();
            }
        }

        public void SetOpeicity(double Value){
            Opacity = Value;
            IsVisible = Opacity != 0; // reducndency line for the Avoilonia Thread to ignore the window when not needed
        }

        private void SmoothVerticalScrollerTrigger(double Value){
            if (_ScrollViewer != null){
                _ScrollViewer.Offset = new Vector(_ScrollViewer.Offset.X, _ScrollViewer.Offset.Y + Value);
            }

        }

        private ConfigProperty? AddConfigProperty(string label, ref double yPos){
            if (MainCanvas == null) return null;
            var prop = new ConfigProperty(MainCanvas, label);
            yPos += 10;
            prop.SetPostionTranslate((MainCanvas.Width - prop.Width) / 2, yPos);
            yPos += prop.Height;
            Properties.Add(prop);
            return prop;
        }

        public void AppendSettingProperties(){
            var staticConfigType = typeof(Config);

            int _index = 0;
            foreach (var staticField in staticConfigType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (_index >= Properties.Count)
                    break;

                var value = staticField.GetValue(null)?.ToString();
                if (value != null) Properties[_index].SetValue(value);

                _index++;
            }
        }

        public void ApplySetting(){
            var NewConfig = new ConfigStruct();
            var configType = typeof(ConfigStruct);
            int _index = 0;

            foreach (var property in configType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (_index >= Properties.Count)
                    break;

                var prop = Properties[_index];
                var value = prop.GetValue();

                try
                {
                    // this try to make it int if it is then everything is good so far
                    try{
                        property.SetValue(NewConfig, int.Parse(value));
                    }
                    catch{ // that means it is string if we couldnt
                        property.SetValue(NewConfig, value);
                    }

                    _index++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to set value for {property.Name}: {ex.Message}");
                }
            }

            Appdata.ApplyConfig(NewConfig);
            Appdata.SaveConfig(NewConfig);
        }

        private void OnClickApplyButton(object? sender = null, RoutedEventArgs? e = null){
            ApplySetting();
        }

        private void OnClickDefaultButton(object? sender = null, RoutedEventArgs? e = null){
            var NewConfig = new ConfigStruct(); // creating the configstruct will already have the defult values inside of it 

       

            var configType = typeof(ConfigStruct);

            int _index = 0;

            foreach (var property in configType.GetProperties(BindingFlags.Public | BindingFlags.Instance)) {

                var value = property.GetValue(NewConfig)?.ToString();

                if (value != null) {
                    Properties[_index].SetValue(value);
                }
                
                
                _index++;
            }


            //Clicking apply is still required after appending the defult values
        }
    }
}
