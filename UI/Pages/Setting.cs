using InputConnect.UI.Containers;
using System.Collections.Generic;
using InputConnect.Structures;
using Avalonia.Interactivity;
using System.Threading.Tasks;
using InputConnect.Setting;
using System.Reflection;
using Avalonia.Controls;
using Avalonia;
using System;







namespace InputConnect.UI.Pages
{
    // this file will only provide the structure of the canvase and how it looks like
    // it will also provide the tools to add and remove  the  children  of it none of
    // its logic will be ran through it

    // this file ended also running the logic as well

    public class Setting : Base
    {

        private Button? ApplyButton;
        private Button? DefaultButton;


        private List<ConfigProperty> _Properties = new List<ConfigProperty>();
        public List<ConfigProperty> Properties{
            get { return _Properties; }
            set { _Properties = value; }
        }

        public Setting(Canvas? master) : base(master)
        {

            if(MainCanvas == null) return;
            IsVisible = true; // turn it on to force the base to load it up frame work issue



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

            // CONFIG.PROPERTY <PopupsTimeout>
            AddConfigProperty("Popups Timeout (ms)", ref yPos);

            // CONFIG.PROPERTY <TransitionReferenceDistance>
            AddConfigProperty("Transition Reference Distance (ms)", ref yPos);

            // CONFIG.PROPERTY <FontSize>
            AddConfigProperty("TimeOutDuration (ms)", ref yPos);



            MainCanvas.Height = yPos + 20; // set the final height



            OnResize(); // trigger the function to set the sizes
            MainCanvas.SizeChanged += OnResize;

            OnShow += LocalOnShow;
            OnHide += LocalOnHide;


            IsVisible = false; // turn it back off proformace wise it doesnt matter any more

        }


        public void OnResize(object? sender = null, SizeChangedEventArgs? e = null){

            if (MainCanvas != null){
                if (ApplyButton != null){
                    Canvas.SetLeft(ApplyButton, ((MainCanvas.Width - ApplyButton.Width) / 2) - (ApplyButton.Width/2 + 10));
    
                }
                if (DefaultButton != null){
                    Canvas.SetLeft(DefaultButton, ((MainCanvas.Width - DefaultButton.Width) / 2) + (DefaultButton.Width / 2 + 10));
                }
            }
        }

        private async void LocalOnShow() {
            AppendSettingProperties();
            foreach (var Propertie in Properties){
                await Task.Delay(50); // this ensures everything loads up smoothly the first time round
                if (IsDisplayed == false) return;
                Propertie.Show();
            }
        }

        private void LocalOnHide(){
            foreach (var Propertie in Properties){
                Propertie.Hide();
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

            foreach (var property in configType.GetProperties(BindingFlags.Public | BindingFlags.Instance)){
                if (_index >= Properties.Count)
                    break;

                var prop = Properties[_index];
                var value = prop.GetValue();

                try{
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

            AppData.ApplyConfig(NewConfig);
            AppData.SaveConfig(NewConfig);
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
