using InputConnect.UI.Containers;
using System.Collections.Generic;
using Avalonia.Interactivity;
using System.Threading.Tasks;
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
                Content = "Defult",
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






        public void ApplySetting(){

        }

        private void OnClickApplyButton(object? sender = null, RoutedEventArgs? e = null){
            ApplySetting();
        }

        private void OnClickDefaultButton(object? sender = null, RoutedEventArgs? e = null){

        }




    }
}
