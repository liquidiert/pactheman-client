using Bebop.Attributes;
using Bebop.Runtime;
using PacTheMan.Models;
using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace pactheman_client {

    [RecordHandler]
    public static class PlayerStateHandler {

        [BindRecord(typeof(BebopRecord<PlayerState>))]
        public static void HandlePlayerStateMsg(object client, PlayerState msg) {
            HumanPlayer player = (HumanPlayer) client;

            player.InternalPlayerState.ReconciliationId = msg.ReconciliationId;
            player.InternalPlayerState.PlayerPositions = msg.PlayerPositions;
            player.InternalPlayerState.GhostPositions = msg.GhostPositions;
            player.InternalPlayerState.Lives = msg.Lives;
            player.InternalPlayerState.Score = msg.Score;

            var clientId = player.InternalPlayerState.Session.ClientId;
            if (msg.Session.ClientId != clientId) {
                var oppPos = player.InternalPlayerState.PlayerPositions.First(p => p.Key != clientId).Value;
                Console.WriteLine($"{oppPos.X} {oppPos.Y}");
                if (oppPos.X > 70 && oppPos.X < 1145) {
                    Console.WriteLine("interp");
                    Environment.Instance.Actors["opponent"].Position =
                        Environment.Instance.Actors["opponent"].Position.Interpolated(new Vector2 { X = oppPos.X, Y = oppPos.Y });
                } else {
                    Console.WriteLine("no interp");
                    Environment.Instance.Actors["opponent"].Position = new Vector2 { X = oppPos.X, Y = oppPos.Y };
                }
            }
        }
    }
}