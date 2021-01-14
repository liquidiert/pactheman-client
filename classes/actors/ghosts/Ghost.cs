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
            this.MovementSpeed = 250f;
            UIState.Instance.StateChanged += async (object sender, UIStateEvent args) => {
                if (args.CurrentState == UIStates.Game && !GameEnv.Instance.IsOnline) {
                    await Task.Delay(TimeSpan.FromMilliseconds(new Random().Next(5000)))
                        .ContinueWith(task => Waiting = false);
                }
            };
        }

        public bool Waiting = true;
        protected readonly float SCATTER_SECONDS = 3.5f;
        protected float scatterTicker { get; set; }
        protected Vector2 lastTarget { get; set; }
        public Vector2 LastTarget { 
            get => lastTarget;
            set => lastTarget = value;
        }
        protected Vector2 scatterTarget { get; set; }
        protected MoveInstruction moveInstruction { get; set; }

        public List<Vector2> Targets = new List<Vector2>();
        protected GhostStates CurrentGhostState = GhostStates.Chase;

        public override void Move(GameTime t) {
            float delta = t.GetElapsedSeconds();
            if (!GameEnv.Instance.IsOnline) {
                if (Waiting) return;
                Vector2 target;
                switch (this.CurrentGhostState) {
                    case GhostStates.Chase:
                        target = lastTarget;
                        if (Targets.IsEmpty()) Targets = GameEnv.Instance.GhostMoveInstructions[Name].GetMoves();
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
                            Targets = GameEnv.Instance.GhostMoveInstructions[Name].GetMoves();
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
                if (Targets.Count == 0) return;
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
            StartPosition = GameEnv.Instance.GhostStartPoints
                .Pop(new Random().Next(GameEnv.Instance.GhostStartPoints.Count)).Position.AddValue(32);
            Position = StartPosition;
            lastTarget = Position;
            Targets.Clear();
        }

    }

}