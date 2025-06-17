using InputConnect.UI.Containers;
using Avalonia.Controls;









namespace InputConnect.UI.Pages
{
    // this file will only provide the structure of the canvase and how it looks like
    // it will also provide the tools to add and remove  the  children  of it none of
    // its logic will be ran through it

    // this file ended also running the logic as well

    public class Device : Base{
        // this is responsible for sending the connection request
        // it will also be responsible for disconnecting and also
        // telling you
        private Connector? connector; 

        public Device(Canvas? master) : base(master)
        {

            if (MainCanvas == null) return;


            connector = new Connector(MainCanvas);
            MainCanvas.Children.Add(connector);


            OnShow += Update; // this will ensure that it updates evertime we display this page


            OnResize(); // trigger the function to set the sizes

            if (Master != null) {
                Master.SizeChanged += OnResize;
            }
        }

                   
        public void Update() { 
            // update the data that we need to work with



        }


        public void OnResize(object? sender = null, SizeChangedEventArgs? e = null){

            if (MainCanvas != null){
                MainCanvas.Width = Width; 
                MainCanvas.Height = Height;


                if (connector != null) {
                    Canvas.SetLeft(connector, MainCanvas.Width - connector.Width - 10);
                    Canvas.SetTop(connector, MainCanvas.Height - connector.Height - 10);
                }
            }
        }




       

    }
}
