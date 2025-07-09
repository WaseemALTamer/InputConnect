using InputConnect.UI.InWindowPopup;
using InputConnect.UI.Containers;
using System.Collections.Generic;
using System.Threading.Tasks;
using InputConnect.UI.Pages;
using InputConnect.Setting;
using Avalonia.Controls;
using Avalonia;
using InputConnect.UI.OutWindowPopup;





namespace InputConnect.UI
{
    public static class PublicWidgets{

        // This will hold all the Big  Widgets togather mostly Canvases
        // This file will also contain data that can be used all around
        // this also keeps track of the history of what was on screen


        // one rule that must be kept for structure  perposes  and  must
        // must not be broken is UI  classes  can  connect  back  to the
        // PublicWidgets class but the  backend  like  ConnectionMangaer
        // should not connect back to here rather the UI  should  map  a
        // function to it instead

        // We use a singlton methode to create the Objects instance this
        // allows us to access it using its public pointer



        // <ASSETS>
        private static BackButton? _BackButton;
        public static BackButton? BackButton{
            get { return _BackButton; }
            set { _BackButton = value; }
        }

        private static SettingButton? _SettingButton;
        public static SettingButton? SettingButton{
            get { return _SettingButton; }
            set { _SettingButton = value; }
        }

        private static PageSwitchSlider? _PageSwitchSlider;
        public static PageSwitchSlider? PageSwitchSlider{
            get { return _PageSwitchSlider; }
            set { _PageSwitchSlider = value; }
        }



        // <PAGES>
        private static Pages.Advertisements? _UIAdvertisements;
        public static Advertisements? UIAdvertisements{
            get { return _UIAdvertisements; }
            set { _UIAdvertisements = value;}
        }


        private static Pages.Connections? _UIConnections;
        public static Pages.Connections? UIConnections{
            get { return _UIConnections; }
            set { _UIConnections = value; }
        }


        private static Pages.Device? _UIDevice;
        public static Pages.Device? UIDevice{
            get { return _UIDevice; }
            set { _UIDevice = value; }
        }


        private static Pages.Setting? _UISetting;
        public static Pages.Setting? UIDeviceSetting{
            get { return _UISetting; }
            set { _UISetting = value; }
        }






        // <POPUPS INWINDOW>
        private static ConnectionReplay? _UIConnectionReplayInPop;
        public static ConnectionReplay? UIConnectionReplayInPop{
            get { return _UIConnectionReplayInPop; }
            set { _UIConnectionReplayInPop = value; }
        }


        // <POPUPS OUTWINDOW>
        private static InvisiableOverlay? _UIInvisiableOverlayOutPop;
        public static InvisiableOverlay? UIInvisiableOverlayOutPop{
            get { return _UIInvisiableOverlayOutPop; }
            set { _UIInvisiableOverlayOutPop = value; }
        }







        private static List<Pages.Base> Pages = new List<Pages.Base>(); // a list of all the holders this is legacy code not used for recent code
        private static List<Pages.Base> PagesHistory = new List<Pages.Base>(); // this will be used with the back button to go back from where we came from

        private static Pages.Base? DisplayedPage;

        public static void Initialize(AvaloniaObject master) {
            var Master = master as Canvas;
            if (Master == null) return; // redundency if statment


            // We create all the instances of all our objects here
            // Objects will  connect them selfs  togather the back
            // button is something different


            // <ASSETS START>

            // this creates and attach the function to the back button
            BackButton = new BackButton(Master);
            Master.Children.Add(BackButton);
            BackButton.Trigger = BackButtonFunction;

            // this creates and attach the function to the back button
            SettingButton = new SettingButton(Master);
            Master.Children.Add(SettingButton);
            SettingButton.Trigger = SettingButtonFunction;

            // this creats and attachs the function to the slider button
            PageSwitchSlider = new PageSwitchSlider(Master);
            Master.Children.Add(PageSwitchSlider);
            PageSwitchSlider.Trigger = PageSwitchSliderFunction;



            // <ASSETS END>



            // <PAGES START>

            // this creates the Advertisements Page
            UIAdvertisements = new Pages.Advertisements(Master);
            Pages.Add(UIAdvertisements);

            // this creates the Device Page
            UIDevice = new Pages.Device(Master);
            Pages.Add(UIDevice);

            // this creates the DeviceSetting Page
            UIDeviceSetting = new Pages.Setting(Master);
            Pages.Add(UIDeviceSetting);
            
            
            // this creates the Connections Page
            UIConnections = new Pages.Connections(Master);
            Pages.Add(UIConnections);

            
            // <PAGES END>


            // <IN_POPUPS START>

            // this creates the popup for when people try to connect to you
            UIConnectionReplayInPop = new ConnectionReplay(Master);


            // <OUT_POPUPS END>



            // <OUT_POPUPS START>

            // this creates the absorber popup
            UIInvisiableOverlayOutPop = new InvisiableOverlay();

            // <IN_POPUPS END>

            //UIConnections
            //UIAdvertisements
            BackButton.Show();
            SettingButton.Show();
            TransitionForward(UIAdvertisements);
        }





