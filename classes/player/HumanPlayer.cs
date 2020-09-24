using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Input;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;

namespace pactheman_client {
    class HumanPlayer : Player {

        private ContentManager _content;
        private KeyboardStateExtended _kState;

        public HumanPlayer(ContentManager content) {
            this._content = content;
            // HACK: MonoGame.Extended somehow can't read xnb files; thus always be sure the file is present in build dir!
            var spriteSheet = _content.Load<SpriteSheet>("sprites/player/spriteFactory.sf", new JsonContentLoader());
            this.Sprite = new AnimatedSprite(spriteSheet);
            this.MovementSpeed = 350f;

            // TODO: play Sprite according to start pos
            // this.Sprite.Play("");
        }

        private Vector2 UpdatePosition(float x = 0, float y = 0) {
            return new Vector2() { X = this.Position.X + x, Y = this.Position.Y + y};
        }

        public override void Move(GameTime gameTime) {
            var delta = (float) gameTime.ElapsedGameTime.TotalSeconds;
            _kState = KeyboardExtended.GetState();

            if (_kState.IsKeyDown(Keys.W)) { // up
                this.Position = this.UpdatePosition(y: this.MovementSpeed * delta * -1);
                this.CurrentMovingState = MovingStates.Up;
            } else if (_kState.IsKeyDown(Keys.S)) { // down
                this.Position = this.UpdatePosition(y: this.MovementSpeed * delta);
                this.CurrentMovingState = MovingStates.Down;
            } else if (_kState.IsKeyDown(Keys.A)) { // left
                this.Position = this.UpdatePosition(x: this.MovementSpeed * delta * -1);
                this.CurrentMovingState = MovingStates.Left;
            } else if (_kState.IsKeyDown(Keys.D)) { // right
                this.Position = this.UpdatePosition(x: this.MovementSpeed * delta);
                this.CurrentMovingState = MovingStates.Right;
            }
        }
    }
}