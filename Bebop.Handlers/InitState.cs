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
            player.InternalPlayerState.Score = msg.PlayerInitScores;
            
            foreach (var ghost in msg.GhostInitPositions) {
                (GameEnv.Instance.Actors[ghost.Key] as Ghost).LastTarget = GameEnv.Instance.Actors[ghost.Key].Position = 
                    GameEnv.Instance.Actors[ghost.Key].StartPosition = 
                        new Vector2 { X = ghost.Value.X, Y = ghost.Value.Y };
            }

            player.Position = player.StartPosition = new Vector2 {
                X = msg.PlayerInitPositions[(Guid)player.InternalPlayerState.Session.ClientId].X,
                Y = msg.PlayerInitPositions[(Guid)player.InternalPlayerState.Session.ClientId].Y
            };
            var oppInitPos = msg.PlayerInitPositions.First(p => p.Key != (Guid)player.InternalPlayerState.Session.ClientId).Value;
            GameEnv.Instance.Actors["opponent"].Position = GameEnv.Instance.Actors["opponent"].StartPosition = 
                new Vector2 {
                    X = oppInitPos.X,
                    Y = oppInitPos.Y
                };

            // remove position score points
            GameEnv.Instance.RemoveScorePoint(new Vector2 { X = player.Position.X, Y = player.Position.Y });
            GameEnv.Instance.RemoveScorePoint(new Vector2 { 
                X = GameEnv.Instance.Actors["opponent"].Position.X,
                Y = GameEnv.Instance.Actors["opponent"].Position.Y
            });

            UIState.Instance.CurrentUIState = UIStates.Game;
            GameState.Instance.CurrentGameState = GameStates.Game;
            UIState.Instance.CurrentScreen = new InGameMenu();
            UIState.Instance.GuiSystem.ActiveScreen.Hide();
        }
    }
}