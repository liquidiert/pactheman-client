using Bebop.Attributes;
using Bebop.Runtime;
using PacTheMan.Models;
using System;

namespace pactheman_client {

    [RecordHandler]
    public static class SessionMsgHandler {

        [BindRecord(typeof(BebopRecord<SessionMsg>))]
        public static void HandleSessionMsg(object client, SessionMsg msg) {
            HumanPlayer player = (HumanPlayer) client;

            player.InternalPlayerState.Session = msg;

            Console.WriteLine(player.InternalPlayerState.Session.ClientId);
            Console.WriteLine(player.InternalPlayerState.Session.SessionId);

            UIState.Instance.CurrentUIState = UIStates.PreLobby;
            UIState.Instance.CurrentScreen = new PreLobby(player.InternalPlayerState.Session.SessionId.ToString());
        }
    }
}