        private static bool _TransitionForwardRunning = false; // for thread safty
        public static async void TransitionForward(Pages.Base Page) {
            if (_TransitionForwardRunning) return;
            _TransitionForwardRunning = true;
            if (DisplayedPage != null) {


                if (DisplayedPage != UIDevice) 
                    // exculde the UIDevice holder because we simply just cant
                    // go back to it because of how  we share the  data, it is
                    // possible but not recommended since it wont be universal
                    // methode on how we handel the shared data
                    PagesHistory.Add(DisplayedPage);
                
                DisplayedPage.Hide();
                DisplayedPage = Page; // redundency
                await Task.Delay(Config.TransitionDuration / 2); // wait for at bit to give ot a smoother feel
            }
            DisplayedPage = Page;
            Page.Show();
            _TransitionForwardRunning = false;
        }

        private static bool _TransitionBackRunning = false; // for thread safty
        public static async void TransitionBack()
        {
            if (_TransitionBackRunning) return;
            _TransitionBackRunning = true;
            if (PagesHistory.Count > 0 &&
                DisplayedPage != null)
            {
                var lastPage = PagesHistory[PagesHistory.Count - 1];
                PagesHistory.RemoveAt(PagesHistory.Count - 1);


                DisplayedPage.Hide();
                DisplayedPage = lastPage;
                await Task.Delay(Config.TransitionDuration / 2);
                DisplayedPage.Show();
            }
            _TransitionBackRunning = false;
        }


        public static void BackButtonFunction()
        {

            // this is for the setting
            if (UIDeviceSetting != null &&
                SettingButton != null &&
                UIDeviceSetting.IsDisplayed)
            {
                SettingButtonFunction(); // loop back to the other function if the button was pressed
                return;
            }


            TransitionBack(); // Transision backward

            // after transitioning back we check if the Page is on addvertisment or connection so we
            // can set the switch slider at the right state

            if (PageSwitchSlider != null) {
                if (DisplayedPage == UIConnections){
                    PageSwitchSlider.SetState(true);
                }
                if (DisplayedPage == UIAdvertisements){
                    PageSwitchSlider.SetState(false);
                }
            }

        }



        public static void SettingButtonFunction(){
            if (_TransitionForwardRunning || _TransitionBackRunning) return;
            if (UIDeviceSetting != null &&
                !UIDeviceSetting.IsDisplayed) // if we dont have the setting displayed
            {
                if (SettingButton != null) SettingButton.RotateAntiClockwise();
                TransitionForward(UIDeviceSetting);
                return;
            }
            if (UIDeviceSetting != null &&
                UIDeviceSetting.IsDisplayed) // if we have the setting displayed
            {
                if (SettingButton != null) SettingButton.RotateClockwise();
                TransitionBack();
                return;
            }

        }


        public static void PageSwitchSliderFunction(bool state) {
            
            
            if (state == true && 
                UIConnections != null &&
                DisplayedPage != UIConnections) 
            { 
                TransitionForward(UIConnections);
            }

            if (state == false &&
                UIAdvertisements != null &&
                DisplayedPage != UIAdvertisements)
            {
                TransitionForward(UIAdvertisements);
            }


        }


    }
}
