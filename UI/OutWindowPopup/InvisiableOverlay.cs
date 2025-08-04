using Avalonia.Threading;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;





namespace InputConnect.UI.OutWindowPopup
{
    public class InvisiableOverlay : Window
    {
        // this file  perpose is to cover  the whole screen with an
        // invisiable window that is not shown to the user its main
        // perpose is fo abbsorb keystokes  or mouse clicks, though




        public InvisiableOverlay() {
            Background = new SolidColorBrush(Color.FromUInt32(0x33000000)); // this sets it to be invisable
            WindowState = WindowState.FullScreen;
            SystemDecorations = SystemDecorations.None;
            IsHitTestVisible = true;
            ShowInTaskbar = false;
            Title = "Abbsorber";
            Closing += OnClose;
            Hide();
            Cursor = new Cursor(StandardCursorType.None);

            // now we can attack the functions for the abbsorber
            Controllers.Mouse.OnVirtualMointorEnter += OnVertualMointorEnterTrigger;
            Controllers.Mouse.OnVirtualMointorExit += OnVertualMointorExitTrigger;
        }



        public void OnVertualMointorEnterTrigger(){
            // ensure that is is on the right thread
            Dispatcher.UIThread.Post(() => {
                Show();
            });
        }

        public void OnVertualMointorExitTrigger(){
            // ensure that is is on the right thread
            Dispatcher.UIThread.Post(() => {
                Hide();
            });
        }

        public void OnClose(object? sender = null, WindowClosingEventArgs? e = null) {
            if (e == null) return;
            e.Cancel = true;
        }

    }
}
