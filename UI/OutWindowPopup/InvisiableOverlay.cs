using Avalonia.Threading;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System;
using InputConnect.Structures;
using Tmds.DBus.Protocol;





namespace InputConnect.UI.OutWindowPopup
{
    public class InvisiableOverlay : Window
    {
        // this file  perpose is to cover  the whole screen with an
        // invisiable window that is not shown to the user its main
        // perpose is fo abbsorb keystokes  or mouse clicks, though


        private MouseController? mouseController;


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

        public InvisiableOverlay() {
            Background = new SolidColorBrush(Color.FromUInt32(0x00000000));
            
            //Background = new SolidColorBrush(Color.FromUInt32(0x33000000)); // this sets it to be invisable
            
            WindowState = WindowState.FullScreen;
            SystemDecorations = SystemDecorations.None;
            IsHitTestVisible = true;
            ShowInTaskbar = false;
            Topmost = true;
            Title = "Abbsorber";
            Closing += OnClose;
            Hide();

            

            Cursor = new Cursor(StandardCursorType.None);


            mouseController = new MouseController(this);

            // now we can attack the functions for the abbsorber
            Controllers.Mouse.OnVirtualMointorEnter += OnVirtualMointorEnterTrigger;
            Controllers.Mouse.OnVirtualMointorExit += OnVirtualMointorExitTrigger;
        }



        public void OnVirtualMointorEnterTrigger(){
            // ensure that is is on the right thread
            Dispatcher.UIThread.Post(() => {

                if (OnShow != null) OnShow.Invoke();

                if (mouseController != null)
                    mouseController.IsHidden = false;



                Show();
            });
        }

        public void OnVirtualMointorExitTrigger(){
            // ensure that is is on the right thread
            Dispatcher.UIThread.Post(() => {
                Hide();

                if (OnHide != null) OnHide.Invoke();
            });
        }

        public void OnClose(object? sender = null, WindowClosingEventArgs? e = null) {
            if (e == null) return;
            e.Cancel = true;
        }

    }
}
