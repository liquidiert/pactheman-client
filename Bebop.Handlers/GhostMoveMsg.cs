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
            foreach (var reset in msg.State.ClearTargets) {
                if (reset.Value) {
                    (Environment.Instance.Actors[reset.Key] as Ghost).Targets = new List<Vector2>();
                }
            }
            foreach (var target in msg.State.Targets) {
                (Environment.Instance.Actors[target.Key] as Ghost)
                    .Targets.Add(new Vector2(target.Value.X, target.Value.Y));
            }
        }
    }
}