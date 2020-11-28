using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace pactheman_client {

    enum GhostStates {
        Scatter,
        Chase,
        Frightened
    }

    class Ghost : Actor {

        public Ghost(ContentManager content, string spriteSheeLocation) : base(content, spriteSheeLocation) {
            this.MovementSpeed = 250f;
            UIState.Instance.StateChanged += (object sender, UIStateEvent args) => {
                if (args.CurrentState == UIStates.Game) {
                    Task.Delay(TimeSpan.FromMilliseconds(new Random().NextDouble() * 5000))
                        .ContinueWith(task => Waiting = false);
                }
            };
        }
        
        public bool Waiting = true;
        protected readonly float SCATTER_SECONDS = 3.5f;
        protected float scatterTicker { get; set; }
        protected Vector2 lastTarget { get; set; }
        protected MoveInstruction moveInstruction { get; set; }

        protected List<Vector2> MovesToMake;
        protected GhostStates CurrentGhostState = GhostStates.Chase;

        public override void Move(GameTime t) {
            if (Waiting) return;
            new ClosestAggression().SelectTarget(this);
        }
        public override void Draw(SpriteBatch b){}
        public override void Reset() {
            Velocity = Vector2.Zero;
            Position = StartPosition;
            MovesToMake.Clear();
            lastTarget = Position;
        }
        public override void Clear() {
            Waiting = true;
            Velocity = Vector2.Zero;
            StartPosition = Environment.Instance.GhostStartPoints
                .Pop(new Random().Next(Environment.Instance.GhostStartPoints.Count)).Position.AddValue(32);
            Position = StartPosition;
            lastTarget = Position;
            MovesToMake.Clear();
        }

    }

}