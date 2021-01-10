using Bebop.Attributes;
using Bebop.Runtime;
using PacTheMan.Models;
using System;

namespace pactheman_client {

    [RecordHandler]
    public static class PlayerStateHandler {

        [BindRecord(typeof(BebopRecord<PlayerState>))]
        public static void HandlePlayerStateMsg(object client, PlayerState msg) {
            HumanPlayer player = (HumanPlayer) client;

            Console.WriteLine("got state");

            player.InternalPlayerState.ReconciliationId = msg.ReconciliationId;
            player.InternalPlayerState.PlayerPositions = msg.PlayerPositions;
            player.InternalPlayerState.GhostPositions = msg.GhostPositions;
            player.InternalPlayerState.Lives = msg.Lives;
            player.InternalPlayerState.Score = msg.Score;
        }
    }
}