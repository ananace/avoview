using System;
using System.IO;
using SnmpSharpNet;

namespace AvoCommLib
{
    namespace Util
    {
        public static class SNMP
        {
            public enum FieldType
            {
                Int32 = 2,
                String = 4,
                OID = 6,
                IPAddress = 64,
                Counter32 = 65,
                Gauge32 = 66,
                TimeTicks = 67
            }

            public static Vb DecodeVarBind(byte[] Data)
            {
                Vb ret = new Vb();

                using (var stream = new MemoryStream(Data))
                using (var read = new BigEndianReader(stream))
                {
                    var nameType = (FieldType)read.ReadByte();
                    if (nameType != FieldType.OID)
                        throw new Exception("Invalid OID type for varbind");

                    var nameLen = read.ReadByte();
                    var oidIntCount = nameLen / sizeof(Int32);
                    var oidInts = new Int32[oidIntCount];
                    for (int i = 0; i < oidIntCount; ++i)
                        oidInts[i] = read.ReadInt32();

                    ret.Oid = new Oid(oidInts);

                    var valueType = (FieldType)read.ReadByte();
                    var valueLen = read.ReadUInt16();

                    switch (valueType)
                    {
                        default: ret.Value = null; break;
                        case FieldType.Int32: ret.Value = new Integer32(read.ReadInt32()); break;
                        case FieldType.String: ret.Value = new OctetString(read.ReadBytes(valueLen)); break;
                        case FieldType.OID:
                            oidIntCount = valueLen / sizeof(Int32);
                            oidInts = new Int32[oidIntCount];
                            for (int i = 0; i < oidIntCount; ++i)
                                oidInts[i] = read.ReadInt32();
                            ret.Value = new Oid(oidInts);
                            break;
                        case FieldType.IPAddress:
                            if (valueLen != 4)
                                throw new Exception("Invalid IP Address length");
                            ret.Value = new IpAddress(read.ReadBytes(4));
                            break;
                        case FieldType.Counter32: ret.Value = new Counter32((UInt32)read.ReadInt32()); break;
                        case FieldType.Gauge32: ret.Value = new Gauge32((UInt32)read.ReadInt32()); break;
                        case FieldType.TimeTicks: ret.Value = new TimeTicks((UInt32)read.ReadInt32()); break;
                    }
                }

                return ret;
            }

            public static byte[] EncodeVarBind(Vb vb)
            {
                var stream = new MemoryStream();

                using (var write = new BigEndianWriter(stream))
                {
                    write.Write((byte)FieldType.OID);
                    write.Write((Int32)vb.Oid.Length * sizeof(Int32));
                    foreach (var part in vb.Oid)
                        write.Write((Int32)part);
                    write.Write((byte)vb.Type);
                    switch ((FieldType)vb.Type)
                    {
                        default: write.Write((Int16)0); break;
                        case FieldType.Int32: write.Write((Int32)(vb.Value as Integer32).Value); break;
                        case FieldType.String: write.Write((vb.Value as OctetString).ToArray()); break;
                        case FieldType.OID:
                            foreach (var part in (vb.Value as Oid))
                                write.Write((Int32)part);
                            break;
                        case FieldType.IPAddress: write.Write((vb.Value as IpAddress).ToArray()); break;
                        case FieldType.Counter32: write.Write((Int32)(vb.Value as Counter32).Value); break;
                        case FieldType.Gauge32: write.Write((Int32)(vb.Value as Counter32).Value); break;
                        case FieldType.TimeTicks: write.Write((Int32)(vb.Value as Counter32).Value); break;
                    }
                }

                var buf = stream.ToArray();
                stream.Dispose();

                return buf;
            }

            public static VbCollection DecodeVarBindList(byte[] Data)
            {
                VbCollection ret = new VbCollection();

                using (var stream = new MemoryStream(Data))
                using (var read = new BigEndianReader(stream))
                {
                    while (true)
                    {
                        byte fieldID = read.ReadByte();
                        ushort fieldLength = read.ReadUInt16();

                        switch (fieldID)
                        {
                            case 1: // Error status
                                {
                                    var errorStatus = read.ReadUInt16();
                                }
                                break;

                            case 2: // Error type
                                {
                                    var errorType = read.ReadUInt16();
                                }
                                break;

                            case 3: // VarBind
                                {
                                    var vb = DecodeVarBind(read.ReadBytes(fieldLength));
                                    ret.Add(vb);
                                }
                                break;
                        }

                        if (fieldID == 255 || read.PeekChar() < 0)
                            break;
                    }
                }

                return ret;
            }

            public static byte[] EncodeVarBindList(VbCollection VarBindList)
            {
                MemoryStream stream = new MemoryStream();

                using (var write = new BigEndianWriter(stream))
                {
                    foreach (var vb in VarBindList)
                    {
                        var bytes = EncodeVarBind(vb);
                        write.Write((byte)1);
                        write.Write((Int16)bytes.Length);
                        write.Write(bytes);
                    }
                }

                var data = stream.ToArray();
                stream.Dispose();

                return data;
            }
        }
    }
}
