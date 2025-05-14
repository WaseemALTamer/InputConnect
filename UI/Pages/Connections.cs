using Avalonia.Controls;
using InputConnect.UI.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputConnect.UI.Pages
{
    public class Connections : Base
    {


        private SlideButton? Test;

        public Connections(Canvas master) : base(master) 
        {
            if (MainCanvas == null || Master == null) return;

            Test = new SlideButton();
            MainCanvas.Children.Add(Test);


            MainCanvas.SizeChanged += OnResize;
        }


        private void OnResize(object? sender = null, SizeChangedEventArgs? e = null)
        {
            if (MainCanvas != null)
            {

                if (Test != null) {
                    Canvas.SetLeft(Test, (MainCanvas.Width - Test.Width) / 2);
                    Canvas.SetTop(Test, (MainCanvas.Height - Test.Height) / 2);
                }

                

            }
        }

    }
}
