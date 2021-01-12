using Bebop.Attributes;
using Bebop.Runtime;
using PacTheMan.Models;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace pactheman_client {

    [RecordHandler]
    public static class GhostMoveHandler {

        [BindRecord(typeof(BebopRecord<GhostMoveMsg>))]
        public static void HandleGhostMove(object val, GhostMoveMsg msg) {
            foreach (var pos in msg.State.Positions) {
                var t = new Vector2(pos.Value.X, pos.Value.Y);
                if (pos.Key == "blinky") {
                    Console.WriteLine(t.ToString());
                }
                Environment.Instance.Actors[pos.Key].Position = 
                    Environment.Instance.Actors[pos.Key].Position.Interpolated(t);
            }
        }
    }
}