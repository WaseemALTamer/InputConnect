using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using InputConnect.Network;
using InputConnect.Setting;
using Avalonia;


namespace InputConnect
{
    public partial class App : Application
    {
        public override void Initialize(){
            AvaloniaXamlLoader.Load(this);


            // those two lines are used to tell the system we need to prepare the static classes

            Appdata.ApplyConfig(Appdata.Config);
            var __ = Assets.AssetsLoaded; 
            var ___ = Tray.ApplicationTray;
            var ____ = ConnectionUDP.Client;
        }

        public override void OnFrameworkInitializationCompleted(){
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop){
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}

// dont try to delete this folder this sets up the project you need the AvaloniaXamlLoader.Load(this); other wise the
// project aint ganna work like you want