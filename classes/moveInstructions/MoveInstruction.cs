using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;

namespace pactheman_client {
    public abstract class MoveInstruction {
        public Actor Moveable;
        public Actor Target;

        public MoveInstruction(Actor moveable, Actor target) => (moveable, target) = (Moveable, Target);

        public abstract List<Vector2> GetMoves(float elapsedSeconds = 1, int iterDepth = 5);
    }
}