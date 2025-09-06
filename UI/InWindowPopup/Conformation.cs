using Avalonia;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InputConnect.UI.InWindowPopup
{
    public class Conformation : Base 
    {

        private Action? _ConfirmTrigger;
        public Action? ConfirmTrigger
        {
            get { return _ConfirmTrigger; }
            set { _ConfirmTrigger = value; }
        }



        private Action? _ReturnTrigger;
        public Action? ReturnTrigger{
            get { return _ReturnTrigger; }
            set { _ReturnTrigger = value; }
        }

        private string? _Note;
        public string? Note{
            get { return _Note; }
            set { _Note = value; }
        }


        private TextBlock? TitleTextBlock;
        private Button? ConfirmButton;
        private Button? ReturnButton;
        private TextBlock? NoteTextBlock;




        public Conformation(Canvas master) : base(master) 
        {
            if (MainCanvas == null) 
                return;

            TitleTextBlock = new TextBlock{
                Text = "Confirmation",
                FontSize = Setting.Config.FontSize * 1.5,
                Width = 250,
                Height = 50,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,

            };
            MainCanvas.Children.Add(TitleTextBlock);



            ConfirmButton = new Button {
                Content = "Confirm",
                Width = 150,
                Height = 50,
                Background = Setting.Themes.Button,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                FontSize = Setting.Config.FontSize,
                CornerRadius = new CornerRadius(Setting.Config.CornerRadius)
            };
            MainCanvas.Children.Add(ConfirmButton);
            ConfirmButton.Click += (s, e) => ConfirmTrigger?.Invoke();
            ConfirmButton.Click += (s, e) => HideRight();

            ReturnButton = new Button {
                Content = "Return",
                Width = 150,
                Height = 50,
                Background = Setting.Themes.Button,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                FontSize = Setting.Config.FontSize,
                CornerRadius = new CornerRadius(Setting.Config.CornerRadius)
            };
            MainCanvas.Children.Add(ReturnButton);
            ReturnButton.Click += (s,e) => ReturnTrigger?.Invoke();
            ReturnButton.Click += (s, e) => Hide();



            NoteTextBlock = new TextBlock{
                Text = "",
                FontSize= Setting.Config.FontSize,
                Width = MainCanvas.Width - 50,
                Height = MainCanvas.Height - 100,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,

            };
            MainCanvas.Children.Add(NoteTextBlock);




            SizeChanged += OnSizeChanged;
        }


        public void Update() {

            if (NoteTextBlock != null)
                NoteTextBlock.Text = Note;

            OnSizeChanged(null, null); // size change for redundency
        }


        public void OnSizeChanged(object? sender, object? e) {

            if (MainCanvas == null) return;


            if (TitleTextBlock != null) {

                Canvas.SetLeft(TitleTextBlock, 25);
                Canvas.SetTop(TitleTextBlock, 25);

                if (NoteTextBlock != null){
                    NoteTextBlock.Width = MainCanvas.Width - 100;
                    NoteTextBlock.Height = MainCanvas.Height - 100;

                    Canvas.SetLeft(NoteTextBlock, 25);
                    Canvas.SetTop(NoteTextBlock, TitleTextBlock.Height + 25);

                }
            }

            if (ConfirmButton != null) {
                Canvas.SetLeft(ConfirmButton, ((MainCanvas.Width - ConfirmButton.Width) / 2) - ((ConfirmButton.Width / 2) + 5));
                Canvas.SetTop(ConfirmButton, MainCanvas.Height - ConfirmButton.Height - 10);
            }

            if (ReturnButton != null){
                Canvas.SetLeft(ReturnButton, ((MainCanvas.Width - ReturnButton.Width) / 2) + ((ReturnButton.Width / 2) + 5));
                Canvas.SetTop(ReturnButton, MainCanvas.Height - ReturnButton.Height - 10);
            }




        }



    }
}
