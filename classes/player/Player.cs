using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace pactheman_client {

    enum MovingStates {
        Up,
        Down,
        Left,
        Right,
        Idle,
    }

    abstract class Player {

        AnimatedSprite Sprite;
        Vector2 Position { get; set; }
        float DirectionX { get; set; }
        float DirectionY { get; set; }
        float MovementSpeed { get; set; }

        Transform2 transform;
        RectangleF BoundingBox => Sprite.GetBoundingRectangle(transform.Position, transform.Rotation, transform.Scale);

        MovingStates movingState;
        // once updated animation will play
        public MovingStates CurrentMovingState { get; 
            protected set {
                if (state != value) {
                    state = value;
                    switch(state) {
                        // TODO: add sprite animations
                        case MovingStates.Up:
                            sprite.Play("WalkUp", () => State = PlayerStates.Idle);
                            break;
                        case MovingStates.Down:
                            sprite.Play("WalkDown", () => State = PlayerStates.Idle);
                            break;
                        case MovingStates.Left:
                            sprite.Play("WalkLeft", () => State = PlayerStates.Idle);
                            break;
                        case MovingStates.Right:
                            sprite.Play("WalkRight", () => State = PlayerStates.Idle);
                            break;
                        case MovingStates.Idle:
                            Sprite.Play("Idle");
                            break;
                    }
                }
            }
        }
        public abstract void Move();

        public void Describe() {
            Console.WriteLine("------------------------------");
            Console.WriteLine($"pos: {this.Position}\ndirectionX: {this.DirectionX}, directionY: {this.DirectionY}\nspeed: {this.MovementSpeed}");
            Console.WriteLine("------------------------------");
        }
    }
}