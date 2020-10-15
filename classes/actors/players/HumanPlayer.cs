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
    class HumanPlayer : Actor {

        private KeyboardStateExtended _kState;
        private int _score = 0;
        private int _lives = 3;
        public int Score {
            get => _score;
        }
        public string Lives {
            get => "<3".Multiple(_lives);
        }
        public string Name = "PlayerOne";

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

        public HumanPlayer(ContentManager content, TiledMap map) : base(content, "sprites/player/spriteFactory.sf") {
            this.Position = Environment.Instance.PlayerStartPoints.Pop(new Random().Next(Environment.Instance.PlayerStartPoints.Count)).Position;
            this.StartPosition = Position;
            this.Sprite.Play(this.Position.X < 1120 ? "right" : "left");
        }

        public override void Move(GameTime gameTime) {
            var delta = gameTime.GetElapsedSeconds();
            _kState = KeyboardExtended.GetState();

            // TODO: use rotation instead of dedicated animations
            Vector2 updatedPosition = Position;
            if (_kState.IsKeyDown(Keys.W)) { // up
                Velocity = new Vector2(0, -1);
                updatedPosition.Y -= MovementSpeed * delta;
                CurrentMovingState = MovingStates.Up;
            }
            if (_kState.IsKeyDown(Keys.S)) { // down
                Velocity = new Vector2(0, 1);
                updatedPosition.Y += MovementSpeed * delta;
                CurrentMovingState = MovingStates.Down;
            }
            if (_kState.IsKeyDown(Keys.A)) { // left
                Velocity = new Vector2(-1, 0);
                updatedPosition.X -= MovementSpeed * delta;
                CurrentMovingState = MovingStates.Left;
            }
            if (_kState.IsKeyDown(Keys.D)) { // right
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