using Bebop.Attributes;
using Bebop.Runtime;
using PacTheMan.Models;
using System;

namespace pactheman_client {

    [RecordHandler]
    public static class ExitMsgHandler {

        [BindRecord(typeof(BebopRecord<ExitMsg>))]
        public static void HandleExitMsg(object client, ExitMsg msg) {
            HumanPlayer player = (HumanPlayer) client;

            Console.WriteLine("received exit msg");
            player.Disconnect();

            GameState.Instance.CurrentGameState = GameStates.MainMenu;
            UIState.Instance.CurrentUIState = UIStates.MainMenu;
            UIState.Instance.CurrentScreen = UIState.Instance.MainMenu;
            Environment.Instance.Clear();
        }
    }
}