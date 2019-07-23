using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace AvoCommLib
{
    namespace Protocols
    {
        public class AIDP : Base
        {
            const int HEADER_SIZE = 13;

            UdpClient _udpSocket = new UdpClient();

            public AIDP(bool multicast)
            {
                if (!multicast)
                    throw new ArgumentException("Multicast must be true for a multicast socket", nameof(multicast));

                _udpSocket.EnableBroadcast = true;
            }

            public AIDP(IPAddress ip, ushort port = 3211) : this(new IPEndPoint(ip, port))
            {
            }

            public AIDP(IPEndPoint endpoint)
            {
                _udpSocket.Connect(endpoint.Address, endpoint.Port);
                // _udpSocket.Client.RemoteEndPoint = endpoint;
            }

            protected override async Task<int> WriteData(byte[] data)
            {
                return await _udpSocket.SendAsync(data, data.Length);
            }

            protected override async Task<byte[]> ReadData()
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(10000);

                return await ReadData(cts.Token);
            }

            private async Task<byte[]> ReadData(CancellationToken token)
            {
                var tcs = new TaskCompletionSource<byte[]>();
                token.Register(() =>
                {
                    tcs.TrySetCanceled();
                });

                var dataTask = _udpSocket.ReceiveAsync();

                var completed = await Task.WhenAny(dataTask, tcs.Task);
                if (completed == dataTask)
                {
                    var data = (await dataTask).Buffer;
                    tcs.TrySetResult(data);
                }

                return await tcs.Task;
            }
        }
    }
}
