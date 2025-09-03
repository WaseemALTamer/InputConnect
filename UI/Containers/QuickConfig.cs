using Avalonia;
using Avalonia.Controls;
using InputConnect.Commands;
using InputConnect.Network;
using System;
using System.Diagnostics.Metrics;


namespace InputConnect.UI.Containers
{
    public class QuickConfig : Border
    {
        // this class is going to hold some of the setting
        // that you may want  to change  on the run as you
        // setup your connection(s) this will be universal
        // to  all connections  and is  not something that
        // you setup for each connection


        private Canvas? Master;

        private Canvas? _MainCanvas;
        public Canvas? MainCanvas{
            get { return _MainCanvas; }
            set { _MainCanvas = value; }
        }

        private TextBlock? QucikConfigHeader;

        private ComboBox? AudioOutputMenu;
        


        public QuickConfig(Canvas master) {

            Master = master;

            Opacity = 1;
            ClipToBounds = true;
            Background = Setting.Themes.Holder;
            CornerRadius = new CornerRadius(Setting.Config.CornerRadius);

            Width = 400; Height = 200;


            MainCanvas = new Canvas{
                ClipToBounds = true,
            };
            Child = MainCanvas;


            if (Master != null){
                Master.PropertyChanged += OnSizeChage;
            }

            QucikConfigHeader = new TextBlock{
                Text = "Quick Config",
                Width = 155, Height = 50,
                FontSize = 25,
                Foreground = Setting.Themes.Text,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            };

            MainCanvas.Children.Add(QucikConfigHeader);


            AudioOutputMenu = new ComboBox{
                Width = 250,
                Height = 40,
                PlaceholderText = "Sound Output",
                Background = Setting.Themes.DropDown,
                PlaceholderForeground = Setting.Themes.WaterMark,
                CornerRadius = new CornerRadius(Setting.Config.CornerRadius),
                FontSize = Setting.Config.FontSize,
            };
            MainCanvas.Children.Add(AudioOutputMenu);


            if (Setting.Config.OutputAudioDevice != ""){
                AudioOutputMenu.Clear(); // clear for redundency
                AudioOutputMenu.Items.Add(new ComboBoxItem{
                    Content = Setting.Config.OutputAudioDevice,
                    FontSize = Setting.Config.FontSize
                });
                AudioOutputMenu.SelectedIndex = 0;
            }

            AudioOutputMenu.SelectionChanged += OnSelectionChanged;
            AudioOutputMenu.DropDownOpened += OnPointerReleased;
        }

        public void OnSizeChage(object? sender = null, object? e = null){

            if (Master != null) {

                Height = Master.Height - 414;

                
                if (AudioOutputMenu != null) {
                    Canvas.SetLeft(AudioOutputMenu, (Width - AudioOutputMenu.Width) / 2);
                    Canvas.SetTop(AudioOutputMenu, 80);
                }

                if (QucikConfigHeader != null){
                    Canvas.SetLeft(QucikConfigHeader, (Width - QucikConfigHeader.Width) / 2);
                    Canvas.SetTop(QucikConfigHeader, 10);
                }

            }


        
        }


        private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e){
            if (AudioOutputMenu == null) return;

            var item = AudioOutputMenu.SelectedItem as ComboBoxItem;
            if (item != null){
                string? deviceName = (string?)item.Content;
                if (deviceName == null || deviceName == "") {
                    Console.WriteLine("Nothing Selected");
                    return;
                }

                // we save the last Selected Device
                Setting.Config.OutputAudioDevice = deviceName;
                AppData.SaveConfig();

                Controllers.Audio.AudioOut.StartAudioOut(deviceName);
            }
        }

        private void OnPointerReleased(object? sender, object? e) {
            if (AudioOutputMenu == null) return;

            Controllers.Audio.AudioOut.DetectAudioOutputDevices(); // run the detection

            if (SharedData.Device.AudioOutputDevices == null) return;

            AudioOutputMenu.Items.Clear();

            foreach (var deviceName in SharedData.Device.AudioOutputDevices) {

                AudioOutputMenu.Items.Add(new ComboBoxItem { 
                                                    Content = deviceName, 
                                                    FontSize = Setting.Config.FontSize
                                                });

            }



        }




    }
}
