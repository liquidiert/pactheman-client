using Bebop.Attributes;
using Bebop.Runtime;
using PacTheMan.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace pactheman_client {

    [RecordHandler]
    public static class PlayerStateHandler {

        [BindRecord(typeof(BebopRecord<PlayerState>))]
        public static void HandlePlayerStateMsg(object client, PlayerState msg) {
            HumanPlayer player = (HumanPlayer) client;

            player.InternalPlayerState.PlayerPositions = msg.PlayerPositions;
            player.InternalPlayerState.Lives = msg.Lives;
            player.InternalPlayerState.Scores = msg.Scores;

            var clientId = player.InternalPlayerState.Session.ClientId;
            if (msg.Session.ClientId != clientId) {
                (GameEnv.Instance.Actors["opponent"] as Player).CurrentMovingState = msg.Direction;
                var oppPos = new Dictionary<Guid, BasePosition>(player.InternalPlayerState.PlayerPositions).First(p => p.Key != clientId).Value;
                if (oppPos.X > 70 && oppPos.X < 1145) {
                    GameEnv.Instance.Actors["opponent"].Position =
                        GameEnv.Instance.Actors["opponent"].Position.Interpolated(new Vector2 { X = oppPos.X, Y = oppPos.Y });
                } else {
                    GameEnv.Instance.Actors["opponent"].Position = new Vector2 { X = oppPos.X, Y = oppPos.Y };
                }
            }
        }
    }
}