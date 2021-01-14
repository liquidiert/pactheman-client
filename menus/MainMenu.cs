using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using System;

namespace pactheman_client {
    public class MainMenu : Screen {

        public string Name = "MainMenu";
        
        public MainMenu(Action exitGameAction) {

            // game start btn
            var gameStartBtn = new Button {
                        BackgroundColor = Color.Transparent,
                        Content = "Game Start",
                        Margin = new Thickness(0, 50)
                    };
            
            gameStartBtn.Clicked += (sender, args) => {
                UIState.Instance.CurrentUIState = UIStates.PreGame;
                UIState.Instance.CurrentScreen = new PreGameMenu();
            };

            // settings btn
            var settingsBtn = new Button {
                        Content = "Settings",
                        BackgroundColor = Color.Transparent,
                        Margin = new Thickness(0, 50)
                    };
            settingsBtn.Clicked += (sender, args) => {
                UIState.Instance.CurrentUIState = UIStates.Settings;
                UIState.Instance.CurrentScreen = new Settings();
            };

            // exit btn
            var exitBtn = new Button {
                        BackgroundColor = Color.Transparent,
                        Content = "Exit",
                        Margin = new Thickness(0, 50)
                    };
            exitBtn.Clicked += (sender, args) => exitGameAction();

            this.Content = new StackPanel {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Centre,
                HorizontalAlignment = HorizontalAlignment.Centre,
                Items = {
                    new Label("Pac-The-Man") {
                        BackgroundColor = Color.Transparent,
                        TextColor = new Color(255, 211, 0),
                        Margin = new Thickness(0, 0, 0, 50)
                    },
                    gameStartBtn,
                    // TODO: investigate why back btn in settings is different
                    settingsBtn,
                    exitBtn,
                }
            };
        }
    }
}