using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using System;

namespace pactheman_client {
    public class HostOrJoinMenu : Screen {

        public HostOrJoinMenu() {

            var player = (HumanPlayer)Environment.Instance.Actors["player"];

            var cancelBtn = new Button {
                Content ="Cancel",
                Margin = new Thickness(20, 0)
            };
            cancelBtn.Clicked += (sender, args) => {
                player.Disconnect();
                UIState.Instance.CurrentUIState = UIStates.PreGame;
                UIState.Instance.CurrentScreen = new PreGameMenu();
            };

            var hostBtn = new Button {
                Content = "Host",
                Margin = new Thickness(20, 0)
            };
            hostBtn.Clicked += async (sender, args) => {
                await player.Host();
            };

            var joinBtn = new Button {
                Content = "Join",
                IsEnabled = false,
                Margin = new Thickness(20, 0)
            };
            joinBtn.Clicked += async (sender, args) => {
                await player.Join();
            };

            var sessionIDBox = new TextBox {
                Width = 750,
                BackgroundColor = Color.Transparent,
                TextColor = Color.White
            };
            sessionIDBox.TextChanged += (sender, args) => {
                player.SessionId = sessionIDBox.Text;
                joinBtn.IsEnabled = true;
            };

            this.Content = new StackPanel {
                Width = 1100,
                Height = 250,
                BorderColor = Color.Yellow,
                BorderThickness = 2,
                Padding = 10,
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Centre,
                VerticalAlignment = VerticalAlignment.Centre,
                Items = {
                    CustomUIComponents.labeledTextBox(
                        "Session ID: ",
                        sessionIDBox
                    ),
                    new StackPanel {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(0, 50),
                        HorizontalAlignment = HorizontalAlignment.Centre,
                        Items = {
                            cancelBtn,
                            hostBtn,
                            joinBtn
                        }
                    }
                }
            };
        }

    }
}