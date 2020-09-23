namespace pactheman_client {

    public enum UIState {
        Menu,
        Settings,
        Game
    }

    class GameState {
        public static UIState CurrentState { get; set; }

    }
}