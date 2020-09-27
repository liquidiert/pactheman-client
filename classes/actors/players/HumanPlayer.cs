using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Input;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Content;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Serialization;
using System;

namespace pactheman_client {
    class HumanPlayer : Actor {

        private ContentManager _content;
        private KeyboardStateExtended _kState;

        private MovingStates CurrentMovingState {
            get { return movingState; } 
            set {
                if (movingState != value) {
                    movingState = value;
                    switch(movingState) {
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

        private TiledMap map;

        public HumanPlayer(ContentManager content, Environment env, TiledMap map) {
            this._content = content;
            // HACK: MonoGame.Extended somehow can't read xnb files; thus always be sure the file is present in build dir!
            var spriteSheet = _content.Load<SpriteSheet>("sprites/player/spriteFactory.sf", new JsonContentLoader());
            this.Sprite = new AnimatedSprite(spriteSheet);
            this.MovementSpeed = 350f;
            this._environment = env;
            this.Position = env.PlayerStartPoints.PopAt(new Random().Next(env.PlayerStartPoints.Count)).Position;
            this.map = map;
            this.Sprite.Play(this.Position.X < 1120 ? "right" : "left");
        }

        private Vector2 UpdatePosition(float x = 0, int xFactor = 1, float y = 0, int yFactor = 1) {
            return new Vector2() { X = (this.Position.X + x) * xFactor, Y = (this.Position.Y + y) * yFactor};
        }

        public override void Move(GameTime gameTime, GraphicsDeviceManager graphics) {
            var delta = (float) gameTime.ElapsedGameTime.TotalSeconds;
            _kState = KeyboardExtended.GetState();

            Vector2 updatedPosition = new Vector2();
            if (_kState.IsKeyDown(Keys.W)) { // up
                this.DirectionY = -1;
                this.DirectionX = 0;
                updatedPosition = this.UpdatePosition(y: this.MovementSpeed * delta * this.DirectionY);
                this.CurrentMovingState = MovingStates.Up;
            } else if (_kState.IsKeyDown(Keys.S)) { // down
                this.DirectionY = 1;
                this.DirectionX = 0;
                updatedPosition = this.UpdatePosition(y: this.MovementSpeed * delta);
                this.CurrentMovingState = MovingStates.Down;
            } else if (_kState.IsKeyDown(Keys.A)) { // left
                this.DirectionX = -1;
                this.DirectionY = 0;
                updatedPosition = this.UpdatePosition(x: this.MovementSpeed * delta * this.DirectionX);
                this.CurrentMovingState = MovingStates.Left;
            } else if (_kState.IsKeyDown(Keys.D)) { // right
                this.DirectionX = 1;
                this.DirectionY = 0;
                updatedPosition = this.UpdatePosition(x: this.MovementSpeed * delta);
                this.CurrentMovingState = MovingStates.Right;
            }

            if (updatedPosition != Vector2.Zero && !_environment.InsideWall(this, delta)) {
                if (updatedPosition.X <= 70 || updatedPosition.X >= 1145) {
                    this.Position = this.UpdatePosition(x: -1216, xFactor: -1);
                } else {
                    this.Position = updatedPosition;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(this.Sprite, this._transform);
        }
    }
}