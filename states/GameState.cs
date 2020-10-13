using System;
using System.Collections.Generic;

namespace pactheman_client {

    public enum UIState {
        Menu,
        MainMenu,
        Settings,
        Game,
        GameReset,
        GamePaused
    }

    class GameState {
        public UIState CurrentUIState { get; set; }
        public List<CollisionPair> CollisionPairs = new List<CollisionPair>();
        public float RESET_COUNTER = 4;

        private static readonly Lazy<GameState> lazy = new Lazy<GameState>(() => new GameState());
        public static GameState Instance { get => lazy.Value; }
        private GameState() {}

    }
}