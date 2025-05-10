using System.Threading.Tasks;
using InputConnect.Setting;
using System.Diagnostics;
using Avalonia.Input;
using System;
using Avalonia.Controls;





namespace InputConnect.UI.Animations
{

    // this file is only  used for scrolling and should not be used for draging
    // this file is only for computers and does not support websites for phones
    // either it does for webstis for desktop though


    // we treat the scroll bar as a particle just in physics and  math  classes
    // this allows the scrollbar to be smooth



    class SmoothScrolling
    {


        public Action<double>? Trigger { get; set; }



        //Handles SmoothScrolling
        public double ScrollingImpulseSpeed = 1000; //this will be timesd by avarage scroll impulse which is 50 => 50*20 = 1000 pixles/s 
        public double ScrollingConstantDeacceleration = 2000; //2000 pixles/s
        public double ScrollingMaxVelocity = 1000;
        public double ScrollingTrusholdVelocity = 1000;
        public double ScrollingDamping = 3; // damping will only be applied after the speed goes behind the trushold velocity
        public int Tick = Config.Tick; //in ms (7ms is about 125fps)
        public int TimeStamp = 0; //in ms

        public bool StopCurrentAnimation = false;




        private bool FunctionRunning = false;
        private double CurrentVelocity = 0;
        private double CurrentDirection = 0; // 1 is going down and -1 is going up
        private Stopwatch stopWatch = new Stopwatch();




        //Over ride the event handler with this function
        public async void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            e.Handled = true; //OverRiding the scrolling function
            if (-(e.Delta.Y / Math.Abs(e.Delta.Y)) != CurrentDirection)
            {
                CurrentVelocity = 0;
            }


            CurrentDirection = -(e.Delta.Y / Math.Abs(e.Delta.Y));
            CurrentVelocity += Math.Abs(ScrollingImpulseSpeed * e.Delta.Y);

            if (!FunctionRunning)
            {
                await ApplySmoothScrolling();
            }
        }

        // this is only used for debugging for erros, either adapt this function or remove it
        public void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
        {
            e.Handled = true; //OverRiding the scrolling function
            Console.WriteLine(e.OffsetDelta.Y);
        }




        private async Task ApplySmoothScrolling()
        {
            FunctionRunning = true;
            stopWatch.Start();
            stopWatch.Restart();


            while (CurrentVelocity > 0)
            {
                if (stopWatch.ElapsedMilliseconds - TimeStamp < Tick)
                {
                    await Task.Delay(Tick / 2); //this was "Tick / 2" but was changed to "1" for prformence reasons
                    continue;
                }
                if (StopCurrentAnimation)
                {
                    StopCurrentAnimation = false;
                    break;
                }

                TimeStamp += Tick;

                double _progress = CurrentVelocity * Tick / 1000;


                if (Trigger != null) Trigger(_progress * CurrentDirection);

                if (ScrollingMaxVelocity < CurrentVelocity)
                {
                    CurrentVelocity -= ScrollingConstantDeacceleration * 2 * CurrentVelocity / ScrollingTrusholdVelocity * Tick / 1000;
                    continue;
                }

                if (CurrentVelocity < ScrollingTrusholdVelocity)
                {
                    CurrentVelocity -= ScrollingConstantDeacceleration * ScrollingDamping * Tick / 1000;
                    continue;
                }

                CurrentVelocity -= ScrollingConstantDeacceleration * Tick / 1000; // apply deacceleration
            }

            CurrentVelocity = 0;
            FunctionRunning = false;
            stopWatch.Stop();
            TimeStamp = 0;
        }
    }
}