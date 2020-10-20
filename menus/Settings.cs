using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;

namespace pactheman_client {
    class Settings : Screen {

        public string Name = "Settings";
        
        public Settings() {

            var backBtn = new Button {
                Content = "Back",
                BackgroundColor = Color.Transparent,
            };
            backBtn.Clicked += (sender, args) => {
                UIState.Instance.CurrentUIState = UIStates.MainMenu;
                UIState.Instance.CurrentScreen = UIState.Instance.PreviousScreen;
            };

            this.Content = new StackPanel {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Centre,
                HorizontalAlignment = HorizontalAlignment.Centre,
                Items = {
                    new Label("None available yet :(") {
                        BackgroundColor = Color.Transparent,
                        TextColor = new Color(255, 211, 0),
                        Margin = new Thickness(0, 0, 0, 50)
                    },
                    backBtn
                }
            };
        }
    }
}