





using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Animation;
using Avalonia.Threading;

namespace InputConnect.UI.Animations
{
    

    public static class AnimationManager{
        // this class is a ticksSystem that  is designed  soully  for to  syncrnise all  the animation
        // and to ditch the async approch where each Transition/animation gets its own async excuation
        // time be aware that this will have its own tick system  and the  the animation  will also be
        // backwared compatable


        // am going to keep this public but make sure you use the wrapers unless you know what you are doing
        // if you dont use the wrapers you risking a race condition
        public static readonly List<IAnimation> Animations = new List<IAnimation>();

        // just to be clear if you are running  a EaseInOut  transition note that the transition it self is just
        // a uniform transition under the hood and the transition it self is not a IAnimation as of now 7/8/2026


        private static bool running = false;


        public static void Initialize(){
            if (running)
                return;

            running = true;

            _ = UpdateLoop();
        }


        private static TimeSpan lastUpdate;

        private static async Task UpdateLoop()
        {
            var stopwatch = Stopwatch.StartNew();

            while (running){
                var tickStart = stopwatch.Elapsed;

                // calculate time since previous update
                var deltaTime = tickStart - lastUpdate;
                lastUpdate = tickStart;

                //Console.WriteLine(deltaTime.TotalMilliseconds);

                foreach (IAnimation animation in Animations){
                    animation.Update(deltaTime.TotalMilliseconds);
                }

                // calculate how long this tick took
                var executionTime = stopwatch.Elapsed - tickStart;

                // only sleep for the remaining time
                var remainingTime =
                    TimeSpan.FromMilliseconds(Setting.Config.Tick) - executionTime;

                if (remainingTime > TimeSpan.Zero){
                    await Task.Delay(remainingTime);
                }
                else{
                    // if we took longer than  the tick interval let the other functions run
                    // but come back to this update  as soon as  possible this avoids errors
                    // on low end devices this needs more exterme  testing  use the i7 7500u
                    // laptop prefarably disable the graphic card on the machine while doing
                    // it
                    await Task.Yield();
                }
            }
        }

        //warpers incase we want to add extra logic later on down the line
        public static void Add(IAnimation animation){

            // we dispach it with the UIThread so it is thread safe
            Dispatcher.UIThread.Post(() =>{
                Animations.Add(animation);
            });
        }

        public static void Remove(IAnimation animation){
            // we dispach it with the UIThread so it is thread safe
            Dispatcher.UIThread.Post(() =>{
                Animations.Remove(animation);
            });
        }

    }
}