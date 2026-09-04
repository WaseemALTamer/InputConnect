using Avalonia.Controls.ApplicationLifetimes;
using InputConnect.UI.OutWindowPopup; 
using Avalonia.Markup.Xaml;
using InputConnect.Network;
using Avalonia.Controls;
using Avalonia;




namespace InputConnect
{
    public partial class App : Application
    {

        // <POPUPS OUTWINDOW>
        private static InvisiableOverlaySDL? _UIInvisiableOverlayOutPop;
        public static InvisiableOverlaySDL? UIInvisiableOverlayOutPop{
            get { return _UIInvisiableOverlayOutPop; }
            set { _UIInvisiableOverlayOutPop = value; }
        }


        public override void Initialize(){
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted(){

            // other than the main window we create a toplevel window this
            // does nothing other than passes  its  own functions and vars
            // so we can use on the go incase you  are using headless mode
            // the whole UI doesnt need to exist at all
            var toplevel = new Window{
                Width = 1,
                Height = 1,
                Opacity = 0,
                ShowInTaskbar = false,
                SystemDecorations = SystemDecorations.None
            };

            SharedData.Device.Screens = toplevel.Screens.All;
            SharedData.Device.TopLevel = toplevel;



            AppData.LoadConfig();
            //AppData.LoadTheme(); // this doesnt work for now
            AppData.LoadConnections(); // load previouse connections if we can




            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop){
                desktop.MainWindow = new MainWindow();
            }

            
            base.OnFrameworkInitializationCompleted();


            // those lines are used to tell the system we need to prepare the static classes


            var _ = Assets.AssetsLoaded;
            var __ = Tray.ApplicationTray;
            var ___ = ConnectionUDP.Client;
            var ____ = Controllers.Hook.StartHook();



            // this creates the absorber popup
            UIInvisiableOverlayOutPop = new InvisiableOverlaySDL(); // we start this after the hooks since this will attach to the hooks



        }
    }
}

// dont try to delete this folder this sets up the project you need the AvaloniaXamlLoader.Load(this); other wise the
// project aint ganna work like you want