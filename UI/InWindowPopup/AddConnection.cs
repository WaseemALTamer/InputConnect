using InputConnect.Structures;
using InputConnect.Network;
using Avalonia.Threading;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia;
using InputConnect.UI.Containers;
using System;








namespace InputConnect.UI.InWindowPopup
{

    public class AddConnection : Base
    {
        


        private Button? AddButton;

        private TextBox? UsernameEntry;
        private TextBox? MacEntry;
        private TextBox? TokenEntry;


        public Action? OnAddButtonTrigger;


        public AddConnection(Canvas master) : base(master)
        {
            if (MainCanvas == null) return;

            MaxWidth = 600;
            MaxHeight = 400;

            Width = 600;
            Height = 400;


            AddButton = new Button{
                Content = "Add",
                Width = 150,
                Height = 50,
                Background = Setting.Themes.Button,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
                FontSize = Setting.Config.FontSize,
                CornerRadius = new CornerRadius(Setting.Config.CornerRadius)
            };
            MainCanvas.Children.Add(AddButton);
            AddButton.Click += OnClickAddButton;



            UsernameEntry = new TextBox
            {
                Text = "",
                Width = 300,
                Height = 40,
                FontSize = Setting.Config.FontSize,
                CornerRadius = new CornerRadius(Setting.Config.CornerRadius),
                Watermark = "Username",
                Background = Setting.Themes.Entry,
            };
            MainCanvas.Children.Add(UsernameEntry);


            MacEntry = new TextBox
            {
                Text = "",
                Width = 300,
                Height = 40,
                FontSize = Setting.Config.FontSize,
                CornerRadius = new CornerRadius(Setting.Config.CornerRadius),
                Watermark = "Mac",
                Background = Setting.Themes.Entry,
            };
            MainCanvas.Children.Add(MacEntry);


            TokenEntry = new TextBox
            {
                Text = "",
                Width = 300,
                Height = 40,
                FontSize = Setting.Config.FontSize,
                CornerRadius = new CornerRadius(Setting.Config.CornerRadius),
                Watermark = "Token",
                PasswordChar = char.Parse("*"),
                Background = Setting.Themes.Entry,
            };
            MainCanvas.Children.Add(TokenEntry);


            

            MainCanvas.SizeChanged += OnResize;

            OnShowTrigger += OnShow;

        }



        public void OnShow()
        {
            
            if (UsernameEntry != null){
                UsernameEntry.Text = "";
            }

            if (MacEntry != null){
                MacEntry.Text = "";

            }

            if (TokenEntry != null){
                TokenEntry.Text = "";
            }

        }


        public void OnResize(object? sender = null, SizeChangedEventArgs? e = null)
        {
            if (Master != null){
                if (MainCanvas != null) { 
                    MainCanvas.Width = Width; 
                    MainCanvas.Height = Height;


                    if (AddButton != null){
                        Canvas.SetLeft(AddButton, (MainCanvas.Width - AddButton.Width) / 2 );
                        Canvas.SetTop(AddButton, MainCanvas.Height - AddButton.Height - 10);
                    }


                    if (UsernameEntry != null){
                        Canvas.SetLeft(UsernameEntry, (MainCanvas.Width - UsernameEntry.Width) / 2 );
                        Canvas.SetTop(UsernameEntry, 50);

                    }

                    if (MacEntry != null){
                        Canvas.SetLeft(MacEntry, (MainCanvas.Width - MacEntry.Width) / 2 );
                        Canvas.SetTop(MacEntry, 125);
                    }

                    if (TokenEntry != null){
                        Canvas.SetLeft(TokenEntry, (MainCanvas.Width - TokenEntry.Width) / 2);
                        Canvas.SetTop(TokenEntry, 200);
                    }


                }
            }
        }


        private void OnClickAddButton(object? sender, object? e)
        {

            if (UsernameEntry == null || 
                MacEntry ==  null || 
                TokenEntry == null) 
                return;

            if (TokenEntry.Text == null) return;

            PasswordKey password_key = new PasswordKey(TokenEntry.Text);

            Connection newConnection = new Connection
            {
            
              DeviceName = UsernameEntry.Text,
              MacAddress = MacEntry.Text,
              PasswordKey = password_key,
              State = Connections.Constants.StateConnected,
              SequenceNumber = 0
            };

            Connections.Manager.AddConnection(newConnection); // add connection


            HideRight();


            if (OnAddButtonTrigger != null){
                OnAddButtonTrigger.Invoke();
            }
        }







    }
    
}