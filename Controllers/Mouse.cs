using Avalonia.Controls;
using SharpHook;
using System;



namespace InputConnect.Controllers
{
    public static class Mouse
    {
        public static TaskPoolGlobalHook Hook = new TaskPoolGlobalHook();
        public static EventSimulator EventSimulator = new EventSimulator();


        public static Action<double, double>? OnMove; // => <x, y>
        public static Action<int>? OnMousePress; // => 0:None -> 1:Left -> 2:Right -> 3:Middle -> 4:Back -> 5:Forward
        public static Action<int>? OnMouseRealse; // => 0:None -> 1:Left -> 2:Right -> 3:Middle -> 4:Back -> 5:Forward
        public static Action<double>? OnScroll; // => <delta>



        public static double? CurrentPositionX;
        public static double? CurrentPositionY;





        public static bool StartHook(){
            Hook.MouseMoved += (_, e) =>{
                CurrentPositionX = e.Data.X;
                CurrentPositionY = e.Data.Y;
                OnMove?.Invoke(e.Data.X, e.Data.Y);
            };

            Hook.MousePressed += (_, e) =>{
                OnMousePress?.Invoke((int)e.Data.Button);
            };

            Hook.MouseReleased += (_, e) => {
                OnMouseRealse?.Invoke((int)e.Data.Button);
            };

            Hook.MouseWheel += (_, e) =>{
                OnScroll?.Invoke(e.Data.Delta * e.Data.Rotation / 360);
            };


            Hook.RunAsync();

            return true;
        }




        public static void MoveMouse(double x, double y) {
            EventSimulator.SimulateMouseMovement((short)x, (short)y);
        }

        public static void PressMouse(int button) {
            // => 0:None -> 1:Left -> 2:Right -> 3:Middle -> 4:Back -> 5:Forward
            EventSimulator.SimulateMousePress((SharpHook.Data.MouseButton)button);
        }

        public static void ReleaseMouse(int button){
            // => 0:None -> 1:Left -> 2:Right -> 3:Middle -> 4:Back -> 5:Forward
            EventSimulator.SimulateMouseRelease((SharpHook.Data.MouseButton)button);
        }

        public static void ScrollMouse(double delta) {
            EventSimulator.SimulateMouseWheel((short)(delta/delta * 360));
        }



    }
}