using Microsoft.Xna.Framework;

namespace pactheman_client {
    public class ClosestAggression : Aggression {
        public void SelectTarget(Actor aggressor) {
            if (aggressor.Position.Distance(GameEnv.Instance.Actors["player"].Position) < aggressor.Position.Distance(GameEnv.Instance.Actors["opponent"].Position)) {
                GameEnv.Instance.GhostMoveInstructions[aggressor.Name].Target = GameEnv.Instance.Actors["player"];
            } else {
                GameEnv.Instance.GhostMoveInstructions[aggressor.Name].Target = GameEnv.Instance.Actors["opponent"];
            }
        }
    }
}