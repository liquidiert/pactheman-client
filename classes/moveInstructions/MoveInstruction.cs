using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace pactheman_client {

    public abstract class MoveInstruction {

        public static readonly Dictionary<string, string> HumanReadableMoveInstructions = new Dictionary<string, string> {
            {"direct_astar", "Direct AStar"},
            {"patroling_astar", "Patroling AStar"},
            {"predicted_astar", "Predicted AStar"},
            {"random_astar", "Random AStar"},
        };

        public Actor Moveable;
        public Actor Target;

        public MoveInstruction(Actor moveable, Actor target) => (moveable, target) = (Moveable, Target);

        public abstract List<Vector2> GetMoves(float elapsedSeconds = 1, int iterDepth = 5);
    }
}