using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Input;
using PacTheMan.Models;

namespace pactheman_client {
    public class Player : Actor {

        public bool IsHooman = true;
        public int Score = 0;
        protected int _lives = 3;
        public string Lives {
            get => "<3".Multiple(_lives);
        }
        public MovingState CurrentMovingState {
            get { return movingState; }
            set {
                if (movingState != value) {
                    movingState = value;
                    switch (movingState) {
                        case MovingState.Up:
                            Sprite.Play("up");
                            break;
                        case MovingState.Down:
                            Sprite.Play("down");
                            break;
                        case MovingState.Left:
                            Sprite.Play("left");
                            break;
                        case MovingState.Right:
                            Sprite.Play("right");
                            break;
                    }
                }
            }
        }
        public Vector2 StatsPosition { get; set; }

        public Player(ContentManager content, string name, string spriteLocation) : base(content, spriteLocation) {
            this.Name = name;
            this.Position = GameEnv.Instance.PlayerStartPoints.Pop(new Random().Next(GameEnv.Instance.PlayerStartPoints.Count)).Position;
            this.StartPosition = Position;
            this.Sprite.Play(this.Position.X < 1120 ? "right" : "left");
        }

        public override void Move(GameTime t) { }
        protected Vector2 keyboardMove(float delta, Boolean useArrows = false) {
            var _kState = KeyboardExtended.GetState();
            // TODO: use rotation instead of dedicated animations
            Vector2 updatedPosition = Position;
            if (useArrows ? _kState.IsKeyDown(Keys.Up) : _kState.IsKeyDown(Keys.W)) { // up
                Velocity = new Vector2(0, -MovementSpeed * delta);
                updatedPosition += Velocity;
                CurrentMovingState = MovingState.Up;
            }
            if (useArrows ? _kState.IsKeyDown(Keys.Down) : _kState.IsKeyDown(Keys.S)) { // down
                Velocity = new Vector2(0, MovementSpeed * delta);
                updatedPosition += Velocity;
                CurrentMovingState = MovingState.Down;
            }
            if (useArrows ? _kState.IsKeyDown(Keys.Left) : _kState.IsKeyDown(Keys.A)) { // left
                Velocity = new Vector2(-MovementSpeed * delta, 0);
                updatedPosition += Velocity;
                CurrentMovingState = MovingState.Left;
            }
            if (useArrows ? _kState.IsKeyDown(Keys.Right) : _kState.IsKeyDown(Keys.D)) { // right
                Velocity = new Vector2(MovementSpeed * delta, 0);
                updatedPosition += Velocity;
                CurrentMovingState = MovingState.Right;
            }
            Velocity = Vector2.Zero;
            return updatedPosition;
        }
        public override void Draw(SpriteBatch b) {
            b.Draw(Sprite, Position);
        }
        public override void Reset() {
            Velocity = Vector2.Zero;
            Position = StartPosition;
        }
        public override void Clear() {
            _lives = 3;
            Position = GameEnv.Instance.PlayerStartPoints.Pop(new Random().Next(GameEnv.Instance.PlayerStartPoints.Count)).Position;
            StartPosition = Position;
            Sprite.Play(this.Position.X < 1120 ? "right" : "left");
        }
        public override void OnCollision(CollisionInfo collisionInfo) {
            Position -= collisionInfo.PenetrationVector;
            base.OnCollision(collisionInfo);
        }

        public virtual void OnActorCollision(object sender, CollisionPairEvent args) {
            if (!GameEnv.Instance.IsOnline) {
                _lives--;
                if (_lives <= 0) {
                    GameEnv.Instance.Clear();
                    return;
                }
                GameEnv.Instance.Reset();
            }
        }

        public void SetLives(int lives) {
            _lives = lives;
        }

    }
}