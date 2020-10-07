using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Sprites;
using System;
using System.Linq;
using System.Collections.Generic;

namespace pactheman_client {

    class Inky : Ghost {

        private Vector2 _randomPatrollingTarget {
            get {
                var targetPos = Environment.Instance.PacMan.DownScaledPosition;
                var possibleTargets = ((Tuple<Vector2, int>[,]) Environment.Instance.MapAsTiles.GetRegion(
                    targetPos,
                    regionSize: 3))
                        .Where(t => t.Item2 == 0).Select(t => t.Item1).ToList();
                return possibleTargets[new Random().Next(possibleTargets.Count)];
            }
        }
        private List<Vector2> _nextSteps => AStar.Instance.GetPath(DownScaledPosition, _randomPatrollingTarget, iterDepth: 5);

        public Inky(ContentManager content, string name) : base(content, "sprites/ghosts/spriteFactoryInky.sf") {
            this.Sprite.Play("moving");
            this.Position = Environment.Instance.GhostStartPoints.Pop(new Random().Next(Environment.Instance.GhostStartPoints.Count)).Position;
            this.StartPosition = Position;
            this.Name = name;
            this.MovesToMake = AStar.Instance.GetPath(DownScaledPosition, Environment.Instance.PacMan.Position, iterDepth: 5);
            this.lastTarget = (MovesToMake.Pop() * 64).AddValue(32);
        }
        public override void Move(GameTime gameTime) {
            if (Waiting) return;
            float delta = gameTime.GetElapsedSeconds();

            Vector2 target;
            switch (this.CurrentGhostState) {
                case GhostStates.Chase:
                    target = lastTarget;
                    if (MovesToMake.IsEmpty()) MovesToMake = _nextSteps;
                    if (Position.EqualsWithTolerence(lastTarget, 5f)) {
                        target = lastTarget = (MovesToMake.Pop() * 64).AddValue(32);
                    }
                    Velocity = target - Position;
                    Position += Velocity.RealNormalize() * MovementSpeed * delta;
                    break;
                case GhostStates.Scatter:
                    // move to lower right corner
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
                        MovesToMake = _nextSteps;
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