using System.Collections.Generic;

namespace pactheman_client {
    static class ListExtension {
        public static T PopAt<T>(this List<T> list, int index) {
            T r = list[index];
            list.RemoveAt(index);
            return r;
        }

        public static List<T> AddMany<T>(this List<T> list, params T[] toAdd) {
            foreach(var obj in toAdd) {
                list.Add(obj);
            }
            return list;
        }
    }
}