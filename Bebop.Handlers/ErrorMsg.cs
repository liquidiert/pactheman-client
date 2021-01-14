using Bebop.Attributes;
using Bebop.Runtime;
using PacTheMan.Models;
using System;

namespace pactheman_client {

    [RecordHandler]
    public static class ErrorMsgHandler {

        [BindRecord(typeof(BebopRecord<ErrorMsg>))]
        public static void HandleErrorMsg(object client, ErrorMsg msg) {
            HumanPlayer player = (HumanPlayer) client;
            
            Console.WriteLine($"received error {msg.ErrorMessage} {new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds()}");
            
            // TODO: handle specific errors
        }
    }
}