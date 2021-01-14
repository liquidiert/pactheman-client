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
                GameEnv.Instance.PlayerOne.IsHooman = (string)playerList.SelectedItem == "Me";
            };

            // ready btn
            readyBtn = new Button {
                BackgroundColor = Color.Transparent,
                Content = "Ready",
                Margin = new Thickness(0, 50)
            };
            readyBtn.Clicked += async (sender, args) => {
                await GameEnv.Instance.PlayerOne.SetReady();
                playerList.IsEnabled = false;
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