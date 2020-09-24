using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;

namespace pactheman_client {

    enum MovingStates {
        Up,
        Down,
        Left,
        Right
    }

    abstract class Player {

        public AnimatedSprite Sprite;
        public Vector2 Position { get; set; }
        public float MovementSpeed { get; set; }

        public Transform2 transform;
        public RectangleF BoundingBox => Sprite.GetBoundingRectangle(transform.Position, transform.Rotation, transform.Scale);

        MovingStates movingState;
        // once updated animation will play
        public MovingStates CurrentMovingState { 
            get { return movingState; } 
            protected set {
                if (movingState != value) {
                    movingState = value;
                    switch(movingState) {
                        case MovingStates.Up:
                            Sprite.Play("up");
                            break;
                        case MovingStates.Down:
                            Sprite.Play("down");
                            break;
                        case MovingStates.Left:
                            Sprite.Play("left");
                            break;
                        case MovingStates.Right:
                            Sprite.Play("right");
                            break;
                    }
                }
            }
        }
        public abstract void Move(GameTime t);

        public void Describe() {
            Console.WriteLine("------------------------------");
            Console.WriteLine($"pos: {this.Position}\nspeed: {this.MovementSpeed}");
            Console.WriteLine("------------------------------");
        }
    }
}