using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.Input;

namespace pactheman_client {
    class HumanPlayer : Player {

        public HumanPlayer(ContentManager content, string name) : base(content, name, "sprites/player/spriteFactory.sf") {
            this.StatsPosition = new Vector2(-350, 50);
        }

        public override void Move(GameTime gameTime) {
            var delta = gameTime.GetElapsedSeconds();
            var _kState = KeyboardExtended.GetState();

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
    }
}