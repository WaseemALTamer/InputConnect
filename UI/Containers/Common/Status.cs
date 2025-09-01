using InputConnect.Setting;
using InputConnect;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Data;






namespace InputConnect.UI.Containers.Common
{
    internal class Status : Border
    {

        private Animations.Transations.Uniform? ColorTransation;
        private Animations.Transations.Uniform? PulseColorTransation;


        public Status() {
            CornerRadius = new Avalonia.CornerRadius(1000);
            Width = 25;
            Height = 25;
            Opacity = 0.7;

            Background = new SolidColorBrush(Color.FromUInt32(0x00000000)); // we start it with the invisable color
        }


        private IBrush? _startingColor;
        private IBrush? _targetColor;
        public void SetColor(IBrush newColor, double durtaion) {

            _startingColor = Background;
            _targetColor = newColor;

            if (ColorTransation != null && 
                ColorTransation.FunctionRunning == true) 
            {
                ColorTransation.Reset();
            }

            ColorTransation = new Animations.Transations.Uniform{
                StartingValue = 0,
                EndingValue = 1,
                Duration = durtaion,
                Trigger = ColorTransitionTrigger
            };

            ColorTransation.TranslateForward();

        }



        private void ColorTransitionTrigger(double value){

            var startBrush = _startingColor as SolidColorBrush;
            var targetBrush = _targetColor as SolidColorBrush;

            byte Lerp(byte start, byte end) => (byte)(start + (end - start) * value);

            if (startBrush == null || targetBrush == null)
                return;

            var newColor = Color.FromArgb(
                Lerp(startBrush.Color.A, targetBrush.Color.A),
                Lerp(startBrush.Color.R, targetBrush.Color.R),
                Lerp(startBrush.Color.G, targetBrush.Color.G),
                Lerp(startBrush.Color.B, targetBrush.Color.B)
            );

            Background = new SolidColorBrush(newColor);
        }











        public void Reset() {
            SetColor(new SolidColorBrush(Color.FromUInt32(0x00000000)), Config.TransitionDuration);
        }


        private IBrush? _startingColorPulse;
        private IBrush? _ColorPulse1;
        private IBrush? _ColorPulse2;
        private double? _durtaion1;
        private double? _durtaion2;

        public void Pulse(IBrush Color1, IBrush Color2, double durtion1, double durtion2) {

            // this will go to the color 1 first at durtaion 1 and then 
            // will go to Color 2 at durtaion 2

            

            if (PulseColorTransation != null &&
                PulseColorTransation.FunctionRunning == true)
            {
                PulseColorTransation.Reset();
            }

            _startingColorPulse = Background;

            _ColorPulse1 = Color1;
            _ColorPulse2 = Color2;

            _durtaion1 = durtion1;
            _durtaion2 = durtion2;


            PulseColorTransation = new Animations.Transations.Uniform{
                StartingValue = 0,
                EndingValue = 1,
                Duration = durtion1 + durtion2,
                Trigger = ColorTransitionTriggerPulse
            };

            PulseColorTransation.TranslateForward();

        }



        private void ColorTransitionTriggerPulse(double value){

            var color1 = _startingColorPulse as SolidColorBrush;
            var color2 = _ColorPulse1 as SolidColorBrush;
            var color3 = _ColorPulse2 as SolidColorBrush;

            if (_durtaion1 == null ||
                _durtaion2 == null ||
                color1 == null ||
                color2 == null||
                color3 == null) return;


            byte Lerp(byte start, byte end, double _value) => (byte)(start + (end - start) * _value);



            var t1 = _durtaion1 / (_durtaion1 + _durtaion2);


            if (t1 >= value){
                double normal = (double)(1 - ((t1 - value) / t1));
                var newColor = Color.FromArgb(
                    Lerp(color1.Color.A, color2.Color.A, normal),
                    Lerp(color1.Color.R, color2.Color.R, normal),
                    Lerp(color1.Color.G, color2.Color.G, normal),
                    Lerp(color1.Color.B, color2.Color.B, normal)
                );

                Background = new SolidColorBrush(newColor);
            }
            else {
                double normal = (double)((value  - t1) /(1 - t1));
                var newColor = Color.FromArgb(
                    Lerp(color2.Color.A, color3.Color.A, normal),
                    Lerp(color2.Color.R, color3.Color.R, normal),
                    Lerp(color2.Color.G, color3.Color.G, normal),
                    Lerp(color2.Color.B, color3.Color.B, normal)
                );

                Background = new SolidColorBrush(newColor);

            }



            
        }



    }
}
