using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.Input;

namespace pactheman_client {
    class Opponent : Player {

        public Opponent(ContentManager content, string name) : base(content, name, "sprites/opponent/spriteFactory.sf") {
            this.StatsPosition = new Vector2(1300, 50);
        }

        public override void Move(GameTime gameTime) {
            var delta = gameTime.GetElapsedSeconds();

            Vector2 updatedPosition;

            if (!Environment.Instance.IsOnline) {
                // TODO: add real ai movement if IsHooman is false
                if (IsHooman) {
                    updatedPosition = keyboardMove(delta, true);
                } else {
                    updatedPosition = new Vector2();
                }
            } else {
                updatedPosition = Position;
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