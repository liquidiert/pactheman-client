using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using System.Linq;

namespace pactheman_client {
    class Lobby : Screen {

        public string Name = "Lobby";
        public string OpponentName = "";
        public string LobbyState = "Waiting for other player...";
        public string SessionID = "";

        private ListBox playerList;
        private Button readyBtn;

        public Lobby(string sessionID) {
            this.SessionID = sessionID;
            this.Content = generateContent();
        }

        private Control generateContent() {
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
                UIState.Instance.CurrentUIState = UIStates.Game;
                GameState.Instance.CurrentGameState = GameStates.Game;
                UIState.Instance.CurrentScreen = new InGameMenu();
            };

            return new StackPanel {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Centre,
                HorizontalAlignment = HorizontalAlignment.Centre,
                Items = {
                    new StackPanel {
                        Height = 500,
                        Width = 1000,
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
                                BackgroundColor = Color.Black,
                                BorderColor = Color.Yellow,
                                BorderThickness = 2,
                                Items = {
                                    new Label(OpponentName),
                                    new Label(LobbyState)
                                }
                            }
                        }
                    },
                    new StackPanel {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Centre,
                        Items = {
                            new Label($"Your session ID is: {SessionID}")
                        }
                    }
                }
            };
        }
    }
}