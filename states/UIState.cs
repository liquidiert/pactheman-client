using MonoGame.Extended.Gui;
using System;

namespace pactheman_client {
    public enum UIStates {
        Menu,
        MainMenu,
        Settings,
        Game
    }
    public class StateEvent : EventArgs {
        public UIStates CurrentState { get; set; }

        public StateEvent(UIStates currentState) => CurrentState = currentState;
    }
    public class UIState {

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
        private UIStates _currentUIState { get; set; }
        public UIStates CurrentUIState { 
            get => _currentUIState; 
            set {
                _currentUIState = value;
                if (StateChanged != null) {
                    StateChanged.Invoke(this, new StateEvent(_currentUIState));
                }
            }
        }
        public event EventHandler<StateEvent> StateChanged; // IDEA: do more with that?

        private static readonly Lazy<UIState> lazy = new Lazy<UIState>(() => new UIState());
        public static UIState Instance { get => lazy.Value; }
        private UIState() {}
    }
}