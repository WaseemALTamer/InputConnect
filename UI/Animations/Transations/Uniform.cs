using System.Threading.Tasks;
using System.Diagnostics;
using System;


namespace InputConnect.UI.Animations.Transations
{


    // this file works everywhere and is supported on all devices this
    // file aims to provide a smooth uniform transition from one value
    // to another while running a functions that you  provide  it  for
    // each tick that happens

    // this transitions class is the bases of all the other transition
    // classes so this file is really important to have working


    class Uniform : IAnimation
    {
        public double Duration; //in ms
        public double StartingValue;
        public double EndingValue;
        public Action<double>? Trigger; // you didnt invoke the trigger you just excauted it as it is why?
        public double CurrentValue;
        public int Tick = Setting.Config.Tick; //in ms (this is 125fps)
        //public bool LinearDeceleration = false;

        public bool FunctionRunning = false;


        public Uniform(){
            AnimationManager.Add(this); // add it to the manager so the manger excautes updates it for you
        }

        ~Uniform(){
            AnimationManager.Remove(this); // upon desposing the class remove it from the List
        }



        public void TranslateForward(object? sender = null, object? e = null)
        {
            // leave these commented out this is because if you transition forward
            // and backward really really  fast then  the opposite transition wont
            // work leaving the next few lines commented out makes it work

            //if (CurrentValue == EndingValue)
            //    return;

            _startingValue = StartingValue;
            _endingValue = EndingValue;

            FunctionRunning = true;
        }

        public void TranslateBackward(object? sender = null, object? e = null)
        {
            // leave these commented out this is because if you transition forward
            // and backward really really  fast then  the opposite transition wont
            // work leaving the next few lines commented out makes it work

            //if (CurrentValue == StartingValue)
            //    return;

            _startingValue = EndingValue;
            _endingValue = StartingValue;

            FunctionRunning = true;
        }


        public void Reset(object? sender = null, object? e = null){ 
            // does not need to be async function 
            FunctionRunning = false;
            CurrentValue = StartingValue;
        }


        private bool _paused = false;
        public void Pause() {
            // does not need to be an async function
            _paused = true;
            FunctionRunning = false;
        }

        public async void Resume() {
            _paused = false;
        }


        private double _startingValue;
        private double _endingValue;


        public void Update(double dt){
            if (!FunctionRunning)
                return;

            if (_paused)
                return;

            double speed = (_endingValue - _startingValue) / Duration;

            CurrentValue += speed * dt;

            // Check if we reached the destination
            if (speed > 0 && CurrentValue >= _endingValue)
            {
                CurrentValue = _endingValue;
                FunctionRunning = false;
            }
            else if (speed < 0 && CurrentValue <= _endingValue){
                CurrentValue = _endingValue;
                FunctionRunning = false;
            }

            // Trigger current value
            if (Trigger != null &&
                !double.IsNaN(CurrentValue))
            {
                Trigger.Invoke(CurrentValue);
            }
        }
    }
}
