using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;

namespace pactheman_client {

    class Blinky : Ghost {

        private Vector2 _lastTarget;
        private float _scatterTicker;

        private List<Vector2> fiveSteps => AStar.Instance.GetPath(DownScaledPosition, Environment.Instance.PacMan.DownScaledPosition, iterDepth: 5);

        public Blinky(ContentManager content, string name) : base(content, "sprites/ghosts/spriteFactory.sf") {
            this.Sprite.Play("moving");
            this.Position = Environment.Instance.GhostStartPoints.Pop(new Random().Next(Environment.Instance.GhostStartPoints.Count)).Position;
            this.Name = name;
            this.MovementSpeed = 275f;
            this.MovesToMake = AStar.Instance.GetPath(DownScaledPosition, Environment.Instance.PacMan.DownScaledPosition, iterDepth: 10);
            this._lastTarget = (MovesToMake.Pop() * 64).AddValue(32);
        }

        public override void Move(GameTime gameTime) {
            float delta = gameTime.GetElapsedSeconds();

            Vector2 target;
            switch (this.CurrentGhostState) {
                case GhostStates.Chase:
                    target = _lastTarget;
                    if (Position.EqualsWithTolerence(_lastTarget, 5f)) {
                        try {
                            target = _lastTarget = (MovesToMake.Pop() * 64).AddValue(32);
                        } catch (ArgumentOutOfRangeException) {
                            MovesToMake = fiveSteps;
                            if (MovesToMake.IsEmpty()) { // hussa pacman reached!
                                // TODO: rather handle that via collision
                                Environment.Instance.PacMan.DecreaseLives();
                                CurrentGhostState = GhostStates.Scatter;
                                MovesToMake = AStar.Instance.GetPath(DownScaledPosition, new Vector2(17, 1));
                                break;
                            }
                            target = _lastTarget = (MovesToMake.Pop() * 64).AddValue(32);
                        }
                    }
                    Velocity = target - Position;
                    Position += Velocity.RealNormalize() * MovementSpeed * delta;
                    break;
                case GhostStates.Scatter:
                    // move to upper right corner
                    target = _lastTarget;
                    if (Position.EqualsWithTolerence(_lastTarget, 5f)) {
                        try {
                            target = _lastTarget = (MovesToMake.Pop() * 64).AddValue(32);
                        } catch (ArgumentOutOfRangeException) {
                            CurrentGhostState = GhostStates.Chase;
                            break;
                        }
                    }
                    if (_scatterTicker >= SCATTER_SECONDS) {
                        MovesToMake = fiveSteps;
                        CurrentGhostState = GhostStates.Chase;
                        _scatterTicker = 0;
                        break;
                    }
                    Velocity = target - Position;
                    Position += Velocity.RealNormalize() * MovementSpeed * delta;
                    _scatterTicker += delta;
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