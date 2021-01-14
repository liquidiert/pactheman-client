using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using System.Collections.Generic;
using System.Linq;
using System;

namespace pactheman_client {
    class Settings : Screen {

        public string Name = "Settings";

        public Settings() {

            var saveBtn = new Button {
                Content = "Save",
                BackgroundColor = Color.Transparent,
                Width = 170
            };
            saveBtn.Clicked += async (sender, args) => {
                UIState.Instance.CurrentUIState = UIStates.MainMenu;
                UIState.Instance.CurrentScreen = UIState.Instance.PreviousScreen;
                await ConfigReader.Instance.Save();
            };

            var cancelBtn = new Button {
                Content = "Cancel",
                Margin = new Thickness(10, 0),
                BackgroundColor = Color.Transparent,
                Width = 200
            };
            cancelBtn.Clicked += async (sender, args) => {
                UIState.Instance.CurrentUIState = UIStates.MainMenu;
                UIState.Instance.CurrentScreen = UIState.Instance.PreviousScreen;
                await ConfigReader.Instance.Reset();
            };

            // server settings
            var serverIP = new TextBox(text: ConfigReader.Instance.config["general"]["server_ip"]) {
                BackgroundColor = Color.Transparent,
                TextColor = Color.White,
            };
            serverIP.TextChanged += (sender, args) => {
                ConfigReader.Instance.config["general"]["server_ip"] = serverIP.Text;
            };
            var serverPort = new TextBox(text: ConfigReader.Instance.config["general"]["server_port"]) {
                BackgroundColor = Color.Transparent,
                TextColor = Color.White,
            };
            serverPort.TextChanged += (sender, args) => {
                ConfigReader.Instance.config["general"]["server_port"] = serverPort.Text;
            };

            // ghost settings
            List<string> possibleBehaviors = new List<string>();
            foreach (var behavior in ConfigReader.Instance.config["ghosts"]["move_behaviors"]) {
                possibleBehaviors.Add(MoveInstruction.HumanReadableMoveInstructions[behavior]);
            }
            var behaviors = possibleBehaviors.ToArray();
            // blinky
            var blinkyMoves = new ListBox();
            blinkyMoves.Items.AddMany((object[])behaviors);
            blinkyMoves.SelectedItem = MoveInstruction.HumanReadableMoveInstructions[ConfigReader.Instance.config["ghosts"]["blinky"]["move_behavior"]["current"]];
            blinkyMoves.SelectedIndexChanged += (sender, args) => {
                ConfigReader.Instance.config["ghosts"]["blinky"]["move_behavior"]["current"] =
                    MoveInstruction.HumanReadableMoveInstructions.FirstOrDefault(x => x.Value == (string)blinkyMoves.SelectedItem).Key;
            };

            // clyde
            var clydeMoves = new ListBox();
            clydeMoves.Items.AddMany((object[])behaviors);
            clydeMoves.SelectedItem = MoveInstruction.HumanReadableMoveInstructions[ConfigReader.Instance.config["ghosts"]["clyde"]["move_behavior"]["current"]];
            clydeMoves.SelectedIndexChanged += (sender, args) => {
                ConfigReader.Instance.config["ghosts"]["clyde"]["move_behavior"]["current"] =
                    MoveInstruction.HumanReadableMoveInstructions.FirstOrDefault(x => x.Value == (string)clydeMoves.SelectedItem).Key;
            };

            // inky
            var inkyMoves = new ListBox();
            inkyMoves.Items.AddMany((object[])behaviors);
            inkyMoves.SelectedItem = MoveInstruction.HumanReadableMoveInstructions[ConfigReader.Instance.config["ghosts"]["inky"]["move_behavior"]["current"]];
            inkyMoves.SelectedIndexChanged += (sender, args) => {
                ConfigReader.Instance.config["ghosts"]["inky"]["move_behavior"]["current"] =
                    MoveInstruction.HumanReadableMoveInstructions.FirstOrDefault(x => x.Value == (string)inkyMoves.SelectedItem).Key;
            };

            // pinky
            var pinkyMoves = new ListBox();
            pinkyMoves.Items.AddMany((object[])behaviors);
            pinkyMoves.SelectedItem = MoveInstruction.HumanReadableMoveInstructions[ConfigReader.Instance.config["ghosts"]["pinky"]["move_behavior"]["current"]];
            pinkyMoves.SelectedIndexChanged += (sender, args) => {
                ConfigReader.Instance.config["ghosts"]["pinky"]["move_behavior"]["current"] =
                    MoveInstruction.HumanReadableMoveInstructions.FirstOrDefault(x => x.Value == (string)pinkyMoves.SelectedItem).Key;
            };

            this.Content = new StackPanel {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Centre,
                Items = {
                    new StackPanel {
                        Orientation = Orientation.Horizontal,
                        Items = {
                            CustomUIComponents.borderedSection(
                                "Server settings",
                                new Control[]{
                                    CustomUIComponents.labeledTextBox("Server IP:", serverIP),
                                    CustomUIComponents.labeledTextBox("Server Port:", serverPort)
                                },
                                margin: new Thickness(0, 0, 20, 50)),
                            CustomUIComponents.borderedSection(
                                "Ghost settings",
                                new Control[] {
                                    new StackPanel {
                                        Orientation = Orientation.Horizontal,
                                        Items = {
                                            new StackPanel {
                                                Orientation = Orientation.Vertical,
                                                Margin = new Thickness(0, 0, 10, 0),
                                                Items = {
                                                    CustomUIComponents.labeledTextBox(
                                                        "Blinky Movement:",
                                                        blinkyMoves,
                                                        labelPaddingBottom: 220
                                                    ),
                                                    CustomUIComponents.labeledTextBox(
                                                        "Clyde Movement:",
                                                        clydeMoves,
                                                        labelPaddingBottom: 220
                                                    )
                                                }
                                            },
                                            new StackPanel {
                                                Orientation = Orientation.Vertical,
                                                Items = {
                                                    CustomUIComponents.labeledTextBox(
                                                        "Inky Movement:",
                                                        inkyMoves,
                                                        labelPaddingBottom: 220
                                                    ),
                                                    CustomUIComponents.labeledTextBox(
                                                        "Pinky Movement:",
                                                        pinkyMoves,
                                                        labelPaddingBottom: 220
                                                    )
                                                }
                                            }
                                        }
                                    }
                                }
                            )
                        }
                    },
                    new StackPanel {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Centre,
                        Margin = new Thickness(0, 10),
                        Items = {
                            cancelBtn,
                            saveBtn
                        }
                    }
                }
            };
        }

    }
}