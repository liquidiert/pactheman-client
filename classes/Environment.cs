using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.Collections.Concurrent;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Collisions;

namespace pactheman_client {

    enum GameModes {
        Local,
        vsAI,
        Online
    }

    sealed class GameEnv {

        public GameModes CurrentGameMode { get; set; }
        private TiledMapTileLayer _obstacles;
        public List<TiledMapObject> PlayerStartPoints;
        public List<TiledMapObject> GhostStartPoints;
        public List<ScorePoint> ScorePointPositions;
        public ConcurrentDictionary<String, Actor> Actors;
        public int[,] MapAsTiles;
        public CollisionWorld Walls;
        public List<CollisionPair> CollisionPairs = new List<CollisionPair>();
        public Dictionary<String, MoveInstruction> GhostMoveInstructions = new Dictionary<string, MoveInstruction>();
        private TiledMapObjectLayer _positionLayer;
        private TiledMapObjectLayer _pointLayer;
        private ContentManager _content;
        public Boolean IsOnline {
            get {
                return CurrentGameMode == GameModes.Online;
            }
        }
        public HumanPlayer PlayerOne {
            get => GameEnv.Instance.Actors["player"] as HumanPlayer;
        }
        public List<Ghost> Ghosts {
            get => GameEnv.Instance.Actors.Where(a => a.Key != "player" && a.Key != "opponent").Select(pair => pair.Value as Ghost).ToList();
        }

        public List<Player> Players {
            get => GameEnv.Instance.Actors.Where(a => a.Key == "player" || a.Key == "opponent").Select(pair => pair.Value as Player).ToList();
        }

        private static readonly Lazy<GameEnv> lazy = new Lazy<GameEnv>(() => new GameEnv());
        public static GameEnv Instance { get => lazy.Value; }
        private GameEnv() {
            var numProcs = System.Environment.ProcessorCount * 3;
            Actors = new ConcurrentDictionary<string, Actor>(numProcs, 6);
        }

        public GameEnv Init(ContentManager content, TiledMap map) {
            this._content = content;
            this._positionLayer = map.ObjectLayers.First(l => l.Name == "positions");
            this._pointLayer = map.ObjectLayers.First(l => l.Name == "points");

            // get start positions
            this.PlayerStartPoints = _positionLayer.Objects.Where(obj => obj.Type.ToString() == "player_start").ToList();
            this.GhostStartPoints = _positionLayer.Objects.Where(obj => obj.Type.ToString() == "ghost_start").ToList();

            // get point positions
            this.ScorePointPositions = _pointLayer.Objects.Select(p => new ScorePoint(content, p.Position)).ToList();

            // get obstacles
            this._obstacles = map.GetLayer<TiledMapTileLayer>("ground");

            // get tile map
            this.MapAsTiles = new int[this._obstacles.Width, this._obstacles.Height];
            for (var h = 0; h < this._obstacles.Height; h++) {
                for (var w = 0; w < this._obstacles.Width; w++) {
                    TiledMapTile? tile = null;
                    this._obstacles.TryGetTile((ushort)w, (ushort)h, out tile);
                    this.MapAsTiles[w, h] = tile.Value.GlobalIdentifier;
                }
            }

            // set wall collision world
            this.Walls = new CollisionWorld(Vector2.Zero);
            this.Walls.CreateGrid(_obstacles.Tiles.Select(tile => tile.GlobalIdentifier).ToArray(), 19, 22, 64, 64);

            return Instance;
        }
        public void InitMoveInstructions() {
            // TODO: read ghost moves from config file
            foreach (var ghost in Actors.Where(a => a.Key != "player" && a.Key != "opponent")) {
                Actor target = ghost.Value.Position.Distance(Actors["player"].Position) < ghost.Value.Position.Distance(Actors["opponent"].Position) ?
                    Actors["player"] : Actors["opponent"];
                this.GhostMoveInstructions.Add(ghost.Value.Name, new DirectAStarMove(ghost.Value, target));
            }
        }
        public bool RemoveScorePoint(Vector2 position) {
            return ScorePointPositions.RemoveWhere(p => p.Position.AddValue(32).EqualsWithTolerence(position, tolerance: 32f));
        }

        public void AddWallCollisions() {
            foreach (var actor in Actors.Values) {
                Walls.CreateActor(actor);
            }
        }
        /// <summary>
        /// Adds collsions for pairs of each Ghost and Player to GameState.
        /// </summary>
        /// <param name="actors">IMPORTANT: index 0 and 1 are reserved for player and opponent</param>
        public void AddActorCollisions() {

            var player = (HumanPlayer)Actors["player"];
            var opponent = (Opponent)Actors["opponent"];
            
            var playerOppPair = new CollisionPair(player, opponent);
            playerOppPair.Collision += player.OnActorCollision;
            var oppPlayerPair = new CollisionPair(opponent, player);
            oppPlayerPair.Collision += opponent.OnActorCollision;

            CollisionPairs.AddMany(
                playerOppPair,
                oppPlayerPair
            );

            foreach (var actor in Actors.Where(a => a.Key != "player" && a.Key != "opponent")) {
                // pair of player and ghost
                var playerPair = new CollisionPair(player, actor.Value);
                playerPair.Collision += player.OnActorCollision;

                // pair of opponent and ghost
                var opponentPair = new CollisionPair(opponent, actor.Value);
                opponentPair.Collision += opponent.OnActorCollision;

                CollisionPairs.AddMany(playerPair, opponentPair);
            }
        }
        public void Reset() {
            GameState.Instance.CurrentGameState = GameStates.GameReset;
            foreach (var actor in Actors.Values) {
                actor.Reset();
            }
        }

        public void Clear() {
            GameState.Instance.CurrentGameState = GameStates.MainMenu;
            UIState.Instance.CurrentUIState = UIStates.MainMenu;
            UIState.Instance.GuiSystem.ActiveScreen.Show();
            ScorePointPositions = _pointLayer.Objects.Select(p => new ScorePoint(_content, p.Position)).ToList();
            PlayerStartPoints = _positionLayer.Objects.Where(obj => obj.Type.ToString() == "player_start").ToList();
            GhostStartPoints = _positionLayer.Objects.Where(obj => obj.Type.ToString() == "ghost_start").ToList();
            foreach (var actor in Actors.Values) {
                actor.Clear();
            }
        }
    }
}