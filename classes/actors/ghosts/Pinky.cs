using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;

namespace pactheman_client {

    class Pinky : Ghost {

        private List<Vector2> nextSteps(GameTime gameTime) => AStar.Instance.GetPath(
            DownScaledPosition, Environment.Instance.PacMan.FuturePosition(gameTime).DivideValue(64).CeilInstance(), iterDepth: 2);

        public Pinky(ContentManager content, string name) : base(content, "sprites/ghosts/spriteFactoryPinky.sf") {
            this.Sprite.Play("moving");
            this.Position = Environment.Instance.GhostStartPoints.Pop(new Random().Next(Environment.Instance.GhostStartPoints.Count)).Position;
            this.Name = name;
            this.MovementSpeed = 275f;
            this.MovesToMake = AStar.Instance.GetPath(DownScaledPosition, Environment.Instance.PacMan.Position, iterDepth: 5);
            this.lastTarget = (MovesToMake.Pop() * 64).AddValue(32);
        }

        public override void Move(GameTime gameTime) {
            float delta = gameTime.GetElapsedSeconds();

            Vector2 target;
            switch (this.CurrentGhostState) {
                case GhostStates.Chase:
                    target = lastTarget;
                    if (Position.EqualsWithTolerence(lastTarget, 5f)) {
                        try {
                            target = lastTarget = (MovesToMake.Pop() * 64).AddValue(32);
                        } catch (ArgumentOutOfRangeException) {
                            MovesToMake = nextSteps(gameTime);
                            if (MovesToMake.IsEmpty()) { // hussa pacman reached!
                                CurrentGhostState = GhostStates.Scatter;
                                MovesToMake = AStar.Instance.GetPath(DownScaledPosition, new Vector2(1, 1));
                                break;
                            }
                            target = lastTarget = (MovesToMake.Pop() * 64).AddValue(32);
                        }
                    }
                    Velocity = target - Position;
                    Position += Velocity.RealNormalize() * MovementSpeed * delta;
                    break;
                case GhostStates.Scatter:
                    // move to upper left corner
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
                        MovesToMake = nextSteps(gameTime);
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