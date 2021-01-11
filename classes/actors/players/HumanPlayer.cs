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
        private NonDisposableStream _stream;
        public PlayerState InternalPlayerState;

        private CancellationTokenSource _ctSource;
        private CancellationToken _ct;
        public bool Connected = false;

        public HumanPlayer(ContentManager content, string name) : base(content, name, "sprites/player/spriteFactory.sf") {
            this.StatsPosition = new Vector2(-350, 50);
            this.InternalPlayerState = new PlayerState();
            Name = name;
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
            _stream = new NonDisposableStream(_client.Client);

            // start listener as seperate thread
            new Thread(() => Listen()).Start();
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

                this._stream = new NonDisposableStream(_client.Client);

                await _stream.WriteAsync(netMessage.Encode());
            } catch (Exception ex) {
                Console.WriteLine(ex);
                Disconnect();
            }
        }

        public void Disconnect() {
            Console.WriteLine("Disconnect got called");
            if (Connected) {
                _stream.YouMayDispose = true;
                _ctSource.Cancel();
                _stream.Close();
                _stream.Dispose();
                _client.Dispose();
                _ctSource.Dispose();
            }
            Connected = false;
        }

        public async Task Exit() {
            Console.WriteLine("called exit");
            var exitMsg = new ExitMsg {
                Session = InternalPlayerState.Session
            };
            var netMsg = new NetworkMessage {
                IncomingOpCode = ExitMsg.OpCode,
                IncomingRecord = exitMsg.EncodeAsImmutable()
            };
            if (_client?.GetState() == TcpState.Established) {
                await _stream.WriteAsync(netMsg.Encode());
            }
            Disconnect();
        }

        private async void Listen() {
            // Were we already canceled?
            _ct.ThrowIfCancellationRequested();

            Byte[] buffer = new Byte[2048];

            try {
                while (true) {
                    if (_ct.IsCancellationRequested) {
                        _ct.ThrowIfCancellationRequested();
                    }

                    if (await _stream.ReadAsync(buffer, _ct) == 0) {
                        // server closed session
                        this.Disconnect();
                        GameState.Instance.CurrentGameState = GameStates.MainMenu;
                        UIState.Instance.CurrentUIState = UIStates.MainMenu;
                        UIState.Instance.CurrentScreen = UIState.Instance.MainMenu;
                        return;
                    }

                    var msg = NetworkMessage.Decode(buffer);
                    BebopMirror.HandleRecord(msg.IncomingRecord.ToArray(), msg.IncomingOpCode ?? 0, this);
                }
            } catch (OperationCanceledException) {
                // swallow -> canceled thread
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
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
                PlayerName = Name
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
                PlayerName = Name,
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
            if (updatedPosition.X <= 38 || updatedPosition.X >= 1177) {
                Position = UpdatePosition(x: -1215, xFactor: -1);
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
                InternalPlayerState.Direction = CurrentMovingState;
                InternalPlayerState.PlayerPositions[(Guid)InternalPlayerState.Session.ClientId] = new Position { X = Position.X, Y = Position.Y };
                var msg = new NetworkMessage {
                    IncomingOpCode = PlayerState.OpCode,
                    IncomingRecord = InternalPlayerState.EncodeAsImmutable()
                };

                try {
                    if (_ct.IsCancellationRequested) {
                        _ct.ThrowIfCancellationRequested();
                    }

                    await _stream.WriteAsync(msg.Encode());
                } catch (ObjectDisposedException) {
                    // swallow -> server sent exit
                } catch (OperationCanceledException) {
                    // swallow -> thread canceled
                }
                
            }
        }
    }
}