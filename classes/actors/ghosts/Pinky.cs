using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;

namespace pactheman_client {

    class Pinky : Ghost {

        public Pinky(ContentManager content, string name) : base(content, "sprites/ghosts/spriteFactoryPinky.sf") {
            this.Sprite.Play("moving");
            this.Position = GameEnv.Instance.GhostStartPoints
                .Pop(new Random().Next(GameEnv.Instance.GhostStartPoints.Count)).Position.AddValue(32);
            this.StartPosition = Position;
            this.Name = name;
            this.lastTarget = StartPosition;
            this.scatterTarget = new Vector2(1, 1);
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