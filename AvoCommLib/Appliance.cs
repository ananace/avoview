using System;
using System.IO;
using System.Linq;
using System.Net;
using AvoCommLib.Enums;
using AvoCommLib.Util;

namespace AvoCommLib
{
    public class Appliance
    {
        public string Hostname { get; private set; }
        public Models Model { get; private set; }
        public IPAddress IPAddress { get; set; }
        public IPAddress GatewayAddress { get; private set; }
        public IPAddress SubnetAddress { get; private set; }
        public byte[] MacAddress { get; private set; }
        public int SubnetLength { get; private set; }

        public void Discover()
        {
            var aidp = new Protocols.AIDP(IPAddress);

            var req = aidp.Request(0x01, new byte[] { 0xFF });
            req.Wait();
            ParseDiscoveryData(req.Result);
        }

        public string SystemName
        {
            get
            {
                var aidp = new Protocols.AIDP(IPAddress);
                var ret = aidp.SnmpGet(new SnmpSharpNet.Oid("1.3.6.1.2.1.1.5.0"));

                ret.Wait();

                return ret.Result.Value.ToString();
            }
        }

        void ParseDiscoveryData(byte[] Data)
        {
            byte fieldId;
            byte[] lenBytes = new byte[2];
            ushort fieldLength;
            using (var stream = new MemoryStream(Data))
            using (var read = new BigEndianReader(stream))
                while (read.PeekChar() >= 0)
                {
                    fieldId = read.ReadByte();
                    if (fieldId == 255)
                        break;

                    fieldLength = read.ReadUInt16();

                    switch (fieldId)
                    {
                        case 1:
                            {
                                Model = (Models)read.ReadUInt16();
                                Console.WriteLine($"Model: {Model}");
                            }
                            break;

                        case 2:
                            {
                                MacAddress = read.ReadBytes(6);
                                Console.WriteLine($"MAC Address: {String.Join(":", MacAddress.Select((b) => b.ToString("X2")))}");
                            }
                            break;

                        case 3:
                            {
                                IPAddress = new IPAddress(read.ReadBytes(fieldLength));
                                Console.WriteLine($"IP Address: {IPAddress}");
                            }
                            break;

                        case 4:
                            {
                                SubnetAddress = new IPAddress(read.ReadBytes(fieldLength));
                                Console.WriteLine($"Subnet Address: {SubnetAddress}");
                            }
                            break;

                        case 5:
                            {
                                GatewayAddress = new IPAddress(read.ReadBytes(fieldLength));
                                Console.WriteLine($"Gateway Address: {GatewayAddress}");
                            }
                            break;

                        case 6:
                            {
                                Hostname = new String(read.ReadChars(read.ReadUInt16()));
                                Console.WriteLine($"Hostname: {Hostname}");
                            }
                            break;

                        case 7:
                            {
                                var flags = read.ReadByte();
                                Console.WriteLine($"Flags: {flags.ToString("X2")}");
                            }
                            break;

                        case 255: return;

                        default:
                            {
                                Console.WriteLine($"Unknown field ID {fieldId}");
                                stream.Seek(fieldLength, SeekOrigin.Current);
                            }
                            break;
                    }
                }
        }
    }
}
