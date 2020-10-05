using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Collisions;
using System;
using System.Linq;
using System.Collections.Generic;

namespace pactheman_client {

    class Clyde : Ghost {

        private List<Vector2> _nextSteps(GameTime gameTime) {
            var isCloseToCenter = DownScaledPosition.X > 2 || DownScaledPosition.X <= 20 && DownScaledPosition.Y > 2 || DownScaledPosition.Y <= 17;
            var possibleTargets = ((Tuple<Vector2, int>[,]) Environment.Instance.MapAsTiles.GetRegion(
                DownScaledPosition,
                regionSize: isCloseToCenter ? 5 : 3
            )).Where(t => t.Item2 == 0).Select(t => t.Item1).ToList();
            return AStar.Instance.GetPath(
                DownScaledPosition,
                possibleTargets[new Random().Next(possibleTargets.Count)],
                iterDepth: 5
            );
        }

        public Clyde(ContentManager content, string name) : base(content, "sprites/ghosts/spriteFactoryClyde.sf") {
            this.Sprite.Play("moving");
            this.Position = Environment.Instance.GhostStartPoints.Pop(new Random().Next(Environment.Instance.GhostStartPoints.Count)).Position;
            this.MovementSpeed = 275f;
            this.StartPosition = Position;
            this.Name = name;
            var region = (Tuple<Vector2, int>[,]) Environment.Instance.MapAsTiles.GetRegion(DownScaledPosition, regionSize: 7);
            var startTargets = region.Where(t => t.Item2 == 0).Select(t => t.Item1).ToList();
            this.MovesToMake = AStar.Instance.GetPath(
                DownScaledPosition,
                startTargets[new Random().Next(startTargets.Count)],
                iterDepth: 5
            );
            this.lastTarget = (MovesToMake.Pop() * 64).AddValue(32);
        }

        public override void Move(GameTime gameTime) {
            float delta = gameTime.GetElapsedSeconds();

            Vector2 target;
            switch (this.CurrentGhostState) {
                case GhostStates.Chase:
                    target = lastTarget;
                    if (MovesToMake.IsEmpty()) MovesToMake = _nextSteps(gameTime);
                    if (Position.EqualsWithTolerence(lastTarget, 5f)) {
                        target = lastTarget = (MovesToMake.Pop() * 64).AddValue(32);
                    }
                    Velocity = target - Position;
                    Position += Velocity.RealNormalize() * MovementSpeed * delta;
                    break;
                case GhostStates.Scatter:
                    // move to lower left corner
                    target = lastTarget;
                    if (Position.EqualsWithTolerence(lastTarget, 5f)) {
                        try {
                            target = lastTarget = (MovesToMake.Pop() * 64).AddValue(32);
                        } catch (ArgumentOutOfRangeException) {
                            CurrentGhostState = GhostStates.Chase;
                            break;
                        }
                    }
                    if (scatterTicker >= SCATTER_SECONDS) {
                        MovesToMake = _nextSteps(gameTime);
                        CurrentGhostState = GhostStates.Chase;
                        scatterTicker = 0;
                        break;
                    }
                    Velocity = target - Position;
                    Position += Velocity.RealNormalize() * MovementSpeed * delta;
                    scatterTicker += delta;
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