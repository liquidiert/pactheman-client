using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tiled;
using System;
using MonoGame.Extended.Collisions;

namespace pactheman_client {
    class Opponent : Actor {

        private KeyboardStateExtended _kState;
        private int _score = 0;
        private bool _isHooman = true;
        private int _lives = 3;
        public int Score {
            get => _score;
        }
        public string Lives {
            get => "<3".Multiple(_lives);
        }
        public string Name = "PlayerTwo"; // TODO: change at game start

        private MovingStates CurrentMovingState {
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

        public Opponent(ContentManager content) : base(content, "sprites/opponent/spriteFactory.sf") {
            this.Position = Environment.Instance.PlayerStartPoints.Pop(new Random().Next(Environment.Instance.PlayerStartPoints.Count)).Position;
            this.StartPosition = Position;
            this.Sprite.Play(this.Position.X < 1120 ? "right" : "left");
        }

        public override void Move(GameTime gameTime) {
            // TODO: add real ai movement if _isHooman is false
            var delta = gameTime.GetElapsedSeconds();
            _kState = KeyboardExtended.GetState();

            // TODO: use rotation instead of dedicated animations
            Vector2 updatedPosition = Position;
            if (_kState.IsKeyDown(Keys.Up)) { // up
                Velocity = new Vector2(0, -1);
                updatedPosition.Y -= MovementSpeed * delta;
                CurrentMovingState = MovingStates.Up;
            }
            if (_kState.IsKeyDown(Keys.Down)) { // down
                Velocity = new Vector2(0, 1);
                updatedPosition.Y += MovementSpeed * delta;
                CurrentMovingState = MovingStates.Down;
            }
            if (_kState.IsKeyDown(Keys.Left)) { // left
                Velocity = new Vector2(-1, 0);
                updatedPosition.X -= MovementSpeed * delta;
                CurrentMovingState = MovingStates.Left;
            }
            if (_kState.IsKeyDown(Keys.Right)) { // right
                Velocity = new Vector2(1, 0);
                updatedPosition.X += MovementSpeed * delta;
                CurrentMovingState = MovingStates.Right;
            }

            // teleport if entering either left or right gate
            if (updatedPosition.X <= 70 || updatedPosition.X >= 1145) {
                Position = UpdatePosition(x: -1216, xFactor: -1);
            } else {
                Position = updatedPosition;
            }

            if (Environment.Instance.RemoveScorePoint(Position)) {
                _score += 10;
            }
        }
        public override void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(Sprite, Position);

        }
        public override void Reset() {
            Velocity = Vector2.Zero;
            Position = StartPosition;
        }
        public override void OnCollision(CollisionInfo collisionInfo) {
            Position -= collisionInfo.PenetrationVector;
            base.OnCollision(collisionInfo);
        }

        public void OnActorCollision(object sender, EventArgs args) {
            DecreaseLives();
            if (_lives <= 0) {
                GameState.Instance.CurrentGameState = GameStates.MainMenu;
                UIState.Instance.CurrentUIState = UIStates.MainMenu;
                UIState.Instance.GuiSystem.ActiveScreen.Show();
                this._lives = 3;
                return;
            }
            Environment.Instance.Reset();
        }

        public void DecreaseLives() {
            _lives--;
        }
    }
}