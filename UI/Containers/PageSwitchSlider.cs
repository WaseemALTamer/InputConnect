using InputConnect.UI.Containers.Common;
using Avalonia.Controls;
using System;


namespace InputConnect.UI.Containers
{
    public class PageSwitchSlider : Border
    {
        // this class main perpose is to provide a button that holds 2 icons at both sides
        // that can be used to toggle between two  pages the  Advertisments discovery page
        // or the connection to page


        private Canvas? _Master;
        public Canvas? Master{
            get { return _Master; }
            set { _Master = value; }
        }

        private Canvas? _MainCanvas;
        public Canvas? MainCanvas{
            get { return _MainCanvas; }
            set { _MainCanvas = value; }
        }

        private Action<bool>? _Trigger;
        public Action<bool>? Trigger{
            get { return _Trigger; }
            set { _Trigger = value; }
        }

        private Image? Search;
        private Image? Chains;
        private SlideButton? Button;


        public PageSwitchSlider(Canvas? master) {
            Master = master;
            Width = 200;
            Height = 80;


            MainCanvas = new Canvas{
                IsHitTestVisible = true,
                ClipToBounds = true
            };


            Button = new SlideButton { 
                Trigger = TriggerMap,
            };
            MainCanvas.Children.Add(Button);

            Search = new Image{
                Stretch = Avalonia.Media.Stretch.Uniform,
                Width = 50, Height = 50,
            };
            Assets.AddAwaitedActions(() => {
                Search.Source = Assets.SearchBitmap;
            });
            MainCanvas.Children.Add(Search);


            Chains = new Image{
                Stretch = Avalonia.Media.Stretch.Uniform,
                Width = 50, Height = 50,
            };
            Assets.AddAwaitedActions(() => {
                Chains.Source = Assets.ChainsBitmap;
            });
            MainCanvas.Children.Add(Chains);

            if (Master != null){
                OnResize(); // trigger the function to set the sizes
                Master.SizeChanged += OnResize;
            }

            Child = MainCanvas;

        }

        private void OnResize(object? sender = null, SizeChangedEventArgs? e = null){
            if (Master != null){

                if (MainCanvas != null) {

                    MainCanvas.Width = Width; 
                    MainCanvas.Height = Height;

                    if (Button != null) {
                        Canvas.SetLeft(Button, (Width - Button.Width) / 2);
                        Canvas.SetTop(Button, (Height - Button.Height) / 2);
                    }

                    if (Search != null){
                        Canvas.SetLeft(Search, 0);
                        Canvas.SetTop(Search, (Height - Search.Height) / 2);
                    }

                    if (Chains != null){
                        Canvas.SetLeft(Chains, (Width - Chains.Width));
                        Canvas.SetTop(Chains, (Height - Chains.Height) / 2);
                    }
                }

                Canvas.SetLeft(this, (Master.Width - Width) / 2);

            }
        }

        private void TriggerMap() { 
            if (Trigger != null && Button != null) Trigger.Invoke(Button.State);

            for (int i = 0; i < Connections.Devices.ConnectionList.Count; i++){
                var device = Connections.Devices.ConnectionList[i];

                Console.WriteLine(device.DeviceName);
                Console.WriteLine(device.State);
            }
        }


        public void SetState(bool state) { // this function will just pass the bool to the button
            if (Button != null) {
                Button.SetState(state);
            }
        }



    }
}
