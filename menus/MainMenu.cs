using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;

namespace pactheman_client {
    class MainMenu : Screen {

        public string Name = "MainMenu";
        
        public MainMenu() {
            this.Content = new StackPanel {
                Orientation = Orientation.Vertical,
                Items = {
                    new Button {
                        Name = "GameStartBtn",
                        Content = "Game Start",
                        HorizontalAlignment = HorizontalAlignment.Centre,
                        VerticalAlignment = VerticalAlignment.Top
                    },
                    new Button {
                        Name = "SettingsBtn",
                        Content = "Settings",
                        HorizontalAlignment = HorizontalAlignment.Centre,
                        VerticalAlignment = VerticalAlignment.Top
                    },
                    new Button {
                        Name = "ExitBtn",
                        Content = "Exit",
                        HorizontalAlignment = HorizontalAlignment.Centre,
                        VerticalAlignment = VerticalAlignment.Top
                    },

                }
            };
        }
    }
}