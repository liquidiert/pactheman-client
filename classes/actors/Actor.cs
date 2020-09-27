using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;

namespace pactheman_client {

    enum MovingStates {
        Up,
        Down,
        Left,
        Right
    }

    abstract class Actor {

        public AnimatedSprite Sprite;
        public float MovementSpeed = 350f;
        public short DirectionX { get; set; }
        public short DirectionY { get; set; }

        protected Transform2 _transform = new Transform2();
        public RectangleF BoundingBox => Sprite.GetBoundingRectangle(_transform.Position, _transform.Rotation, _transform.Scale);
        public Vector2 Position {
            get { return _transform.Position; }
            set { _transform.Position = value; }
        }

        protected Environment _environment;

        protected MovingStates movingState;

        protected Vector2 UpdatePosition(float x = 0, int xFactor = 1, float y = 0, int yFactor = 1) {
            return new Vector2() { X = (this.Position.X + x) * xFactor, Y = (this.Position.Y + y) * yFactor};
        }
        public abstract void Move(GameTime t, GraphicsDeviceManager g);
        public abstract void Draw(SpriteBatch b);
        public dynamic Describe() {
            return new {pos = _transform.Position, speed = MovementSpeed};
        }

        public new string ToString() {
            return $"posX: {(ushort) this._transform.Position.X} posY: {(ushort) this._transform.Position.Y}\nspeed: {this.MovementSpeed}";
        }
    }
}