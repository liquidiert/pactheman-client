using Microsoft.Xna.Framework;

namespace pactheman_client {
    public class ClosestAggression : Aggression {
        public void SelectTarget(Actor aggressor) {
            if (aggressor.Position.Distance(Environment.Instance.Actors["player"].Position) < aggressor.Position.Distance(Environment.Instance.Actors["opponent"].Position)) {
                Environment.Instance.GhostMoveInstructions[aggressor.Name].Target = Environment.Instance.Actors["player"];
            } else {
                Environment.Instance.GhostMoveInstructions[aggressor.Name].Target = Environment.Instance.Actors["opponent"];
            }
        }
    }
}