using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;

namespace pactheman_client {
    class Point : IActorTarget {

        public Texture2D Sprite;
        public Vector2 Velocity { get; set; }
        public Vector2 Position { get; set; }
        public RectangleF BoundingBox { get; set; }

        public Point(ContentManager content, Vector2 position) {
            this.Position = position;
            this.Sprite = content.Load<Texture2D>("sprites/objects/point");
            this.BoundingBox = new RectangleF(Position, new Size2(64f, 64f));
        }

        public void OnCollision(CollisionInfo collisionInfo) {
            Console.WriteLine(collisionInfo.Other.BoundingBox.Position == Environment.Instance.PacMan.Position.ToPoint());
        }

        public void Draw(SpriteBatch batch) {
            batch.Draw(Sprite, Position, color: Color.White);
        }
    }
}