using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Collisions;
using System;

namespace pactheman_client {

    class Blinky : Ghost {

        private Vector2 _lastTarget;

        public Blinky(ContentManager content, string name) : base(content, "sprites/ghosts/spriteFactory.sf") {
            this.Sprite.Play("moving");
            this.Position = Environment.Instance.GhostStartPoints.Pop(new Random().Next(Environment.Instance.GhostStartPoints.Count)).Position;
            this.Name = name;
            this.MovementSpeed = 300f;
            this.MovesToMake = AStar.Instance.GetPath(UpScaledPosition, Environment.Instance.PacMan.DownScaledPosition);
            this._lastTarget = (MovesToMake.Pop() * 64).AddValue(32);
        }

        public override void Move(GameTime gameTime, GraphicsDeviceManager graphics) {
            float delta = gameTime.GetElapsedSeconds();

            switch (this.CurrentGhostState) {
                case GhostStates.Chase:
                    var target = _lastTarget;
                    if (MovesToMake.IsEmpty()) MovesToMake = AStar.Instance.GetPath(UpScaledPosition, Environment.Instance.PacMan.DownScaledPosition, iterDepth: 10);
                    if (Position.EqualsWithTolerence(_lastTarget, 5f)) {
                        _lastTarget = (MovesToMake.Pop() * 64).AddValue(32);
                        target = _lastTarget;
                    }
                    MovesToMake.Print();
                    Velocity = target - Position;
                    Position += Velocity.RealNormalize() * MovementSpeed * delta;

                    break;
                case GhostStates.Scatter:
                    break;
                case GhostStates.Frightened:
                    break;
            }
        }
        public override void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(Sprite, Position);
        }
        public override void OnCollision(CollisionInfo collisionInfo) {
            Position -= collisionInfo.PenetrationVector;
            base.OnCollision(collisionInfo);
        }
    }
}