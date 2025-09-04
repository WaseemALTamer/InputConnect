using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Avalonia.Controls;
using System;




namespace InputConnect
{


    // this class is made so we can load all the assets using it and access the assets  through it
    // by giving it the function that you want to use the asset if the function requires a certain
    // asset and it does not exist it will wait until all of the assets loaded in then it runs all
    // all the functions that you gave it

    public static class Assets
    {

        public static Action? AwaitedActions;

        // we load all the assents as a public
        public static WindowIcon? Icone;
        public static Bitmap? BackButtonBitmap;
        public static Bitmap? CloseButtonBitmap;
        public static Bitmap? SettingButtonBitmap;
        public static Bitmap? WifiBitmap;
        public static Bitmap? SearchBitmap;
        public static Bitmap? ChainsBitmap;
        public static Bitmap? MouseBitmap;
        public static Bitmap? KeyboardBitmap;
        public static Bitmap? AudioBitmap;
        public static Bitmap? LockBitmap;
        public static Bitmap? MiniLockBitmap;
        public static Bitmap? WarningBitmap;
        public static Bitmap? TrashBinBitmap;
        public static Bitmap? ConnectorBitmap;
        public static Bitmap? NoConnectorBitmap;



        public static bool AssetsLoaded = false;

        static Assets() {
            Task.Run(() => LoadAssets()); // we need to be carfull though as we may want to only create certin
                                          // containers when we have its assets ready
        }


        public static void AddAwaitedAction(Action action) {
            if (AssetsLoaded){
                action();
            }
            else {
                AwaitedActions += action;
            }
        }

        async public static void LoadAssets() {
            AssetsLoaded = false;
            // we load all the assets here


            Icone = new WindowIcon("Assets/Icone/Icone.ico");
            BackButtonBitmap = new Bitmap("Assets/Images/BackButton.png");
            CloseButtonBitmap = new Bitmap("Assets/Images/CloseButton.png");
            SettingButtonBitmap = new Bitmap("Assets/Images/SettingButton.png");
            WifiBitmap = new Bitmap("Assets/Images/Wifi.png");
            SearchBitmap = new Bitmap("Assets/Images/Search.png");
            ChainsBitmap = new Bitmap("Assets/Images/Chains.png");
            MouseBitmap = new Bitmap("Assets/Images/Mouse.png");
            KeyboardBitmap = new Bitmap("Assets/Images/Keyboard.png");
            AudioBitmap = new Bitmap("Assets/Images/Audio.png");
            LockBitmap = new Bitmap("Assets/Images/Lock.png");
            MiniLockBitmap = new Bitmap("Assets/Images/MiniLock.png");
            WarningBitmap = new Bitmap("Assets/Images/Warning.png");
            TrashBinBitmap = new Bitmap("Assets/Images/TrashBin.png");
            ConnectorBitmap = new Bitmap("Assets/Images/Connector.png");
            NoConnectorBitmap = new Bitmap("Assets/Images/NoConnector.png");


            // we finnish loading the assests here
            AssetsLoaded = true;

            if (AwaitedActions != null){
                foreach (var action in AwaitedActions.GetInvocationList()){
                    if (action is Action validAction){
                        Dispatcher.UIThread.Post(() => validAction());
                        await Task.Delay(5); // give some time for the UI thread to process
                    }
                }
                AwaitedActions = null;
            }
        }
    }
}
