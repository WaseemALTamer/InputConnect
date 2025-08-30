using System;
using System.Diagnostics;
using System.Threading.Tasks;
using InputConnect.Controllers.Mouse;
using SDL2;

namespace InputConnect.UI.OutWindowPopup
{
    public class InvisiableOverlaySDL{


        private IntPtr _Window;
        public IntPtr Window{
            get { return _Window; }
            set { _Window = value; }
        }


        private bool _IsVisible;
        public bool IsVisible{
            get { return _IsVisible; }
            set { _IsVisible = value; }
        }

        private bool _IsRunning;
        public bool IsRunning
        {
            get { return _IsRunning; }
            set { _IsRunning = value; }
        }

        private Task? _EventLoopTask;
        public Task? EventLoopTask{
            get { return _EventLoopTask; }
            set { _EventLoopTask = value; }
        }


        private Action? _OnShow;
        public Action? OnShow{
            get { return _OnShow; }
            set { _OnShow = value; }
        }

        private Action? _OnHide;
        public Action? OnHide{
            get { return _OnHide; }
            set { _OnHide = value; }
        }


        public Action<int, int>? OnMouseMove { get; set; } // dx, dy
        public Action<int>? OnMouseButtonPress { get; set; }
        public Action<int>? OnMouseButtonRelease { get; set; }
        public Action<int, int>? OnMouseScroll { get; set; } // direction +1, -1 => X axis, Y axis


        public InWindowMouse? MouseController { get; set; }


        public InvisiableOverlaySDL(){
            if (SDL.SDL_Init(SDL.SDL_INIT_AUDIO) < 0)
                throw new Exception("SDL could not initialize! " + SDL.SDL_GetError());

            // Create fullscreen, borderless, transparent window
            Window = SDL.SDL_CreateWindow(
                "Absorber",
                SDL.SDL_WINDOWPOS_CENTERED,
                SDL.SDL_WINDOWPOS_CENTERED,
                1920, 1080, // fallback resolution
                SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP |
                SDL.SDL_WindowFlags.SDL_WINDOW_BORDERLESS |
                SDL.SDL_WindowFlags.SDL_WINDOW_ALWAYS_ON_TOP |
                SDL.SDL_WindowFlags.SDL_WINDOW_HIDDEN |
                SDL.SDL_WindowFlags.SDL_WINDOW_SKIP_TASKBAR
            );

            if (Window == IntPtr.Zero)
                throw new Exception("Window could not be created! " + SDL.SDL_GetError());


            SDL.SDL_SetWindowOpacity(Window, 0f);
            //SDL.SDL_ShowCursor(SDL.SDL_DISABLE);
            

            IsVisible = false;
            IsRunning = true;

            // Start background event loop
            EventLoopTask = Task.Run(EventLoop);

            // Hook mouse enter/exit like Avalonia version
            GlobalMouse.OnVirtualMointorEnter += OnVirtualMointorEnterTrigger;
            GlobalMouse.OnVirtualMointorExit += OnVirtualMointorExitTrigger;


            MouseController = new InWindowMouse(this);
        }

        private Stopwatch _stopwatch = new Stopwatch();
        private double _timeStamp = 0;
        private bool FunctionRunning = false;

        private async Task EventLoop(){
            FunctionRunning = true;
            _stopwatch.Restart();

            while (IsRunning && FunctionRunning){
                // tick timing
                if (_stopwatch.ElapsedMilliseconds - _timeStamp < Setting.Config.MouseUpdateTickRate){
                    await Task.Delay(Setting.Config.MouseUpdateTickRate / 2);
                    continue;
                }
                _timeStamp += Setting.Config.MouseUpdateTickRate;

                if (IsVisible){
                    SDL.SDL_SetWindowInputFocus(Window);
                    SDL.SDL_RaiseWindow(Window);
                }

                int totalDx = 0;
                int totalDy = 0;
                bool mouseMoved = false;

                // drain all SDL events this tick
                while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1){
                    switch (e.type){
                        case SDL.SDL_EventType.SDL_QUIT:
                            break;

                        case SDL.SDL_EventType.SDL_MOUSEMOTION:
                            // overwrite with the latest relative motion
                            totalDx += e.motion.xrel;
                            totalDy += e.motion.yrel;
                            mouseMoved = true;
                            break;

                        case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                            OnMouseButtonPress?.Invoke(e.button.button);
                            break;

                        case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
                            OnMouseButtonRelease?.Invoke(e.button.button);
                            break;

                        case SDL.SDL_EventType.SDL_MOUSEWHEEL:
                            int scrollX = e.wheel.x;
                            int scrollY = e.wheel.y;
                            OnMouseScroll?.Invoke(scrollX, scrollY);
                            break;
                    }
                }

                // only invoke once per tick
                if (mouseMoved){
                    OnMouseMove?.Invoke(totalDx, totalDy);
                }
            }
        }







        public void OnVirtualMointorEnterTrigger(){
            Show();
            OnShow?.Invoke();
        }

        public void OnVirtualMointorExitTrigger(){
            Hide();
            OnHide?.Invoke();
        }

        public void Show(){
            if (!IsVisible){
                SDL.SDL_ShowWindow(Window);
                SDL.SDL_RaiseWindow(Window);
                SDL.SDL_SetWindowInputFocus(Window);
                SDL.SDL_SetWindowOpacity(Window, 1f);
                SDL.SDL_SetRelativeMouseMode(SDL.SDL_bool.SDL_TRUE);
                IsVisible = true;
            }
        }

        public void Hide(){
            if (IsVisible){
                SDL.SDL_SetWindowOpacity(Window, 0f);
                SDL.SDL_HideWindow(Window);
                //SDL.SDL_MinimizeWindow(Window);
                SDL.SDL_SetRelativeMouseMode(SDL.SDL_bool.SDL_FALSE);
                IsVisible = false;
            }
        }

        public void Close(){
            IsRunning = false;
            EventLoopTask?.Wait();

            SDL.SDL_DestroyWindow(Window);
            SDL.SDL_Quit();
        }
    }
}
