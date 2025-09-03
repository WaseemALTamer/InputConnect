using Avalonia.Controls;
using System;



namespace InputConnect
{
    static class Tray{

        public static bool TrayEnabled = true;

        public static TrayIcon? ApplicationTray;
        public static NativeMenu TrayMenu = new NativeMenu();

        // those tray items are are must
        public static NativeMenuItem TrayMenuItem_Exit = new NativeMenuItem("Exit");
        public static NativeMenuItem TrayMenuItem_Show = new NativeMenuItem("Show");

        public static Action<object?, EventArgs>? OnClickTrigger;

        static Tray() {
            InitTray(); // intialise the tray even through it will
                        // be  reinsitalsed when it  would be used

            TrayMenu.Items.Add(TrayMenuItem_Show);
            TrayMenu.Items.Add(TrayMenuItem_Exit);
        }

        public static void InitTray() {

            // this function is created spcicifically to fix an issue with linux 
            // Initlising the tray everytime you want to use it is  as it always
            // works the first time but crashes the  second time  this will keep
            // it on the first cycle this  function need to  be changed for more
            // efficnecny when the Avalonia people decide to fix this issue


            // consider  fixiing the avaloina  library ut self and making a push
            // request later  on for  this issue  to make  this application more
            // efficnent for linux systems




            ApplicationTray = new TrayIcon{
                ToolTipText = "InputConnect",
                IsVisible = false,
                Menu = TrayMenu,
            };
            
            

            Assets.AddAwaitedAction(() => {
                ApplicationTray.IsVisible = true;
                ApplicationTray.Icon = Assets.Icone;
                ApplicationTray.IsVisible = false;
            }); // we add it with the asset loader so its thread safe

            ApplicationTray.Clicked += OnClicked;
        }


        public static void AddMenuItem(string Header, EventHandler? Function) { 
            NativeMenuItem Item = new NativeMenuItem(Header);
            Item.Click += Function;
            TrayMenu.Items.Add(Item);
        }

        public static void ClearMenu() { 
            TrayMenu.Items.Clear();

            TrayMenu.Items.Add(TrayMenuItem_Show);
            TrayMenu.Items.Add(TrayMenuItem_Exit);
        }


        public static void OnClicked(object? sender, EventArgs e) {

            if (OnClickTrigger != null) {
                OnClickTrigger.Invoke(sender, e);
            }
        
        }
    }
}
