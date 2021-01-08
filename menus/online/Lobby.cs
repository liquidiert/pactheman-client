using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using System.Linq;
using System;

namespace pactheman_client {
    class Lobby : Screen {

        public string Name = "Lobby";
        public string OpponentName = "";
        private Label lobbyState;
        private ListBox playerList;
        private Button readyBtn;

        public Lobby(string oppName) {
            this.OpponentName = oppName;
            this.Content = generateContent();
        }

        public void UpdateLobbyState(string text) {
            lobbyState.Content = text;
        }

        private Control generateContent() {
            // state
            lobbyState = new Label($"{OpponentName} joined");

            // player list -> who plays?
            playerList = new ListBox();
            playerList.Items.AddMany("Me", "AI");
            playerList.SelectedItem = "Me";
            playerList.SelectedIndexChanged += (sender, args) => {
                (Environment.Instance.Actors
                    .Where(a => a.Value.ID == "playerOne")
                        .First().Value as Player).IsHooman = (string)playerList.SelectedItem == "Me";
            };

            // ready btn
            readyBtn = new Button {
                BackgroundColor = Color.Transparent,
                Content = "Ready",
                Margin = new Thickness(0, 50)
            };
            readyBtn.Clicked += async (sender, args) => {
                await (Environment.Instance.Actors
                    .Where(a => a.Value.ID == "playerOne")
                        .First().Value as HumanPlayer).SetReady();
                /* TODO: transfer to start
                UIState.Instance.CurrentUIState = UIStates.Game;
                GameState.Instance.CurrentGameState = GameStates.Game;
                UIState.Instance.CurrentScreen = new InGameMenu(); */
            };

            return new StackPanel {
                Height = 500,
                Width = 1100,
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Centre,
                HorizontalAlignment = HorizontalAlignment.Centre,
                Items = {
                    new StackPanel {
                        Orientation = Orientation.Vertical,
                        Padding = 10,
                        Width = 500,
                        BackgroundColor = Color.Black,
                        BorderColor = Color.Yellow,
                        BorderThickness = 2,
                        Items = {
                            CustomUIComponents.labeledListBox(
                                "Choose your fighter: ",
                                playerList
                            ),
                            readyBtn
                        }
                    },
                    new StackPanel {
                        Orientation = Orientation.Vertical,
                        Padding = 10,
                        Width = 500,
                        BackgroundColor = Color.Black,
                        BorderColor = Color.Yellow,
                        BorderThickness = 2,
                        Items = {
                            lobbyState
                        }
                    }
                }
            };
        }
    }
}