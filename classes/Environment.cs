using System.Linq;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using MonoGame.Extended.Tiled;

namespace pactheman_client {

    sealed class Environment {

        private TiledMapTileLayer _obstacles;
        public List<TiledMapObject> PlayerStartPoints;
        public List<TiledMapObject> GhostStartPoints;
        public int[,] MapAsTiles;
        public Actor PacMan;

        private static readonly Lazy<Environment> lazy = new Lazy<Environment>(() => new Environment());
        public static Environment Instance { get { return lazy.Value; } }
        private Environment() {}

        public Environment Init(TiledMap map) {
            var positionLayer = map.ObjectLayers.First(l => l.Name == "positions");

            // get start positions
            this.PlayerStartPoints = positionLayer.Objects.Where(obj => obj.Type.ToString() == "player_start").ToList();
            this.GhostStartPoints = positionLayer.Objects.Where(obj => obj.Type.ToString() == "ghost_start").ToList();

            // get obstacles
            this._obstacles = map.GetLayer<TiledMapTileLayer>("ground");

            // set tile map
            this.MapAsTiles = new int[this._obstacles.Width + 1, this._obstacles.Height + 1];
            for (var h = 0; h < this._obstacles.Height; h++) {
                for (var w = 0; w < this._obstacles.Width; w++) {
                    TiledMapTile? tile = null;
                    this._obstacles.TryGetTile((ushort) w, (ushort) h, out tile);
                    this.MapAsTiles[w, h] = tile.Value.GlobalIdentifier;
                }
            }

            return Instance;
        }

        public bool InsideWall(Actor actor, Vector2 toCheck) {
            TiledMapTile? tile = null;
            Func<double, bool> roundCondition = (double toRound) => {
                double toTest = toRound % 1;
                return toTest >= 0.95 || toTest <= 0.05;
            };
            _obstacles.TryGetTile(
                // only round if "close" to edge
                (ushort) MathExtension.RoundIf(toCheck.X / 64 + actor.DirectionX * 0.5, roundCondition),
                (ushort) MathExtension.RoundIf(toCheck.Y / 64 + actor.DirectionY * 0.5, roundCondition),
                out tile
            );
            return tile.Value.GlobalIdentifier != 6;
        }
    }
}