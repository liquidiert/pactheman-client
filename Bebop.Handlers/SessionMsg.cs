using Bebop.Attributes;
using Bebop.Runtime;
using PacTheMan.Models;
using System;

namespace pactheman_client {

    [RecordHandler]
    public static class SessionMsgHandler {

        [BindRecord(typeof(BebopRecord<SessionMsg>))]
        public static void HandleGhostMove(object client, SessionMsg msg) {
            HumanPlayer player = (HumanPlayer) client;

            player.ClientId = (Guid) msg.ClientId;
            player.SessionId = (Guid) msg.SessionId;

            Console.WriteLine(player.ClientId);
            Console.WriteLine(player.SessionId);
        }
    }
}