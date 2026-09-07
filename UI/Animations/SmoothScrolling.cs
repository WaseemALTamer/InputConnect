using System.Threading.Tasks;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;
using System;





namespace InputConnect.UI.Animations
{

    // this file is only  used for scrolling and should not be used for draging
    // this file is only for computers and does not support websites for phones
    // either it does for webstis for desktop though


    // we treat the scroll bar as a particle just in physics and  math  classes
    // this allows the scrollbar to be smooth


    // scrolling down  with touchscreen  is not yet supported though the logic is
    // the exact same track the hold track the movment  of the  finger then track
    // when the finger leaves the display calcualte the speed of the fingure then
    // simply apply the speed to the as impulse and start the function though you
    // may want to multiply the impulse relative to the speed they are going



    class SmoothScrolling : IAnimation
    {


        public Action<double>? Trigger { get; set; }



        //Handles SmoothScrolling
        public double ScrollingImpulseSpeed = 1000; //this will be timesd by avarage scroll impulse which is 50 => 50*20 = 1000 pixles/s 
        public double ScrollingConstantDeacceleration = 2000; //2000 pixles/s
        public double ScrollingMaxVelocity = 1000;
        public double ScrollingTrusholdVelocity = 1000;
        public double ScrollingDamping = 3; // damping will only be applied after the speed goes behind the trushold velocity
        public int Tick = Setting.Config.Tick; //in ms (7ms is about 125fps)
        public int TimeStamp = 0; //in ms

        public bool StopCurrentAnimation = false;




        private bool FunctionRunning = false;
        private double CurrentVelocity = 0;
        private double CurrentDirection = 0; // 1 is going down and -1 is going up
        private Stopwatch stopWatch = new Stopwatch();


        public SmoothScrolling()
        {
            AnimationManager.Add(this);
        }

        ~SmoothScrolling()
        {
            AnimationManager.Remove(this);
        }


        //Over ride the event handler with this function
        public void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e){
            e.Handled = true; // overriding the scrolling function

            double direction = -(e.Delta.Y / Math.Abs(e.Delta.Y));

            // if direction changed, stop the existing momentum
            if (direction != CurrentDirection){
                CurrentVelocity = 0;
            }

            CurrentDirection = direction;

            CurrentVelocity += Math.Abs(
                ScrollingImpulseSpeed * e.Delta.Y
            );

            FunctionRunning = true;
        }

        // this is only used for debugging for erros, either adapt this function or remove it
        public void OnScrollChanged(object? sender, ScrollChangedEventArgs e){
            e.Handled = true; // overriding the scrolling function
            Console.WriteLine(e.OffsetDelta.Y);
        }




        public void Update(double dt)
        {
            if (!FunctionRunning)
                return;

            if (StopCurrentAnimation)
            {
                StopCurrentAnimation = false;
                CurrentVelocity = 0;
                FunctionRunning = false;
                return;
            }


            // convert milliseconds to seconds
            double dtSeconds = dt / 1000.0;


            // calculate how far we should scroll this frame
            double progress =
                CurrentVelocity *
                dtSeconds *
                CurrentDirection;


            Trigger?.Invoke(progress);


            // apply deceleration
            if (ScrollingMaxVelocity < CurrentVelocity){
                CurrentVelocity -=
                    ScrollingConstantDeacceleration *
                    2 *
                    CurrentVelocity /
                    ScrollingTrusholdVelocity *
                    dtSeconds;
            }
            else if (CurrentVelocity < ScrollingTrusholdVelocity){
                CurrentVelocity -=
                    ScrollingConstantDeacceleration *
                    ScrollingDamping *
                    dtSeconds;
            }
            else{
                CurrentVelocity -=
                    ScrollingConstantDeacceleration *
                    dtSeconds;
            }


            // Prevent velocity going negative
            if (CurrentVelocity <= 0){
                CurrentVelocity = 0;
                FunctionRunning = false;
            }
        }
    }
}