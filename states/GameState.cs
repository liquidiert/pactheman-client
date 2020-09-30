using System;
using System.Collections.Generic;

namespace pactheman_client {

    public enum UIState {
        Menu,
        Settings,
        Game
    }

    class GameState {
        public UIState CurrentState { get; set; }
        public List<CollisionPair> CollisionPairs = new List<CollisionPair>();

        private static readonly Lazy<GameState> lazy = new Lazy<GameState>(() => new GameState());
        public static GameState Instance { get => lazy.Value; }
        private GameState() {}

    }
}