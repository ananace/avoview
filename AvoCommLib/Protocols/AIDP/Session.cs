using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AvoCommLib.Protocols.Base;
using AvoCommLib.Util;

namespace AvoCommLib
{
    namespace Protocols
    {
        namespace AIDP
        {
            public class Session : IDisposable
            {
                static ushort _sequenceCounter = 0;

                private bool _isDisposed = false;
                readonly UdpClient _udpSocket = new UdpClient(0);

                readonly Dictionary<ushort, TaskCompletionSource<BaseCommand>> _requestsInFlight = new Dictionary<ushort, TaskCompletionSource<BaseCommand>>();

                public IPEndPoint Target { get; set; } = new IPEndPoint(IPAddress.Any, 3211);

                public Session()
                {
                    _udpSocket.BeginReceive(MessageReceived, null);
                }

                public Session(bool Broadcast, IPAddress Target = null) : this()
                {
                    if (!Broadcast)
                        throw new ArgumentException("Broadcast must be true", nameof(Broadcast));

                    if (Target == null)
                        Target = IPAddress.Broadcast;

                    _udpSocket.EnableBroadcast = true;
                }

                public Session(IPEndPoint Target) : this()
                {
                    this.Target = Target;
                }

                protected virtual void Dispose(bool disposing)
                {
                    if (!_isDisposed)
                    {
                        if (disposing)
                        {
                            _udpSocket.Dispose();
                        }

                        _requestsInFlight.Clear();
                        _isDisposed = true;
                    }
                }

                public void Dispose()
                {
                    Dispose(true);
                }

                public async Task<BaseCommand> SendRequest(BaseCommand request)
                {
                    try
                    {
                        var tcs = new TaskCompletionSource<BaseCommand>();
                        using (var cts = new CancellationTokenSource())
                        {
                            cts.CancelAfter(10000);
                            cts.Token.Register(() => tcs.TrySetCanceled());

                            request.CommandSequence = GetSequenceNumber();
                            await Task.Run(() => {
                                lock (_requestsInFlight)
                                    _requestsInFlight[request.CommandSequence] = tcs;
                            });

                            var bytes = request.ToByteArray();
                            await _udpSocket.SendAsync(bytes, bytes.Length, Target);
                        }

                        return await tcs.Task;
                    }
                    finally
                    {
                        // In case the request fails for any reason
                        lock (_requestsInFlight)
                            _requestsInFlight.Remove(request.CommandSequence);
                    }
                }

                public void SendCommand(BaseCommand command)
                {
                    command.CommandSequence = GetSequenceNumber();

                    var bytes = command.ToByteArray();
                    _udpSocket.Send(bytes, bytes.Length, Target);
                }

                public static BaseCommand ParseCommand(Stream stream)
                {
                    BaseCommand cmd;
                    using (var read = new BigEndianReader(stream))
                    {
                        if (read.ReadByte() != 1)
                            throw new Exception("Faulty or missing SOH");
                        if (read.ReadUInt32() != 0x41494450)
                            throw new Exception("Invalid response (wrong signature)");
                        ushort sequence = read.ReadUInt16();
                        byte commandID = read.ReadByte();
                        int responseLength = read.ReadInt32();
                        if (responseLength > 8192)
                            throw new Exception("Invalid response (too big)");

                        var commands = Assembly.GetExecutingAssembly().GetTypes()
                            .Where(t => t.Namespace == "AvoCommLib.Protocols.AIDP" && t.GetCustomAttribute(typeof(CommandAttribute), true) != null);

                        var found = commands.FirstOrDefault(t => (t.GetCustomAttribute(typeof(CommandAttribute)) as CommandAttribute).CommandID == commandID);
                        if (found == null)
                            throw new Exception("Unknown AIDP message received.");

                        cmd = Activator.CreateInstance(found) as BaseCommand;
                        cmd.CommandSequence = sequence;
                        cmd.ReadFromReader(read);

                        if (read.ReadByte() != 13)
                            throw new Exception("Faulty or missing terminator");
                    }

                    return cmd;
                }

                public static ushort GetSequenceNumber()
                {
                    return ++_sequenceCounter;
                }

                void MessageReceived(IAsyncResult result)
                {
                    IPEndPoint receivedIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] receivedBytes = _udpSocket.EndReceive(result, ref receivedIpEndPoint);

                    var cmd = ParseCommand(new MemoryStream(receivedBytes));
                    var type = (CommandTypes)cmd.GetCommandInfo().CommandID;

                    lock(_requestsInFlight)
                    {
                        if (_requestsInFlight.ContainsKey(cmd.CommandSequence))
                        {
                            _requestsInFlight[cmd.CommandSequence].TrySetResult(cmd);
                            _requestsInFlight.Remove(cmd.CommandSequence);
                        }
                        else
                        {
                            Console.WriteLine($"Received untracked response {cmd.CommandSequence} of type {type}.");
                        }
                    }

                    _udpSocket.BeginReceive(MessageReceived, null);
                }
            }
        }
    }
}
