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

            player.InternalPlayerState.PlayerPositions = msg.PlayerInitPositions;
            player.InternalPlayerState.Lives = msg.PlayerInitLives;
            player.InternalPlayerState.Scores = msg.PlayerInitScores;
            
            foreach (var ghost in msg.GhostInitPositions) {
                (GameEnv.Instance.Actors[ghost.Key] as Ghost).LastTarget = GameEnv.Instance.Actors[ghost.Key].Position = 
                    GameEnv.Instance.Actors[ghost.Key].StartPosition = 
                        new Vector2 { X = ghost.Value.X, Y = ghost.Value.Y };
            }

            player.Position = player.StartPosition = (msg.PlayerInitPositions[(Guid)player.InternalPlayerState.Session.ClientId] as Position).ToVec2();
            var oppInitPos = msg.PlayerInitPositions.First(p => p.Key != (Guid)player.InternalPlayerState.Session.ClientId).Value;
            GameEnv.Instance.Actors["opponent"].Position = GameEnv.Instance.Actors["opponent"].StartPosition = (oppInitPos as Position).ToVec2();

            // remove position score points
            GameEnv.Instance.RemoveScorePoint(player.Position);
            GameEnv.Instance.RemoveScorePoint(GameEnv.Instance.Actors["opponent"].Position);

            UIState.Instance.CurrentUIState = UIStates.Game;
            GameState.Instance.CurrentGameState = GameStates.Game;
            UIState.Instance.CurrentScreen = new InGameMenu();
            UIState.Instance.GuiSystem.ActiveScreen.Hide();
        }
    }
}