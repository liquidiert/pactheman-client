using Bebop.Attributes;
using Bebop.Runtime;
using PacTheMan.Models;
using System;

namespace pactheman_client {

    [RecordHandler]
    public static class PlayerJoinedHandler {

        [BindRecord(typeof(BebopRecord<PlayerJoinedMsg>))]
        public static void HandlePlayerJoinedMsg(object client, PlayerJoinedMsg msg) {
            HumanPlayer player = (HumanPlayer) client;

            if (msg.Session != null) {
                player.SessionId = msg.Session.SessionId;
                player.ClientId = (Guid) msg.Session.ClientId;
            }

            UIState.Instance.CurrentUIState = UIStates.Lobby;
            UIState.Instance.CurrentScreen = new Lobby(msg.PlayerName);
        }
    }
}