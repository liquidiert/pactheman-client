using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Input;

namespace pactheman_client {
    public class Player : Actor {

        public bool IsHooman = true;
        protected int _score = 0;
        protected int _lives = 3;
        public int Score {
            get => _score;
        }
        public string Lives {
            get => "<3".Multiple(_lives);
        }
        protected MovingStates CurrentMovingState {
            get { return movingState; }
            set {
                if (movingState != value) {
                    movingState = value;
                    switch (movingState) {
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
        public Vector2 StatsPosition { get; set; }

        public Player(ContentManager content, string name, string spriteLocation) : base(content, spriteLocation) {
            this.Name = name;
            this.Position = Environment.Instance.PlayerStartPoints.Pop(new Random().Next(Environment.Instance.PlayerStartPoints.Count)).Position;
            this.StartPosition = Position;
            this.Sprite.Play(this.Position.X < 1120 ? "right" : "left");
        }

        public override void Move(GameTime t) {}
        protected Vector2 keyboardMove(float delta, Boolean useArrows = false) {
            var _kState = KeyboardExtended.GetState();
            // TODO: use rotation instead of dedicated animations
            Vector2 updatedPosition = Position;
            if (useArrows ? _kState.IsKeyDown(Keys.Up) : _kState.IsKeyDown(Keys.W)) { // up
                Velocity = new Vector2(0, -1);
                updatedPosition.Y -= MovementSpeed * delta;
                CurrentMovingState = MovingStates.Up;
            }
            if (useArrows ? _kState.IsKeyDown(Keys.Down) : _kState.IsKeyDown(Keys.S)) { // down
                Velocity = new Vector2(0, 1);
                updatedPosition.Y += MovementSpeed * delta;
                CurrentMovingState = MovingStates.Down;
            }
            if (useArrows ? _kState.IsKeyDown(Keys.Left) : _kState.IsKeyDown(Keys.A)) { // left
                Velocity = new Vector2(-1, 0);
                updatedPosition.X -= MovementSpeed * delta;
                CurrentMovingState = MovingStates.Left;
            }
            if (useArrows ? _kState.IsKeyDown(Keys.Right) : _kState.IsKeyDown(Keys.D)) { // right
                Velocity = new Vector2(1, 0);
                updatedPosition.X += MovementSpeed * delta;
                CurrentMovingState = MovingStates.Right;
            }
            return updatedPosition;
        }
        public override void Draw(SpriteBatch b){
            b.Draw(Sprite, Position);
        }
        public override void Reset() {
            Velocity = Vector2.Zero;
            Position = StartPosition;
        }
        public override void Clear() {
            _lives = 3;
            Position = Environment.Instance.PlayerStartPoints.Pop(new Random().Next(Environment.Instance.PlayerStartPoints.Count)).Position;;
            StartPosition = Position;
            Sprite.Play(this.Position.X < 1120 ? "right" : "left");
        }
        public override void OnCollision(CollisionInfo collisionInfo) {
            Position -= collisionInfo.PenetrationVector;
            base.OnCollision(collisionInfo);
        }

        public virtual void OnActorCollision(object sender, EventArgs args) {
            _lives--;
            if (_lives <= 0) {
                Environment.Instance.Clear();
                return;
            }
            Environment.Instance.Reset();
        }

    }
}