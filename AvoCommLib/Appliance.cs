using System;
using System.IO;
using System.Net;
using AvoCommLib.Enums;

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

            aidp.Request(0x01, new byte[] { 0xFF })
                .ContinueWith((Data) => ParseDiscoveryData(Data.Result));
        }

        void ParseDiscoveryData(byte[] Data)
        {
            byte fieldId;
            byte[] lenBytes = new byte[2];
            ushort fieldLength;
            using (MemoryStream stream = new MemoryStream(Data))
            using (BinaryReader read = new BinaryReader(stream))
                while (read.PeekChar() >= 0)
                {
                    fieldId = read.ReadByte();
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
                                Console.WriteLine($"MAC Address: {String.Join(":", MacAddress)}");
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
                                var stringLength = read.ReadUInt16();
                                Hostname = new String(read.ReadChars(stringLength));
                                Console.WriteLine($"Hostname: {Hostname}");
                            }
                            break;

                        case 7:
                            {
                                var flags = read.ReadByte();
                                Console.WriteLine($"Flags: {flags.ToString("X")}");
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
