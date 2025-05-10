


namespace InputConnect.Commands
{
    public class Mouse
    {
        // this is used to tell the other device where on screen they should be
        public double? X { get; set; } 
        public double? Y { get; set; }
        public double? scroll { get; set; }


        public bool? MouseHide { get; set; } // this is used to tell the other deivces if there mouse should be hidden
    }
}