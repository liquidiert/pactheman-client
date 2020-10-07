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
            Task.Delay(TimeSpan.FromMilliseconds(new Random().NextDouble() * 4500))
                .ContinueWith(task => Waiting = false);
        }

        public string Name { get; set; }
        public bool Waiting = true;
        protected readonly float SCATTER_SECONDS = 3.5f;
        protected float scatterTicker { get; set; }
        protected Vector2 lastTarget { get; set; }

        protected List<Vector2> MovesToMake;
        protected GhostStates CurrentGhostState = GhostStates.Chase;

        protected MovingStates CurrentMovingState {
            get { return movingState; } 
            set { movingState = value; }
        }

        public override void Move(GameTime t) {}
        public override void Draw(SpriteBatch b){}
        public override void Reset() {
            Velocity = Vector2.Zero;
            Position = StartPosition.AddValue(32);
            MovesToMake.Clear();
            lastTarget = Position;
        }

    }

}