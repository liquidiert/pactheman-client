using System;
using System.Collections.Generic;
using PacTheMan.Models;
using Microsoft.Xna.Framework;

namespace pactheman_client {

    public class VectorEqualityComparator : IEqualityComparer<Vector2> {
        public bool Equals(Vector2 vec1, Vector2 vec2) {
            return vec1.X == vec2.X && vec1.Y == vec2.Y;
        }

        public int GetHashCode(Vector2 vec) {
            int code = (int)vec.X ^ (int)vec.Y;
            return code.GetHashCode();
        }
    }

    static class Vector2Extension {

        public static Position ToPosition(this Vector2 vector) {
            return new Position { X = vector.X, Y = vector.Y };
        }

        public static Vector2 RealNormalize(this Vector2 vector) {
            if (vector == Vector2.Zero) return vector;
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

        public static Vector2 FloorInstance(this Vector2 vector) {
            vector.Floor();
            return vector;
        }

        public static double Distance(this Vector2 vector, Vector2 toCompare) {
            return Math.Sqrt((vector.X - toCompare.X) * (vector.X - toCompare.X) + (vector.Y - toCompare.Y) * (vector.Y - toCompare.Y));
        }

        public static Vector2 Interpolated(this Vector2 vector, Vector2 other) {
            return new Vector2 {
                X = (vector.X + other.X) / 2,
                Y = (vector.Y + other.Y) / 2
            };
        }

    }

}