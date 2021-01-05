using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Sockets;
using MonoGame.Extended;
using PacTheMan.Models;
using Bebop.Runtime;

namespace pactheman_client {
    class HumanPlayer : Player {

        private Guid playerId;
        private TcpClient client;
        private Task listener;
        public PlayerState PlayerState;

        public HumanPlayer(ContentManager content, string name) : base(content, name, "sprites/player/spriteFactory.sf") {
            this.StatsPosition = new Vector2(-350, 50);
        }

        public async void Connect() {
            var address = IPAddress.Parse(ConfigReader.Instance.config["server_ip"]);
            var port = ConfigReader.Instance.config["server_port"];
            this.client = new TcpClient();
            await client.ConnectAsync(address, port);
            listener = Listen();
        }

        private async Task Listen() {
            Byte[] buffer = new Byte[4096];

            while (true) {
                await client.GetStream().ReadAsync(buffer);
                var msg = NetworkMessage.Decode(buffer);
                BebopMirror.HandleRecord(msg.IncomingRecord.ToArray(), msg.IncomingOpCode ?? 0, this);
            }
        }

        public override void OnActorCollision(object sender, EventArgs args) {
            _lives--;
            if (!Environment.Instance.IsOnline) {
                if (_lives <= 0) {
                    Environment.Instance.Clear();
                    return;
                }
                Environment.Instance.Reset();
            }
        }

        public override void Move(GameTime gameTime) {
            var delta = gameTime.GetElapsedSeconds();

            Vector2 updatedPosition;

            if (IsHooman) {
                updatedPosition = keyboardMove(delta);
            } else {
                // TODO: ai movement
                updatedPosition = new Vector2();
            }

            // teleport if entering either left or right gate
            if (updatedPosition.X <= 70 || updatedPosition.X >= 1145) {
                Position = UpdatePosition(x: -1216, xFactor: -1);
            } else {
                Position = updatedPosition;
            }

            if (Environment.Instance.RemoveScorePoint(Position)) {
                _score += 10;
                if (Environment.Instance.IsOnline) {
                    PlayerState.Score[playerId] += 10;
                }
            }

            if (Environment.Instance.IsOnline) {
                PlayerState.ReconciliationId++;
                PlayerState.PlayerPositions[playerId] = new Position { X = (int)DownScaledPosition.X, Y = (int)DownScaledPosition.Y };
                var msg = new NetworkMessage {
                    IncomingOpCode = PlayerState.OpCode,
                    IncomingRecord = PlayerState.EncodeAsImmutable()
                };
                client.GetStream().WriteAsync(msg.Encode());
            }
        }
    }
}