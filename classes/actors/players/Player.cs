using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Sprites;

namespace pactheman_client {
    public class Player : Actor {
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

        public void OnActorCollision(object sender, EventArgs args) {
            _lives--;
            if (_lives <= 0) {
                Environment.Instance.Clear();
                return;
            }
            Environment.Instance.Reset();
        }

    }
}