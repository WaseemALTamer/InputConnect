using Avalonia.Controls;
using System;



namespace InputConnect.Setting
{
    static class Tray{

        public static bool TrayEnabled = false;

        public static TrayIcon? ApplicationTray;
        public static NativeMenu TrayMenu = new NativeMenu();

        // those tray items are are must
        public static NativeMenuItem TrayMenuItem_Exit = new NativeMenuItem("Exit");
        public static NativeMenuItem TrayMenuItem_Show = new NativeMenuItem("Show");

        static Tray() {
            ApplicationTray = new TrayIcon
            {
                ToolTipText = "InputConnect",
                IsVisible = false,
                Menu = TrayMenu
            };
            TrayMenu.Items.Add(TrayMenuItem_Show);
            TrayMenu.Items.Add(TrayMenuItem_Exit);


            Assets.AddAwaitedActions(() => {
                ApplicationTray.IsVisible = true;
                ApplicationTray.Icon = Assets.Icone;
                ApplicationTray.IsVisible = false;
            }); // we add it with the asset loader so its thread safe
        }


        static public void AddMenuItem(string Header, EventHandler? Function) { 
            NativeMenuItem Item = new NativeMenuItem(Header);
            Item.Click += Function;
            TrayMenu.Items.Add(Item);
        }

        static public void ClearMenu() { 
            TrayMenu.Items.Clear();

            TrayMenu.Items.Add(TrayMenuItem_Show);
            TrayMenu.Items.Add(TrayMenuItem_Exit);
        }
    }
}
