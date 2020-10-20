using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace pactheman_client {
    class RandomAStarMove : MoveInstruction {

        public RandomAStarMove(Actor moveable, Actor target) : base(moveable, target) {}

        public override List<Vector2> GetMoves(float elapsedSeconds, int iterDepth = 5) {
            var isCloseToCenter = Moveable.DownScaledPosition.X > 2 || Moveable.DownScaledPosition.X <= 20 && Moveable.DownScaledPosition.Y > 2 || Moveable.DownScaledPosition.Y <= 17;
            var possibleTargets = ((Tuple<Vector2, int>[,]) Environment.Instance.MapAsTiles.GetRegion(
                Moveable.DownScaledPosition,
                regionSize: isCloseToCenter ? 5 : 3
            )).Where(t => t.Item2 == 0).Select(t => t.Item1).ToList();
            return AStar.Instance.GetPath(
                Moveable.DownScaledPosition,
                possibleTargets[new Random().Next(possibleTargets.Count)],
                iterDepth: iterDepth
            );
        }
    }
}