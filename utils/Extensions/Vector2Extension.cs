using Microsoft.Xna.Framework;

namespace pactheman_client {

    static class Vector2Extension {

        public static Vector2 RealNormalize(this Vector2 vector) {
            vector /= vector.Length();
            vector.Round();
            return vector;
        }

        public static Vector2 SubtractValue(this Vector2 vector, int toSubtract) {
            vector.X -= toSubtract;
            vector.Y -= toSubtract;
            return vector;
        }

        public static Vector2 AddValue(this Vector2 vector, int toAdd) {
            vector.X += toAdd;
            vector.Y += toAdd;
            return vector;
        }

        public static Vector2 DivideValue(this Vector2 vector, int toDivide) {
            vector.X /= toDivide;
            vector.Y /= toDivide;
            return vector;
        }

        public static Vector2 CeilInstance(this Vector2 vector) {
            vector.Ceiling();
            return vector;
        }

    }

}