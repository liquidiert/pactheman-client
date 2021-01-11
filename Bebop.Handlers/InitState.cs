using Bebop.Attributes;
using Bebop.Runtime;
using PacTheMan.Models;
using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace pactheman_client {

    [RecordHandler]
    public static class InitStateHandler {

        [BindRecord(typeof(BebopRecord<InitState>))]
        public static void HandleInitStateMsg(object client, InitState msg) {
            HumanPlayer player = (HumanPlayer) client;

            player.InternalPlayerState.ReconciliationId = msg.StartReconciliationId[(Guid)player.InternalPlayerState.Session.ClientId];
            player.InternalPlayerState.PlayerPositions = msg.PlayerInitPositions;
            player.InternalPlayerState.GhostPositions = msg.GhostInitPositions;
            player.InternalPlayerState.Lives = msg.PlayerInitLives;
            player.InternalPlayerState.Score = msg.PlayerInitScores;
            // TODO: set ghost positions
            player.Position = player.StartPosition = new Vector2 {
                X = msg.PlayerInitPositions[(Guid)player.InternalPlayerState.Session.ClientId].X,
                Y = msg.PlayerInitPositions[(Guid)player.InternalPlayerState.Session.ClientId].Y
            };
            var oppInitPos = msg.PlayerInitPositions.First(p => p.Key != (Guid)player.InternalPlayerState.Session.ClientId).Value;
            Environment.Instance.Actors["opponent"].Position = Environment.Instance.Actors["opponent"].StartPosition = 
                new Vector2 {
                    X = oppInitPos.X,
                    Y = oppInitPos.Y
                };

            // remove position score points
            Environment.Instance.RemoveScorePoint(new Vector2 { X = player.Position.X, Y = player.Position.Y });
            Environment.Instance.RemoveScorePoint(new Vector2 { 
                X = Environment.Instance.Actors["opponent"].Position.X,
                Y = Environment.Instance.Actors["opponent"].Position.Y
            });

            UIState.Instance.CurrentUIState = UIStates.Game;
            GameState.Instance.CurrentGameState = GameStates.Game;
            UIState.Instance.CurrentScreen = new InGameMenu();
        }
    }
}