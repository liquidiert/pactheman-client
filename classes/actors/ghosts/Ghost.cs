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

        public Ghost(ContentManager content, string spriteSheeLocation) : base(content, spriteSheeLocation) {}

        public string Name { get; set; }
        protected readonly float SCATTER_SECONDS = 3.5f;

        protected List<Vector2> MovesToMake;
        protected GhostStates CurrentGhostState = GhostStates.Chase;

        protected MovingStates CurrentMovingState {
            get { return movingState; } 
            set { movingState = value; }
        }

        public override void Move(GameTime t, GraphicsDeviceManager g) {}
        public override void Draw(SpriteBatch b){}

    }

}