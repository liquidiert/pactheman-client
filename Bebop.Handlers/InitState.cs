using Bebop.Attributes;
using Bebop.Runtime;
using PacTheMan.Models;

namespace pactheman_client {

    [RecordHandler]
    public static class InitStateHandler {

        [BindRecord(typeof(BebopRecord<InitState>))]
        public static void HandleInitStateMsg(object client, InitState msg) {
            HumanPlayer player = (HumanPlayer) client;

            player.PlayerState.ReconciliationId = msg.StartReconciliationId[player.ClientId];
            player.PlayerState.PlayerPositions = msg.PlayerInitPositions;
            player.PlayerState.GhostPositions = msg.GhostInitPositions;
            player.PlayerState.Lives = msg.PlayerInitLives;
            player.PlayerState.Score = msg.PlayerInitScores;

            UIState.Instance.CurrentUIState = UIStates.Game;
            GameState.Instance.CurrentGameState = GameStates.Game;
            UIState.Instance.CurrentScreen = new InGameMenu();
        }
    }
}