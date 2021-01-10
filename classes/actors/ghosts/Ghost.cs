using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace pactheman_client {

    enum GhostStates {
        Scatter,
        Chase,
        Frightened
    }

    class Ghost : Actor {

        public Ghost(ContentManager content, string spriteSheeLocation) : base(content, spriteSheeLocation) {
            this.MovementSpeed = 0.5f;
            UIState.Instance.StateChanged += (object sender, UIStateEvent args) => {
                if (args.CurrentState == UIStates.Game && !Environment.Instance.IsOnline) {
                    Task.Delay(TimeSpan.FromMilliseconds(new Random().NextDouble() * 5000))
                        .ContinueWith(task => Waiting = false);
                }
            };
        }

        public bool Waiting = true;
        protected readonly float SCATTER_SECONDS = 3.5f;
        protected float scatterTicker { get; set; }
        protected Vector2 lastTarget { get; set; }
        protected Vector2 scatterTarget { get; set; }
        protected MoveInstruction moveInstruction { get; set; }

        public List<Vector2> Targets = new List<Vector2>();
        protected GhostStates CurrentGhostState = GhostStates.Chase;

        public override void Move(GameTime t) {
            if (Waiting) return;
            float delta = t.GetElapsedSeconds();
            if (!Environment.Instance.IsOnline) {
                Vector2 target;
                switch (this.CurrentGhostState) {
                    case GhostStates.Chase:
                        target = lastTarget;
                        if (Targets.IsEmpty()) Targets = Environment.Instance.GhostMoveInstructions[Name].GetMoves();
                        if (Position.EqualsWithTolerence(lastTarget, 5f)) {
                            target = lastTarget = (Targets.Pop() * 64).AddValue(32);
                        }
                        Velocity = target - Position;
                        Position += Velocity.RealNormalize() * MovementSpeed * delta;
                        break;
                    case GhostStates.Scatter:
                        // move to lower left corner
                        target = lastTarget;
                        if (Position.EqualsWithTolerence(lastTarget, 5f)) {
                            try {
                                target = lastTarget = (Targets.Pop() * 64).AddValue(32);
                            } catch (ArgumentOutOfRangeException) {
                                CurrentGhostState = GhostStates.Chase;
                                break;
                            }
                        }
                        if (scatterTicker >= SCATTER_SECONDS) {
                            Targets = Environment.Instance.GhostMoveInstructions[Name].GetMoves();
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
            } else {
                Vector2 target = lastTarget;
                if (Position.EqualsWithTolerence(lastTarget, 5f)) {
                    target = lastTarget = (Targets.Pop() * 64).AddValue(32);
                }
                Velocity = target - Position;
                Position += Velocity.RealNormalize() * MovementSpeed * delta;
            }
        }
        public override void Draw(SpriteBatch b) { }
        public override void Reset() {
            Velocity = Vector2.Zero;
            Position = StartPosition;
            Targets.Clear();
            lastTarget = Position;
        }
        public override void Clear() {
            Waiting = true;
            Velocity = Vector2.Zero;
            StartPosition = Environment.Instance.GhostStartPoints
                .Pop(new Random().Next(Environment.Instance.GhostStartPoints.Count)).Position.AddValue(32);
            Position = StartPosition;
            lastTarget = Position;
            Targets.Clear();
        }

    }

}