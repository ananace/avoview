using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AvoCommLib.Util;
using Lextm.SharpSnmpLib;

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

                using (var stream = new MemoryStream(packet))
                using (var write = new BigEndianWriter(stream))
                {
                    write.Write((byte)1);
                    write.Write("AIDP".ToCharArray());
                    write.Write((UInt16)sequence);
                    write.Write((byte)CommandID);
                    write.Write((UInt32)CommandData.Length);
                    write.Write(CommandData);
                    write.Write((byte)13);
                }

                /*
                Console.WriteLine($"Writing {packet.Length}B data:");
                Console.WriteLine(String.Join(" ", packet.Select((b) => b.ToString("X2"))));
                */

                // TODO: Use sequence ID for in-flight packets
                var written = await WriteData(packet);
                if (written != packet.Length)
                {
                    throw new Exception("Failed to send request");
                }

                var data = await ReadData();

                /*
                Console.WriteLine($"Read {data.Length}B data:");
                Console.WriteLine(String.Join(" ", data.Select((b) => b.ToString("X2"))));
                */

                byte[] responseBytes;
                using (var stream = new MemoryStream(data))
                using (var read = new BigEndianReader(stream))
                {
                    if (read.ReadByte() != 1)
                        throw new Exception("Invalid response (no SOH)");
                    if (read.ReadUInt32() != 0x41494450)
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

            public async Task<Variable> SnmpGet(ObjectIdentifier oid)
            {
                return await SnmpGet(new Variable(oid));
            }

            public async Task<Variable> SnmpGet(Variable varBind)
            {
                var ret = await SnmpRequest(new List<Variable>(new[] { varBind }), 16);
                return ret.First();
            }

            public async Task<Variable> SnmpGetNext(ObjectIdentifier oid)
            {
                return await SnmpGet(new Variable(oid));
            }

            public async Task<Variable> SnmpGetNext(Variable varBind)
            {
                var ret = await SnmpRequest(new List<Variable>(new[] { varBind }), 17);
                return ret.First();
            }

            public async Task<List<Variable>> SnmpGet(List<Variable> varBindList)
            {
                return await SnmpRequest(varBindList, 16);
            }

            public async Task<List<Variable>> SnmpGetNext(List<Variable> varBindList)
            {
                return await SnmpRequest(varBindList, 17);
            }

            public async Task<List<Variable>> SnmpRequest(List<Variable> varBindList, int method)
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
