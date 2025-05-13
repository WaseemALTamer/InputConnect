



using Avalonia.Controls;

namespace InputConnect.UI.WindowPopup
{
    public static class Global{
        
        // this file is created for all the in window popups to share it
        // this is soly for  the overlay  that will  cover the bottom of 
        // anything below the  popup, but if you  want to make more than
        // one popup which is in another window this will be an issue as
        // a result you may need to create an overlay for each pop which
        // would make sense but this methode is more effecnet if you ask
        // me and it would require less ram to



        private static Overlay? _Overlay;
        public static Overlay? Overlay{
            get { return _Overlay; }
            set { _Overlay = value; }
        }
        
        
    }
}
