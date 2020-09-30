using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Collisions;

namespace pactheman_client {

    enum MovingStates {
        Up,
        Down,
        Left,
        Right
    }

    abstract class Actor : IActorTarget {

        public Actor(ContentManager content, string spriteSheetLocation) {
            // HACK: MonoGame.Extended somehow can't read xnb files; thus always be sure the file is present in build dir!
            var spriteSheet = content.Load<SpriteSheet>(spriteSheetLocation, new JsonContentLoader());
            Sprite = new AnimatedSprite(spriteSheet);
            BoundingBox = Sprite.GetBoundingRectangle(new Transform2());
            // offset bounding box a little up and left
            BoundingOffset = new Vector2(-32, -32);
        }

        public AnimatedSprite Sprite;
        public float MovementSpeed = 350f;
        public RectangleF BoundingBox { get; set; }
        public Vector2 BoundingOffset { get; set; }

        private Vector2 _position;
        public Vector2 Position {
            get => _position;
            set { 
                _position = value;
                var t = BoundingBox;
                t.Position = value + BoundingOffset;
                BoundingBox = t;
            }
        }
        public Vector2 Velocity { get; set; }
        /// <summary>
        /// Position scaled down by tile size
        /// </summary>
        /// <returns>
        /// scaled position as [Vector2]
        /// </returns>
        public Vector2 DownScaledPosition {
            get { return new Vector2((float) Math.Floor(Position.X / 64), (float) Math.Round(Position.Y / 64)); }
        }
        public Vector2 UpScaledPosition {
            get { return new Vector2((float) Math.Ceiling(Position.X / 64), (float) Math.Ceiling(Position.Y / 64)); }
        }

        protected MovingStates movingState;

        protected Vector2 UpdatePosition(float x = 0, int xFactor = 1, float y = 0, int yFactor = 1) {
            return new Vector2() { X = (this.Position.X + x) * xFactor, Y = (this.Position.Y + y) * yFactor};
        }
        public abstract void Move(GameTime t);
        public abstract void Draw(SpriteBatch b);
        public virtual void OnCollision(CollisionInfo collisionInfo) {}
        public dynamic Describe() {
            return new {pos = Position, speed = MovementSpeed};
        }

        public new string ToString() {
            return $"posX: {(ushort) Position.X} posY: {(ushort) Position.Y}\nvelocity: {Velocity}\n";
        }
    }
}