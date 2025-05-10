using System.Threading.Tasks;
using InputConnect.Setting;
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


    class Uniform
    {
        public double Duration; //in ms
        public double StartingValue;
        public double EndingValue;
        public Action<double>? Trigger;
        public double CurrentValue;
        public int Tick = Config.Tick; //in ms (this is 125fps)
        //public bool LinearDeceleration = false;

        public bool FunctionRunning = false;


        public async void TranslateForward(object? sender = null, object? e = null)
        {

            if (!FunctionRunning)
            {
                if (CurrentValue == EndingValue) return;
                _startingValue = StartingValue;
                _endingValue = EndingValue;
                _duration = Duration;
                _timeStamp = 0;
                _stopwatch.Reset();
                _stopwatch.Start();
                await Transation();
            }
            else
            {
                if (CurrentValue == EndingValue) return;
                _stopwatch.Stop();
                _startingValue = CurrentValue;
                _endingValue = EndingValue;
                _duration = Duration * ((_endingValue - CurrentValue) / (_endingValue - _startingValue));
                _timeStamp = 0;
                _stopwatch.Reset();
                _stopwatch.Start();
            }
        }

        public async void TranslateBackward(object? sender = null, object? e = null)
        {
            if (!FunctionRunning)
            {
                if (CurrentValue == StartingValue) return;
                _startingValue = EndingValue;
                _endingValue = StartingValue;
                _duration = Duration;
                _timeStamp = 0;
                _stopwatch.Reset();
                _stopwatch.Start();
                await Transation();
            }
            else
            {
                if (CurrentValue == StartingValue) return;
                _stopwatch.Stop();
                _startingValue = CurrentValue;
                _endingValue = StartingValue;
                _duration = Duration * ((_endingValue - CurrentValue) / (_endingValue - _startingValue));
                _timeStamp = 0;
                _stopwatch.Reset();
                _stopwatch.Start();
            }
        }

        bool _reset = false;
        public void Reset(object? sender = null, object? e = null)
        { // does not need to be async function 
            FunctionRunning = false;
            _reset = true;
        }




        private Stopwatch _stopwatch = new Stopwatch();
        private double _startingValue;
        private double _endingValue;
        private double _duration;
        private double _timeStamp = 0;

        async private Task Transation()
        {
            FunctionRunning = true;
            while (_stopwatch.ElapsedMilliseconds < Duration && FunctionRunning)
            {
                if (FunctionRunning == false) break;
                if (_stopwatch.ElapsedMilliseconds - _timeStamp < Tick)
                {
                    await Task.Delay(Tick / 2);
                    continue;
                }
                _timeStamp += Tick;
                double _delta = _timeStamp / _duration * (_endingValue - _startingValue);
                //if (LinearDeceleration) _delta = _delta * (2 - (_timeStamp / _duration));


                CurrentValue = _delta + _startingValue;

                // this if statement is a for redundency for NaN values and also 
                if (Trigger != null && !double.IsNaN(CurrentValue) &&
                    CurrentValue <= Math.Max(StartingValue, EndingValue) &&
                    CurrentValue >= Math.Min(StartingValue, EndingValue))
                    Trigger(CurrentValue);
            }

            if (FunctionRunning == false && _reset)
            { // this means we reseted
                CurrentValue = StartingValue;
                _reset = false;
                return;
            }

            if (Trigger != null && !double.IsNaN(_endingValue) &&
                CurrentValue <= Math.Max(StartingValue, EndingValue) &&
                CurrentValue >= Math.Min(StartingValue, EndingValue))
                Trigger(_endingValue);

            FunctionRunning = false;
            if (Trigger != null) Trigger(_endingValue); // run the function one last time after the FunctionRunning has been ran
                                                        // so my fellow developer can make a good use of it 
        }
    }
}
