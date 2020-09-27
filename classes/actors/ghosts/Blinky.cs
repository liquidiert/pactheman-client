using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Content;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Serialization;
using System;

namespace pactheman_client {

    class Blinky : Ghost {

        private Vector2 _lastTarget;
        private Vector2 _lastDirection = new Vector2();

        public Blinky(ContentManager content, string name) {
            // HACK: MonoGame.Extended somehow can't read xnb files; thus always be sure the file is present in build dir!
            var ghostSprite = content.Load<SpriteSheet>("sprites/ghosts/spriteFactory.sf", new JsonContentLoader());
            this.Sprite = new AnimatedSprite(ghostSprite, "moving");
            this.Position = Environment.Instance.GhostStartPoints.Pop(new Random().Next(Environment.Instance.GhostStartPoints.Count)).Position;
            this.Name = name;
            Console.WriteLine(ScaledPosition);
            this.MovesToMake = AStar.Instance.GetPath(ScaledPosition, Environment.Instance.PacMan.ScaledPosition);
            this._lastTarget = MovesToMake.Pop();
            MovesToMake.Print();
            Console.WriteLine(_lastTarget);
        }

        public override void Move(GameTime gameTime, GraphicsDeviceManager graphics) {
            float delta = (float) gameTime.ElapsedGameTime.TotalSeconds;
            var pacMan = Environment.Instance.PacMan;

            switch (this.CurrentGhostState) {
                case GhostStates.Chase:
                    var target = _lastTarget;
                    if (MovesToMake.IsEmpty()) MovesToMake = AStar.Instance.GetPath(ScaledPosition, Environment.Instance.PacMan.ScaledPosition, iterDepth: 5);
                    if (ScaledPosition == _lastTarget) {
                        _lastTarget = MovesToMake.Pop();
                        target = _lastTarget;
                    }
                    var direction = target - ScaledPosition;
                    direction.Normalize();
                    Vector2 updatedPos = UpdatePosition(
                        x: MovementSpeed * delta * direction.X,
                        y: MovementSpeed * delta * direction.Y
                    );

                    if (!Environment.Instance.InsideWall(this, updatedPos)) {
                        Position = updatedPos;
                    } else {
                        Position = new Vector2(
                            updatedPos.X + MovementSpeed * delta * _lastDirection.X * 0.25f,
                            updatedPos.Y + MovementSpeed * delta * _lastDirection.Y * 0.25f
                        );
                    }

                    _lastDirection = direction;

                    break;
                case GhostStates.Scatter:
                    break;
                case GhostStates.Frightened:
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(this.Sprite, this._transform);
        }
    }
}