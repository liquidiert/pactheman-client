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

                // teleport if entering either left or right gate
                if (updatedPosition.X <= 38 || updatedPosition.X >= 1177) {
                    Position = UpdatePosition(x: -1215, xFactor: -1);
                } else {
                    Position = updatedPosition;
                }
            } else {
                updatedPosition = Position;
            }

            if (Environment.Instance.RemoveScorePoint(Position)) {
                _score += 10;
            }
        }
    }
}