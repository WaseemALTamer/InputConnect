﻿using Avalonia.Interactivity;
using System.Threading.Tasks;
using InputConnect.Setting;
using Avalonia.Threading;
using Avalonia.Controls;
using InputConnect.UI;
using System;



namespace InputConnect
{
    class MainWindow : Window{
        
        public Canvas? MainCanvas;

        public MainWindow(){
            Icon = Assets.Icone;
            Title = Config.ApplicationName;


            MinWidth = 912;
            MinHeight = 513;

            Loaded += OnLoaded;
            Closing += OnWindowClosing;

            SharedData.Device.Screens = Screens.All; // add the Screens data for the public varables for other
                                                     // parts of the code to use it
        }



        async public void OnLoaded(object? sender, RoutedEventArgs? e){


            if (Tray.ApplicationTray != null){ // redundancy if statment
                Tray.OnClickTrigger += OnTrayClick;
                Tray.TrayMenuItem_Exit.Click += OnClickExit;
                Tray.TrayMenuItem_Show.Click += OnClickShow;
            }

            // we make sure that we got the Width and Height as they are nessasary for the next parts of the  code
            // i cant relie on the Avalonia.Net to give me the right values as soon as possible because it runs it
            // asynchronously and it might run my code before which would create problems for me as i am  reliying 
            // on there code to run first
            while (double.IsNaN(Width) || double.IsNaN(Height)) await Task.Delay(1);


            MainCanvas = new Canvas{ // now we create the canvase after we load up for preformece
                Width = Width,
                Height = Height,
                Background = Themes.Backgruond,
                Focusable = true,
            };


            // this is the part of the code that handels all the other  Canvasue on screen
            // note that this file is for desktop only but this part can be shared acrross
            // probably based on the Avalonia.Net support for other devices in terms of UI

            PublicWidgets.Initialize(MainCanvas);


            Resized += OnResize;
            Content = MainCanvas;
        }

        public void OnResize(object? sender, WindowResizedEventArgs? e) {
            if (MainCanvas != null) { 
                MainCanvas.Width = Width;
                MainCanvas.Height = Height;
            }
        }

        private void OnWindowClosing(object? sender, System.ComponentModel.CancelEventArgs e){
            // Prevent window from closing and just hide it
            if (Tray.TrayEnabled) {
                e.Cancel = true;
                WindowState = WindowState.Minimized;

                // this helps complete the animation for windows so it smoothly get minimised
                Dispatcher.UIThread.Post(async () => {
                    await Task.Delay(300);
                    ShowInTaskbar = false;
                });

                Tray.InitTray(); // intialise it before we use it
                if (Tray.ApplicationTray != null)
                    Tray.ApplicationTray.IsVisible = true;
            }
        }

        private void OnTrayClick(object? sender, EventArgs e) {
            if (Tray.ApplicationTray != null){
                Tray.ApplicationTray.IsVisible = false;
            }

            ShowInTaskbar = true;
            // this helps complete the animation for windows so it smoothly appears on screen
            Dispatcher.UIThread.Post(async () =>{
                await Task.Delay(100);
                WindowState = WindowState.Normal;
            });
        }



        private void OnClickShow(object? sender, object? e) {

            if (Tray.ApplicationTray != null) {
                Tray.ApplicationTray.IsVisible = false;
            }


            ShowInTaskbar = true;
            // this helps complete the animation for windows so it smoothly appears on screen
            Dispatcher.UIThread.Post(async () => {
                await Task.Delay(100);
                WindowState = WindowState.Normal;
            });
        }



        private void OnClickExit(object? sender, object? e) {
            Environment.Exit(0);
        }
    }
}