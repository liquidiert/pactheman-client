using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;

namespace pactheman_client {

    class Inky : Ghost {

        public Inky(ContentManager content, string name) : base(content, "sprites/ghosts/spriteFactoryInky.sf") {
            this.Sprite.Play("moving");
            this.Position = Environment.Instance.GhostStartPoints
                .Pop(new Random().Next(Environment.Instance.GhostStartPoints.Count)).Position;
            this.StartPosition = Position;
            this.Name = name;
            this.MovesToMake = new List<Vector2>();
            this.lastTarget = StartPosition.AddValue(32);
        }
        public override void Move(GameTime gameTime) {
            if (Waiting) return;
            float delta = gameTime.GetElapsedSeconds();

            Vector2 target;
            switch (this.CurrentGhostState) {
                case GhostStates.Chase:
                    target = lastTarget;
                    if (MovesToMake.IsEmpty()) MovesToMake = Environment.Instance.GhostMoveInstructions[Name].GetMoves();
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
                        MovesToMake = Environment.Instance.GhostMoveInstructions[Name].GetMoves();
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