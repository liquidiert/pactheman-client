using System.Linq;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using MonoGame.Extended.Tiled;

namespace pactheman_client {

    class Environment {

        private TiledMapTileLayer _obstacles;
        public List<TiledMapObject> PlayerStartPoints;
        public List<TiledMapObject> GhostStartPoints;

        public Environment(TiledMap map) {

            var positionLayer = map.ObjectLayers.First(l => l.Name == "positions");

            // get start positions
            this.PlayerStartPoints = positionLayer.Objects.Where(obj => obj.Type.ToString() == "player_start").ToList();
            this.GhostStartPoints = positionLayer.Objects.Where(obj => obj.Type.ToString() == "ghost_start").ToList();

            // get obstacles
            this._obstacles = map.GetLayer<TiledMapTileLayer>("ground");

        }

        public bool InsideWall(Actor actor, float delta) {
            var futurePos = new Vector2() { 
                X = (actor.Position.X + actor.MovementSpeed * delta * actor.DirectionX) / 64 + actor.DirectionX * 0.5f,
                Y = (actor.Position.Y + actor.MovementSpeed * delta * actor.DirectionY) / 64 + actor.DirectionY * 0.5f
            };
            TiledMapTile? tile = null;
            Func<double, bool> roundCondition = (double toRound) => {
                double toTest = toRound % 1;
                return toTest >= 0.95 || toTest <= 0.05;
            };
            _obstacles.TryGetTile(
                // only round if "close" to edge
                (ushort) MathExtension.RoundIf(futurePos.X, roundCondition),
                (ushort) MathExtension.RoundIf(futurePos.Y, roundCondition),
                out tile
            );
            return tile.Value.GlobalIdentifier != 6;
        }
    }
}