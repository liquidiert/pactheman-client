using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace pactheman_client {
    class PredictedAStarMove : MoveInstruction {

        public PredictedAStarMove(Actor moveable, Actor target) : base(moveable, target) {}

        public override List<Vector2> GetMoves(float elapsedSeconds, int iterDepth = 3) {
            return AStar.Instance.GetPath(
                Moveable.DownScaledPosition, Target.FuturePosition(elapsedSeconds).DivideValue(64).FloorInstance(), iterDepth: iterDepth);
        }
    }
}