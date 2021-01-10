using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Net;
using System.IO;
using System.Net.NetworkInformation;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using MonoGame.Extended;
using PacTheMan.Models;
using Bebop.Runtime;

namespace pactheman_client {
    class HumanPlayer : Player {

        private TcpClient _client;
        private NetworkStream _stream;
        private StreamWriter _writer;
        public PlayerState InternalPlayerState;

        private CancellationTokenSource _ctSource;
        private CancellationToken _ct;
        public bool Connected = false;

        public HumanPlayer(ContentManager content, string name) : base(content, name, "sprites/player/spriteFactory.sf") {
            this.StatsPosition = new Vector2(-350, 50);
            this.InternalPlayerState = new PlayerState();
            InternalPlayerState.Name = name;
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
            _client = new TcpClient();
            await _client.ConnectAsync(address, port);
            _stream = _client.GetStream();
            _writer = new StreamWriter(_stream, leaveOpen: true);
            Task.Run(() => Listen(), _ct);
            Connected = true;
        }

        private async Task _reconnect() {
            try {
                _client = new TcpClient();
                await _client.ConnectAsync(
                    IPAddress.Parse(ConfigReader.Instance.config["general"]["server_ip"]),
                    int.Parse(ConfigReader.Instance.config["general"]["server_port"])
                );

                var netMessage = new NetworkMessage {
                    IncomingOpCode = ReconnectMsg.OpCode,
                    IncomingRecord = new ReconnectMsg {
                        Session = InternalPlayerState.Session
                    }.EncodeAsImmutable()
                };

                this._stream = _client.GetStream();
                this._writer = new StreamWriter(_stream, leaveOpen: true);

                await _stream.WriteAsync(netMessage.Encode());
            } catch (Exception ex) {
                Console.WriteLine(ex);
                Disconnect();
            }
        }

        public void Disconnect() {
            if (Connected) {
                _ctSource.Cancel();
                _stream.Close();
                _stream.Dispose();
                _writer.Dispose();
                _client.Dispose();
                _ctSource.Dispose();
            }
            Connected = false;
        }

        public async Task Exit() {
            var exitMsg = new ExitMsg {
                Session = InternalPlayerState.Session
            };
            var netMsg = new NetworkMessage {
                IncomingOpCode = ExitMsg.OpCode,
                IncomingRecord = exitMsg.EncodeAsImmutable()
            };
            await _writer.WriteLineAsync(netMsg.Encode().ToString());
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
                    if (await _stream.ReadAsync(buffer) == 0) {
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

            await _stream.WriteAsync(netMsg.Encode());
        }

        public async Task Join() {
            // send join
            var joinMsg = new JoinMsg {
                PlayerName = "PlayerOne",
                Session = InternalPlayerState.Session
            };
            var netMsg = new NetworkMessage {
                IncomingOpCode = JoinMsg.OpCode,
                IncomingRecord = joinMsg.EncodeAsImmutable()
            };

            await _stream.WriteAsync(netMsg.Encode());
        }

        public async Task SetReady() {
            var rdyMsg = new ReadyMsg {
                Session = InternalPlayerState.Session,
                Ready = true
            };
            var netMsg = new NetworkMessage {
                IncomingOpCode = ReadyMsg.OpCode,
                IncomingRecord = rdyMsg.EncodeAsImmutable()
            };
            await _stream.WriteAsync(netMsg.Encode());
        }

        public override async void Move(GameTime gameTime) {
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
                    InternalPlayerState.Score[(Guid)InternalPlayerState.Session.ClientId] = _score;
                }
            }

            if (Environment.Instance.IsOnline) {
                InternalPlayerState.ReconciliationId++;
                InternalPlayerState.PlayerPositions[(Guid)InternalPlayerState.Session.ClientId] = new Position { X = (int)DownScaledPosition.X, Y = (int)DownScaledPosition.Y };
                var msg = new NetworkMessage {
                    IncomingOpCode = PlayerState.OpCode,
                    IncomingRecord = InternalPlayerState.EncodeAsImmutable()
                };
                try {
                    await _writer.WriteLineAsync(msg.Encode().ToString());
                } catch (ObjectDisposedException) {
                    await _reconnect();
                    await _writer.WriteLineAsync(msg.Encode().ToString());
                }
            }
        }
    }
}