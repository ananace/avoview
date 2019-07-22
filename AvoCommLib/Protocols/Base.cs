using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AvoCommLib.Util;
using SnmpSharpNet;

namespace AvoCommLib
{
    namespace Protocols
    {
        public abstract class Base
        {
            const int HEADER_SIZE = 13;

            ushort _sequenceID = 0;

            protected abstract Task<int> WriteData(byte[] data);
            protected abstract Task<byte[]> ReadData();

            public async Task<byte[]> Request(byte CommandID, byte[] CommandData)
            {
                var packet = new byte[CommandData.Length + HEADER_SIZE];

                var sequence = NextSequenceID();
                byte[] seqBytes = BitConverter.GetBytes((UInt16)sequence);
                byte[] lenBytes = BitConverter.GetBytes((UInt32)CommandData.Length);

                using (MemoryStream stream = new MemoryStream(packet))
                using (BinaryWriter write = new BinaryWriter(stream))
                {
                    write.Write((byte)1);
                    write.Write("AIDP".ToCharArray());
                    write.Write((UInt16)sequence);
                    write.Write((byte)CommandID);
                    write.Write((UInt32)CommandData.Length);
                    write.Write(CommandData);
                    write.Write((byte)13);
                }

                // TODO: Use sequence ID for in-flight packets
                var written = await WriteData(packet);
                if (written != packet.Length)
                {
                    throw new Exception("Failed to send request");
                }

                var data = await ReadData();
                byte[] responseBytes;
                using (MemoryStream stream = new MemoryStream(data))
                using (BinaryReader read = new BinaryReader(stream))
                {
                    if (read.ReadByte() != 1)
                        throw new Exception("Invalid response (no SOH)");
                    if (read.ReadChars(4).ToString() != "AIDP")
                        throw new Exception("Invalid response (wrong signature)");
                    if (read.ReadUInt16() != sequence)
                        throw new Exception("Invalid response (wrong sequence ID)");
                    byte command = read.ReadByte();
                    uint respLen = read.ReadUInt32();
                    if (respLen > 8192)
                        throw new Exception("Invalid response (too big)");

                    responseBytes = read.ReadBytes((int)respLen);

                    /*
                    if (stream.ReadByte() != 13)
                        throw new Exception("Invalid response (no terminator)");
                    */
                }

                return responseBytes;
            }

            public async Task<Vb> SnmpGet(Vb varBind)
            {
                var ret = await snmpRequest(new VbCollection(new[] { varBind }), 16);
                return ret.First();
            }

            public async Task<Vb> SnmpGetNext(Vb varBind)
            {
                var ret = await snmpRequest(new VbCollection(new[] { varBind }), 17);
                return ret.First();
            }

            public async Task<VbCollection> SnmpGet(VbCollection varBindList)
            {
                return await snmpRequest(varBindList, 16);
            }

            public async Task<VbCollection> SnmpGetNext(VbCollection varBindList)
            {
                return await snmpRequest(varBindList, 17);
            }

            public async Task<VbCollection> snmpRequest(VbCollection varBindList, int method)
            {
                if (method != 16 && method != 17)
                    throw new ArgumentException("Method must be one of 16, 17", nameof(method));

                var response = await Request((byte)method, SNMP.EncodeVarBindList(varBindList));
                var ret = SNMP.DecodeVarBindList(response);

                return ret;
            }

            private ushort NextSequenceID()
            {
                if (_sequenceID == 65535)
                    _sequenceID = 0;
                return ++_sequenceID;
            }
        }
    }
}
