using Microsoft.Xna.Framework;

namespace pactheman_client {

    static class Vector2Extension {

        public static Vector2 RealNormalize(this Vector2 vec) {
            vec /= vec.Length();
            vec.Round();
            return vec;
        }

        public static Vector2 SubtractValue(this Vector2 vec, int toSubtract) {
            vec.X -= toSubtract;
            vec.Y -= toSubtract;
            return vec;
        }

        public static Vector2 AddValue(this Vector2 vec, int toAdd) {
            vec.X += toAdd;
            vec.Y += toAdd;
            return vec;
        }

    }

}