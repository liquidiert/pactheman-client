using MonoGame.Extended.Gui;
using System;

namespace pactheman_client {
    public enum UIStates {
        Menu,
        MainMenu,
        Settings,
        Game
    }
    class UIState {

        public Screen PreviousScreen { get; set; }
        public Screen CurrentScreen {
            get {
                return GuiSystem.ActiveScreen;
            }
            set {
                PreviousScreen = CurrentScreen;
                GuiSystem.ActiveScreen = value;
            }
        }
        public GuiSystem GuiSystem { get; set; }
        public UIStates CurrentUIState { get; set; }

        private static readonly Lazy<UIState> lazy = new Lazy<UIState>(() => new UIState());
        public static UIState Instance { get => lazy.Value; }
        private UIState() {}
    }
}