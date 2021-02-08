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
            player.Score = (int)player.InternalPlayerState.Scores[(Guid)player.InternalPlayerState.Session.ClientId];
            player.InternalPlayerState.ScorePositions = msg.ScorePositions;
            GameEnv.Instance.ScorePointPositions = msg.ScorePositions.Select(p => new ScorePoint((p as Position).ToVec2())).ToList();

            var clientId = player.InternalPlayerState.Session.ClientId;
            if (msg.Session.ClientId != clientId) {
                var opp = (Player)GameEnv.Instance.Actors["opponent"];
                opp.Score = (int)player.InternalPlayerState.Scores[(Guid)msg.Session.ClientId];
                opp.CurrentMovingState = msg.Direction;
                var oppPos = new Dictionary<Guid, BasePosition>(player.InternalPlayerState.PlayerPositions).First(p => p.Key != clientId).Value;
                if (oppPos.X > 70 && oppPos.X < 1145) {
                    opp.Position =
                        opp.Position.Interpolated(new Vector2 { X = oppPos.X, Y = oppPos.Y });
                } else {
                    opp.Position = new Vector2 { X = oppPos.X, Y = oppPos.Y };
                }
            }
        }
    }
}