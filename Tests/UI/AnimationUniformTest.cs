using InputConnect.Connections;
using InputConnect.Structures;
using System.Threading.Tasks;
using System;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;




namespace InputConnect.Tests
{
    
    public class AnimationUniformTest : ITest
    {
        
        public string Name => "uniform_animation";


        private UI.Animations.Transations.Uniform? animation;


        private bool transition_running = false;


        private int transition_forward_ticks = 0;
        private void TransitionForwardTest(double value){
            transition_forward_ticks += 1;
            if (animation?.FunctionRunning == false) transition_running = false;
        }


        private int transition_backward_ticks = 0;
        private void TransitionBackwardTest(double value){
            transition_backward_ticks += 1;
            if (animation?.FunctionRunning == false) transition_running = false;
        }

        
        private int transition_halfway_ticks = 0;
        private void TransitionHalfTest(double value){
            if (animation?.FunctionRunning == false) transition_running = false;
            transition_halfway_ticks += 1;
            if (value > 0.5) animation?.TranslateBackward();
        }


        public async Task<int> Initialize()
        {
            animation = new UI.Animations.Transations.Uniform
            {
                StartingValue = 0,
                EndingValue = 1,
                Duration = 200,
            };

            // Forward transition test
            animation.Trigger += TransitionForwardTest;

            transition_running = true;
            animation.TranslateForward();

            var stopwatch = Stopwatch.StartNew();

            while (transition_running){
                if (stopwatch.Elapsed >= TimeSpan.FromSeconds(1)){
                    animation.Trigger -= TransitionForwardTest;
                    Console.WriteLine("Failed Trnaisition forward Test");
                    return -1;
                }

                await Task.Delay(10);
            }

            animation.Trigger -= TransitionForwardTest;
            Console.WriteLine(
                $"Passed transitioning forward total ticks = {transition_forward_ticks}"
            );


            // Backward transition test
            animation.Trigger += TransitionBackwardTest;

            transition_running = true;
            animation.TranslateBackward();

            stopwatch.Restart();

            while (transition_running){
                if (stopwatch.Elapsed >= TimeSpan.FromSeconds(1)){
                    animation.Trigger -= TransitionBackwardTest;
                    Console.WriteLine("Failed Trnaisition backward Test");
                    return -1;
                }

                await Task.Delay(10);
            }

            animation.Trigger -= TransitionBackwardTest;
            Console.WriteLine(
                $"Passed transitioning backward total ticks = {transition_backward_ticks}"
            );


            // halfway transition test
            animation.Trigger += TransitionHalfTest;

            transition_running = true;
            animation.TranslateForward();

            stopwatch.Restart();

            while (transition_running){
                if (stopwatch.Elapsed >= TimeSpan.FromSeconds(1)){
                    animation.Trigger -= TransitionHalfTest;
                    Console.WriteLine("Failed Trnaisition backward Test");
                    return -1;
                }

                await Task.Delay(10);
            }

            animation.Trigger -= TransitionHalfTest;
            Console.WriteLine(
                $"Passed transitioning halfway total ticks = {transition_backward_ticks}"
            );

            return 1;
        }



    }
}