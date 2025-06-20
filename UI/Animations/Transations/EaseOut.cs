using InputConnect.Setting;
using System;


namespace InputConnect.UI.Animations.Transations
{
    class EaseOut
    {

        // this function is based on the unfirom transition function but applies mathmatical equation
        // f(x) = (1 − (1 − x)^d) * (b−a)
        // b = ending value
        // a = Starting value
        // d = damping
        // a = intial value


        private Uniform? Transition;
        public double Duration; //in ms
        public double StartingValue;
        public double EndingValue;
        public Action<double>? Trigger;
        public double CurrentValue;
        public int Tick = Config.Tick; //in ms (this is 125fps)

        public double Damping = 2; // this is the expenatal the higher it is the faster its  going to be
                                   // this value must to be zero and if you make it below 1 then you get
                                   // an ease In function

        public bool FunctionRunning = false;


        public EaseOut(){
            Transition = new Uniform{
                StartingValue = 0,
                EndingValue = 1,
                CurrentValue = CurrentValue / (EndingValue + StartingValue),
                Duration = Duration,
                Trigger = _Trigger,
            };
        }



        public void TranslateForward(){
            if (Transition == null) return;

            Transition.Duration = Duration;


            Transition.TranslateForward();
        }

        public void TranslateBackward(){
            if (Transition == null) return;

            Transition.Duration = Duration;

            Transition.TranslateBackward();
        }

        public void Reset(object? sender = null, object? e = null){ // does not need to be async function
            if (Transition == null) return;
            FunctionRunning = false;
            Transition.Reset();
        }


        public void Pause(){
            if (Transition == null) return;
            Transition.Pause();
        }

        public void Resume() {
            if (Transition == null) return;
            Transition.Resume();
        }



        private void _Trigger(double Value){
            if (Transition == null) return;
            FunctionRunning = Transition.FunctionRunning;
            double _delta = (1 - Math.Pow(1 - Value, Damping)) * (EndingValue - StartingValue);
            CurrentValue = StartingValue + _delta;
            if (Trigger != null) Trigger(CurrentValue);
        }

    }
}
