using Avalonia.Controls;
using Avalonia.Media;
using System;




namespace InputConnect.UI.OutWindowPopup
{
    public class InvisiableOverlay : Window
    {
        // this file  perpose is to cover  the whole screen with an
        // invisiable window that is not shown to the user its main
        // perpose is fo abbsorb keystokes  or mouse clicks, though




        public InvisiableOverlay() { 
            Background = new SolidColorBrush(Color.FromUInt32(0x00000000)); // this sets it to be invisable
            IsHitTestVisible = true;
            WindowState = WindowState.FullScreen;
            ShowInTaskbar = false;
            Title = "Abbsorber";
            Closing += OnClose;
            Hide();
        }



        public void OnClose(object? sender = null, WindowClosingEventArgs? e = null) {
            if (e == null) return;
            e.Cancel = true;
        }

    }
}
