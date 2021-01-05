using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using System;

namespace pactheman_client {
    class InGameMenu : Screen {

        public string Name = "InGameMenu";
        
        public InGameMenu() {

            // continue btn
            var continueBtn = new Button {
                        BackgroundColor = Color.Transparent,
                        Content = "Continue",
                        Margin = new Thickness(0, 50, 0, 25)
                    };
            
            continueBtn.Clicked += (sender, args) => {
                UIState.Instance.CurrentUIState = UIStates.Game;
                GameState.Instance.CurrentGameState = GameStates.Game;
                UIState.Instance.GuiSystem.ActiveScreen.Hide();
            };

            // exit btn
            var exitBtn = new Button {
                        BackgroundColor = Color.Transparent,
                        Content = "Exit",
                        Margin = new Thickness(0, 25)
                    };
            exitBtn.Clicked += (sender, args) => {
                Environment.Instance.Clear();
                // TODO: check mode and if connected to server disconnect gracefully
                UIState.Instance.CurrentUIState = UIStates.MainMenu;
                GameState.Instance.CurrentGameState = GameStates.MainMenu;
                UIState.Instance.CurrentScreen = UIState.Instance.MainMenu;
            };

            this.Content = new StackPanel {
                Height = 300,
                Width = 300,
                Padding = new Thickness(25, 0),
                BackgroundColor = Color.Black,
                BorderColor = Color.Yellow,
                BorderThickness = 2,
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Centre,
                HorizontalAlignment = HorizontalAlignment.Centre,
                Items = {
                    continueBtn,
                    exitBtn,
                }
            };
        }
    }
}