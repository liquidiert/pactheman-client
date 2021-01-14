using Bebop.Attributes;
using Bebop.Runtime;
using PacTheMan.Models;
using System;

namespace pactheman_client {
    [RecordHandler]
    public static class ResetHandler {

        [BindRecord(typeof(BebopRecord<ResetMsg>))]
        public static void HandleResetMsg(object client, ResetMsg msg) {
            try {
                Console.WriteLine("received reset");
                foreach (var live in msg.PlayerLives) {
                    if (live.Key == GameEnv.Instance.PlayerOne.InternalPlayerState.Session.ClientId) {
                        (GameEnv.Instance.Actors["player"] as Player).SetLives((int)live.Value);
                    } else {
                        (GameEnv.Instance.Actors["opponent"] as Player).SetLives((int)live.Value);
                    }
                }
                GameEnv.Instance.Reset();
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}