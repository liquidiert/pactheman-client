using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Collisions;

namespace pactheman_client {

    sealed class Environment {

        private TiledMapTileLayer _obstacles;
        public List<TiledMapObject> PlayerStartPoints;
        public List<TiledMapObject> GhostStartPoints;
        public List<ScorePoint> ScorePointPositions;
        public List<Actor> Actors;
        public int[,] MapAsTiles;
        public HumanPlayer PacMan;
        public CollisionWorld Walls;

        private static readonly Lazy<Environment> lazy = new Lazy<Environment>(() => new Environment());
        public static Environment Instance { get => lazy.Value; }
        private Environment() {}

        public Environment Init(ContentManager content, TiledMap map) {
            var positionLayer = map.ObjectLayers.First(l => l.Name == "positions");
            var pointLayer = map.ObjectLayers.First(l => l.Name == "points");

            // get start positions
            this.PlayerStartPoints = positionLayer.Objects.Where(obj => obj.Type.ToString() == "player_start").ToList();
            this.GhostStartPoints = positionLayer.Objects.Where(obj => obj.Type.ToString() == "ghost_start").ToList();

            // get point positions
            this.ScorePointPositions = pointLayer.Objects.Select(p => new ScorePoint(content, p.Position)).ToList();

            // get obstacles
            this._obstacles = map.GetLayer<TiledMapTileLayer>("ground");

            // get tile map
            this.MapAsTiles = new int[this._obstacles.Width + 1, this._obstacles.Height + 1];
            for (var h = 0; h < this._obstacles.Height; h++) {
                for (var w = 0; w < this._obstacles.Width; w++) {
                    TiledMapTile? tile = null;
                    this._obstacles.TryGetTile((ushort) w, (ushort) h, out tile);
                    this.MapAsTiles[w, h] = tile.Value.GlobalIdentifier;
                }
            }

            // set wall collision world
            Walls = new CollisionWorld(Vector2.Zero);
            Walls.CreateGrid(_obstacles.Tiles.Select(tile => tile.GlobalIdentifier).ToArray(), 19, 22, 64, 64);

            return Instance;
        }
        public bool RemoveScorePoint(Vector2 position) {
            return ScorePointPositions.RemoveWhere(p => p.Position.AddValue(32).EqualsWithTolerence(position, tolerance: 32f));
        }
        /// <summary>
        /// Adds collsions for walls and also for pairs of each Ghost and Player to GameState.
        /// </summary>
        /// <param name="actors">IMPORTANT: index 0 and 1 are reserved for player and opponent</param>
        public void AddCollisions() {
            foreach (var actor in Actors) {
                Walls.CreateActor(actor);
            }
            var actorsCopy = new List<Actor>(Actors);
            var player = (HumanPlayer) actorsCopy.Pop(0);
            // TODO: add opponent var opponent = actors.Pop(0);
            foreach (var actor in actorsCopy) {
                var pair = new CollisionPair(player, actor);
                pair.Collision += player.OnActorCollision;
                GameState.Instance.CollisionPairs.Add(pair);
            }
        }
        public void Reset() {
            GameState.Instance.CurrentUIState = UIState.GameReset;
            foreach (var actor in Actors) {
                actor.Reset();
            }
        }
    }
}