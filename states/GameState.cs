using System;
using System.Collections.Generic;

namespace pactheman_client {

    public enum GameStates {
        MainMenu,
        Game,
        GameReset,
        GamePaused
    }

    class GameState {
        public GameStates CurrentGameState { get; set; }
        public float RESET_COUNTER = 4;

        private static readonly Lazy<GameState> lazy = new Lazy<GameState>(() => new GameState());
        public static GameState Instance { get => lazy.Value; }
        private GameState() {}

    }
}