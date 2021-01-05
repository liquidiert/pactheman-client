using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using System;

namespace pactheman_client {
    class Lobby : Screen {

        public string Name = "Lobby";
        
        public Lobby() {

            // player list -> who plays?
            var playerList = new ListBox();
            playerList.Items.AddMany("Me", "AI");

            // ready btn
            var readyBtn = new Button {
                        BackgroundColor = Color.Transparent,
                        Content = "Ready",
                        Margin = new Thickness(0, 50)
                    };
            
            readyBtn.Clicked += (sender, args) => {
                // TODO: tell server handler I'm rdy
                UIState.Instance.CurrentUIState = UIStates.Game;
                GameState.Instance.CurrentGameState = GameStates.Game;
                UIState.Instance.CurrentScreen = new InGameMenu();
            };

            this.Content = new StackPanel {
                Height = 500,
                Width = 700,
                BackgroundColor = Color.Black,
                BorderColor = Color.Yellow,
                BorderThickness = 2,
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Centre,
                HorizontalAlignment = HorizontalAlignment.Centre,
                Items = {
                    playerList,
                    readyBtn
                }
            };
        }
    }
}