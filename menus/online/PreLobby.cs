using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using System.Linq;

namespace pactheman_client {
    public class PreLobby : Screen {
        public PreLobby(string sessionID) {
            this.Content = new StackPanel {
                BorderColor = Color.Yellow,
                BorderThickness = 2,
                Padding = 10,
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Centre,
                VerticalAlignment = VerticalAlignment.Centre,
                Items = {
                    new Label("Invite another player!") { Margin = new Thickness(75, 10, 0, 25) },
                    new Label($"Your SessionID is: {sessionID}") { Margin = new Thickness(10, 0, 10, 25) }
                }
            };
        }
    }
}