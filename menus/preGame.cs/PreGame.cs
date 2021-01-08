using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using System.Threading;

namespace pactheman_client {
    class PreGameMenu : Screen {

        public string Name = "PreGameMenu";
        
        public PreGameMenu() {

            // local btn
            var localBtn = new Button {
                        BackgroundColor = Color.Transparent,
                        Content = "Local",
                        Margin = new Thickness(0, 50)
                    };
            
            localBtn.Clicked += (sender, args) => {
                Environment.Instance.CurrentGameMode = GameModes.Local;
                UIState.Instance.CurrentUIState = UIStates.Game;
                GameState.Instance.CurrentGameState = GameStates.Game;
                UIState.Instance.GuiSystem.ActiveScreen.Hide();
                UIState.Instance.CurrentScreen = new InGameMenu();
            };

            // me vs ai btn
            var vsAIBtn = new Button {
                        BackgroundColor = Color.Transparent,
                        Content = "Me vs AI",
                        Margin = new Thickness(0, 50)
                    };
            vsAIBtn.Clicked += (sender, args) => {
                Environment.Instance.CurrentGameMode = GameModes.vsAI;
                // TODO: check if ai available; otherwise error out
                UIState.Instance.CurrentUIState = UIStates.Game;
                GameState.Instance.CurrentGameState = GameStates.Game;
                UIState.Instance.GuiSystem.ActiveScreen.Hide();
                UIState.Instance.CurrentScreen = new InGameMenu();
            };

            // online btn
            var onlineBtn = new Button {
                        BackgroundColor = Color.Transparent,
                        Content = "Online",
                        Margin = new Thickness(0, 50)
                    };
            onlineBtn.Clicked += async (sender, args) => {
                Environment.Instance.CurrentGameMode = GameModes.Online;
                var player = (HumanPlayer)Environment.Instance.Actors["player"];
                await player.Connect();
                UIState.Instance.CurrentUIState = UIStates.HostOrJoin;
                UIState.Instance.CurrentScreen = new HostOrJoinMenu();
            };

            this.Content = new StackPanel {
                Height = 500,
                Width = 350,
                Padding = new Thickness(50, 0),
                BackgroundColor = Color.Black,
                BorderColor = Color.Yellow,
                BorderThickness = 2,
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Centre,
                HorizontalAlignment = HorizontalAlignment.Centre,
                Items = {
                    localBtn,
                    vsAIBtn,
                    onlineBtn
                }
            };
        }
    }
}