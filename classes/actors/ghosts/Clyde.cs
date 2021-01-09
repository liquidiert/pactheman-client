using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Collisions;
using System;
using System.Linq;
using System.Collections.Generic;

namespace pactheman_client {

    class Clyde : Ghost {

        public Clyde(ContentManager content, string name) : base(content, "sprites/ghosts/spriteFactoryClyde.sf") {
            this.Sprite.Play("moving");
            this.Position = Environment.Instance.GhostStartPoints
                .Pop(new Random().Next(Environment.Instance.GhostStartPoints.Count)).Position.AddValue(32);
            this.StartPosition = Position;
            this.Name = name;
            this.lastTarget = StartPosition;
            this.scatterTarget = new Vector2(1, 21);
        }

        public override void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(Sprite, Position);
        }

        public override void OnCollision(CollisionInfo collisionInfo) {
            Position -= collisionInfo.PenetrationVector;
            base.OnCollision(collisionInfo);
        }
    }
}