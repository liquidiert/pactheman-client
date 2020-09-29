using System;

namespace pactheman_client {

    static class TowDimArrayExtension {

        public static void Print(this int[,] arr) {
            for (var h = 0; h < arr.GetLength(1) - 1; h++) {
                for (var w = 0; w < arr.GetLength(0) - 1; w++) {
                    Console.Write($"{arr[w, h]} ");
                }
                Console.WriteLine("");
            }
        }

    }
}