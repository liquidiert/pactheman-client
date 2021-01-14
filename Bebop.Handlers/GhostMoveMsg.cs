using Bebop.Attributes;
using Bebop.Runtime;
using PacTheMan.Models;
using System;

namespace pactheman_client {

    [RecordHandler]
    public static class GhostMoveHandler {

        [BindRecord(typeof(BebopRecord<GhostMoveMsg>))]
        public static void HandleGhostMove(object val, GhostMoveMsg msg) {
            try {
                foreach (var ghost in msg.GhostPositions) {
                    GameEnv.Instance.Actors[ghost.Key].Position = (ghost.Value as Position).ToVec2();
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}