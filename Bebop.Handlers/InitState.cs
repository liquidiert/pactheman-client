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
            Environment.Instance.Actors["opponent"].Position = Environment.Instance.Actors["opponent"].StartPosition = 
                new Vector2 {
                    X = msg.PlayerInitPositions.Where(p => p.Key != (Guid)player.InternalPlayerState.Session.ClientId).First().Value.X,
                    Y = msg.PlayerInitPositions.Where(p => p.Key != (Guid)player.InternalPlayerState.Session.ClientId).First().Value.Y
                };

            UIState.Instance.CurrentUIState = UIStates.Game;
            GameState.Instance.CurrentGameState = GameStates.Game;
            UIState.Instance.CurrentScreen = new InGameMenu();
        }
    }
}