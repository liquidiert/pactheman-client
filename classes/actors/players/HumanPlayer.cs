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

        public Guid ClientId { get; set; }
        public Guid SessionId { get; set; }
        private TcpClient client;
        private Task listener;
        public PlayerState PlayerState;

        public HumanPlayer(ContentManager content, string name) : base(content, name, "sprites/player/spriteFactory.sf") {
            this.StatsPosition = new Vector2(-350, 50);
        }

        public async Task Connect() {
            IPAddress address;
            if (!IPAddress.TryParse(ConfigReader.Instance.config["general"]["server_ip"], out address)) {
                Console.WriteLine("Invalid ip address in config");
            }
            int port;
            if (!int.TryParse(ConfigReader.Instance.config["general"]["server_port"], out port)) {
                Console.WriteLine("Invalid port in config");
            }
            this.client = new TcpClient();
            await client.ConnectAsync(address, port);
            listener = Listen();

            // send join
            var joinMsg = new JoinMsg {
                Algorithms = new GhostAlgorithms {
                    Blinky = ConfigReader.Instance.CurrentMoveBehavior("blinky"),
                    Clyde = ConfigReader.Instance.CurrentMoveBehavior("clyde"),
                    Inky = ConfigReader.Instance.CurrentMoveBehavior("inky"),
                    Pinky = ConfigReader.Instance.CurrentMoveBehavior("pinky")
                },
                PlayerName = "PlayerOne"
            };
            var netMsg = new NetworkMessage {
                IncomingOpCode = JoinMsg.OpCode,
                IncomingRecord = joinMsg.EncodeAsImmutable()
            };

            await client.GetStream().WriteAsync(netMsg.Encode());
        }

        private async Task Listen() {
            Byte[] buffer = new Byte[4096];

            try {
                while (true) {
                    Console.WriteLine("waiting for msg...");
                    await client.GetStream().ReadAsync(buffer);
                    var msg = NetworkMessage.Decode(buffer);
                    BebopMirror.HandleRecord(msg.IncomingRecord.ToArray(), msg.IncomingOpCode ?? 0, this);
                }
            } catch (SocketException ex) {
                Console.WriteLine("Lost connection to server: " + ex);
            } finally {
                client.Dispose();
            }
        }

        public async Task SetReady() {
            var rdyMsg = new ReadyMsg {
                Session = new SessionMsg {
                    SessionId = this.SessionId,
                    ClientId = this.ClientId
                },
                Ready = true
            };
            var netMsg = new NetworkMessage {
                IncomingOpCode = ReadyMsg.OpCode,
                IncomingRecord = rdyMsg.EncodeAsImmutable()
            };
            await client.GetStream().WriteAsync(netMsg.Encode());
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
                    PlayerState.Score[ClientId] += 10;
                }
            }

            if (Environment.Instance.IsOnline) {
                PlayerState.ReconciliationId++;
                PlayerState.PlayerPositions[ClientId] = new Position { X = (int)DownScaledPosition.X, Y = (int)DownScaledPosition.Y };
                var msg = new NetworkMessage {
                    IncomingOpCode = PlayerState.OpCode,
                    IncomingRecord = PlayerState.EncodeAsImmutable()
                };
                client.GetStream().WriteAsync(msg.Encode());
            }
        }
    }
}