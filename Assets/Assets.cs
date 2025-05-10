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
        public static Bitmap? SettingButtonBitmap;


        public static bool AssetsLoaded = false;

        static Assets() {
            Task.Run(() => LoadAssets()); // we need to be carfull though as we may want to only create certin
                                          // containers when we have its assets ready
        }


        public static void AddAwaitedActions(Action action) {
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
            SettingButtonBitmap = new Bitmap("Assets/Images/SettingButton.png");

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
