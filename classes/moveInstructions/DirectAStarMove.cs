using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace pactheman_client {
    class DirectAStarMove : MoveInstruction {

        public DirectAStarMove(Actor moveable, Actor target) : base(moveable, target) {
            Moveable = moveable;
            Target = target;
        }

        public override List<Vector2> GetMoves(float elapsedSeconds, int iterDepth = 5) {
            return AStar.Instance.GetPath(Moveable.DownScaledPosition, Target.DownScaledPosition, iterDepth: iterDepth);
        }
    }
}