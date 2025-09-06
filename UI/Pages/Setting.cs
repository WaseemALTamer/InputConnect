using InputConnect.UI.Containers;
using System.Collections.Generic;
using Avalonia.Interactivity;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia;
using InputConnect.SettingStruct;
using System.Reflection;
using System;
using InputConnect.UI.InWindowPopup;







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

        private Conformation? ConformationPopup;


        // this approch will be changed and i will proabably make it manual
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
                Background = InputConnect.Setting.Themes.Button,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                FontSize = InputConnect.Setting.Config.FontSize,
                CornerRadius = new CornerRadius(InputConnect.Setting.Config.CornerRadius)
            };
            MainCanvas.Children.Add(ApplyButton);
            yPos += 10; // add a padding 
            Canvas.SetTop(ApplyButton, yPos);
            //yPos += ApplyButton.Height; // we dont add padding since next item is going to be on the same 
            ApplyButton.Click += OnClickApplyButton;


            DefaultButton = new Button{
                Content = "Default",
                Width = 150,
                Height = 50,
                Background = InputConnect.Setting.Themes.Button,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                FontSize = InputConnect.Setting.Config.FontSize,
                CornerRadius = new CornerRadius(InputConnect.Setting.Config.CornerRadius)
            };
            MainCanvas.Children.Add(DefaultButton);
            //yPos += 10; // padding already added from before
            Canvas.SetTop(DefaultButton, yPos);
            yPos += DefaultButton.Height;
            DefaultButton.Click += OnClickDefaultButton;

            if (PublicWidgets.Master != null) {
                ConformationPopup = new Conformation(PublicWidgets.Master);
                ConformationPopup.ConfirmTrigger += OnConformApplySetting;
            }
                

            AddSettingProperties();


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

            ConfigStruct configStruct = InputConnect.Setting.Config;
            PropertyInfo[] properties = typeof(ConfigStruct).GetProperties();

            for (int i = 0; i < properties.Length; i++){
                PropertyInfo prop = properties[i];
                var name = prop.Name;
                var rawValue = prop.GetValue(configStruct);
                var value = rawValue?.ToString() ?? string.Empty; // always a string now

                Properties[i].SetValue(value);

            }



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



        public void AddSettingProperties() {

            if (MainCanvas == null) return;

            double pading = 10;
            double Ypos = 75 + pading;


            foreach (PropertyInfo prop in typeof(ConfigStruct).GetProperties()){
                var name = prop.Name;
                var value = prop.GetValue(InputConnect.Setting.Config);

                var UIConfig = new ConfigProperty(MainCanvas, name);

                UIConfig.SetValue(value?.ToString());
                
                MainCanvas.Children.Add(UIConfig);
                Properties.Add(UIConfig);
                
                Canvas.SetTop(UIConfig, Ypos);
                Canvas.SetLeft(UIConfig, (MainCanvas.Width - UIConfig.Width)/ 2);
                Ypos += UIConfig.Height + pading;
            }

            MainCanvas.Height = Ypos + pading;

        }

        private ConfigStruct? configStructToSave;
        private void OnConformApplySetting() {
            if (configStructToSave == null) return;
            InputConnect.Setting.Config = configStructToSave;
            AppData.SaveConfig();
        }



        public void ApplySetting(ConfigStruct Config){
            configStructToSave = Config;
            if (ConformationPopup == null) return;

            ConformationPopup.Note = "Are you sure you want to apply and overwrite the config?";
            ConformationPopup.Update();
            ConformationPopup.Show();
        }

        private void OnClickApplyButton(object? sender = null, RoutedEventArgs? e = null){

            ConfigStruct configStruct = new ConfigStruct();
            PropertyInfo[] properties = typeof(ConfigStruct).GetProperties();

            for (int i = 0; i < properties.Length; i++){
                PropertyInfo prop = properties[i];
                var name = prop.Name;
                var value = prop.GetValue(configStruct);


                if (prop.PropertyType == typeof(string)){
                    prop.SetValue(configStruct, Properties[i].GetValue());
                }
                else if (prop.PropertyType == typeof(int)){
                    int intValue = Convert.ToInt32(Properties[i].GetValue());
                    prop.SetValue(configStruct, intValue);
                }
            }

            ApplySetting(configStruct);

        }

        private void OnClickDefaultButton(object? sender = null, RoutedEventArgs? e = null){
            ConfigStruct configStruct = new ConfigStruct();
            PropertyInfo[] properties = typeof(ConfigStruct).GetProperties();

            for (int i = 0; i < properties.Length; i++){
                PropertyInfo prop = properties[i];
                var name = prop.Name;
                var rawValue = prop.GetValue(configStruct);
                var value = rawValue?.ToString() ?? string.Empty; // always a string now

                Properties[i].SetValue(value);

            }
            //ApplySetting(configStruct); apply the defult setting by clicking the Aplly button
        }




    }
}
