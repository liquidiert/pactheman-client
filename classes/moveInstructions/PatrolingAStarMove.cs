using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace pactheman_client {
    public class PatrolingAStarMove : MoveInstruction {

        private Vector2 _randomPatrollingTarget {
            get {
                var targetPos = Target.DownScaledPosition;
                var possibleTargets = ((Tuple<Vector2, int>[,]) GameEnv.Instance.MapAsTiles.GetRegion(
                    targetPos,
                    regionSize: 3))
                        .Where(t => t.Item2 == 0).Select(t => t.Item1).ToList();
                return possibleTargets[new Random().Next(possibleTargets.Count)];
            }
        }

        public PatrolingAStarMove(Actor moveable, Actor target) : base(moveable, target) {}

        public override List<Vector2> GetMoves(float elapsedSeconds, int iterDepth = 5) {
            return AStar.Instance.GetPath(Moveable.DownScaledPosition, _randomPatrollingTarget, iterDepth: iterDepth);
        }
    }
}