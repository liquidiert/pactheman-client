using System;
using System.Linq;
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
 
        /** 
        * returns: Index of the minimal member
        **/
        public static int MinIndex <T>(this List<T> list) where T: IComparable<T> {
            dynamic min = Int32.MaxValue;
            int res = 0;
            foreach (var el in list.Select((entry, ind) => new { entry = entry, ind = ind})) {
                if (el.entry < min) {
                    min = el.entry;
                    res = el.ind;
                }
            }
            return res;
        }
    }
}