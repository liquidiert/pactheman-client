using System;
using MonoGame.Extended;

namespace pactheman_client {
    class CollisionPair {

        private Actor collidable1;
        private Actor collidable2;

        public bool Enabled { get; set; }
        public int UpdateOrder { get; set; }
        public event EventHandler<EventArgs> Collision;

        public CollisionPair(params Actor[] actors) => (collidable1, collidable2) = (actors[0], actors[1]);

        public void Update() {
            if (collidable1.Position.EqualsWithTolerence(collidable2.Position, 32f)) {
                // activate event with dummy args -> collidables already know they are meant
                Collision.Invoke(this, new EventArgs());
            }
        }

    }
}