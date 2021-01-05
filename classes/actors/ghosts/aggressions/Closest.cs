using Microsoft.Xna.Framework;

namespace pactheman_client {
    public class ClosestAggression : Aggression {
        public void SelectTarget(Actor aggressor) {
            if (aggressor.Position.Distance(Environment.Instance.Actors[0].Position) < aggressor.Position.Distance(Environment.Instance.Actors[1].Position)) {
                Environment.Instance.GhostMoveInstructions[aggressor.Name].Target = Environment.Instance.Actors[0];
            } else {
                Environment.Instance.GhostMoveInstructions[aggressor.Name].Target = Environment.Instance.Actors[1];
            }
        }
    }
}