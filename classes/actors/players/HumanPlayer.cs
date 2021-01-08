using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Net;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using MonoGame.Extended;
using PacTheMan.Models;
using Bebop.Runtime;

namespace pactheman_client {
    class HumanPlayer : Player {

        public Guid ClientId { get; set; }
        public string SessionId { get; set; }
        private TcpClient client;
        public PlayerState PlayerState;

        private CancellationTokenSource _ctSource;
        private CancellationToken _ct;
        public bool Connected = false;

        public HumanPlayer(ContentManager content, string name) : base(content, name, "sprites/player/spriteFactory.sf") {
            this.StatsPosition = new Vector2(-350, 50);
            this.ID = "playerOne";
            this.PlayerState = new PlayerState();
        }

        public async Task Connect() {
            _ctSource = new CancellationTokenSource();
            _ct = _ctSource.Token;
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
            Task.Run(() => Listen(), _ct);
            Connected = true;
        }

        public void Disconnect() {
            if (Connected) {
                _ctSource.Cancel();
                client.Close();
                client.Dispose();
                _ctSource.Dispose();
            }
            Connected = false;
        }

        public async Task Exit() {
            var exitMsg = new ExitMsg {
                Session = new SessionMsg {
                    SessionId = SessionId,
                    ClientId = ClientId
                }
            };
            var netMsg = new NetworkMessage {
                IncomingOpCode = ExitMsg.OpCode,
                IncomingRecord = exitMsg.EncodeAsImmutable()
            };
            await client.GetStream().WriteAsync(netMsg.Encode());
            Disconnect();
        }

        private async Task Listen() {
            // Were we already canceled?
            _ct.ThrowIfCancellationRequested();

            Byte[] buffer = new Byte[4096];

            try {
                while (true) {
                    if (_ct.IsCancellationRequested) {
                        _ct.ThrowIfCancellationRequested();
                    }

                    Console.WriteLine("waiting for msg...");
                    if (await client.GetStream().ReadAsync(buffer) == 0) {
                        // server closed session
                        this.Disconnect();
                        UIState.Instance.CurrentUIState = UIStates.PreGame;
                        UIState.Instance.CurrentScreen = new PreGameMenu();
                        return;
                    }
                    var msg = NetworkMessage.Decode(buffer);
                    BebopMirror.HandleRecord(msg.IncomingRecord.ToArray(), msg.IncomingOpCode ?? 0, this);
                }
            } catch (SocketException ex) {
                Console.WriteLine("Lost connection to server: " + ex);
            } finally {
                this.Disconnect();
            }
        }

        public async Task Host() {
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

        public async Task Join() {
            // send join
            var joinMsg = new JoinMsg {
                PlayerName = "PlayerOne",
                Session = new SessionMsg {
                    ClientId = this.ClientId,
                    SessionId = this.SessionId
                }
            };
            var netMsg = new NetworkMessage {
                IncomingOpCode = JoinMsg.OpCode,
                IncomingRecord = joinMsg.EncodeAsImmutable()
            };

            await client.GetStream().WriteAsync(netMsg.Encode());
        }

        public async Task SetReady() {
            this.PlayerState.Ready = true;
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