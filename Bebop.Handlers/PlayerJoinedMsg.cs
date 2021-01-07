using Bebop.Attributes;
using Bebop.Runtime;
using PacTheMan.Models;

namespace pactheman_client {

    [RecordHandler]
    public static class PlayerJoinedHandler {

        [BindRecord(typeof(BebopRecord<PlayerJoinedMsg>))]
        public static void HandleGhostMove(object client, PlayerJoinedMsg msg) {
            HumanPlayer player = (HumanPlayer) client;

            // TODO: show in lobby
        }
    }
}