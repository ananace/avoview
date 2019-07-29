using System;
using System.Collections.Generic;
using System.IO;
using Lextm.SharpSnmpLib;

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
                Null = 5,
                OID = 6,
                IPAddress = 64,
                Counter32 = 65,
                Gauge32 = 66,
                TimeTicks = 67
            }

            public static Variable DecodeVarBind(byte[] Data)
            {
                using (var stream = new MemoryStream(Data))
                using (var read = new BigEndianReader(stream))
                {
                    var nameType = (FieldType)read.ReadByte();
                    if (nameType != FieldType.OID)
                        throw new Exception("Invalid OID type for varbind");

                    var nameLen = read.ReadInt16();
                    var oidIntCount = nameLen / sizeof(Int32);
                    var oidInts = new UInt32[oidIntCount];
                    for (int i = 0; i < oidIntCount; ++i)
                        oidInts[i] = (UInt32)read.ReadInt32();

                    var oid = new ObjectIdentifier(oidInts);

                    var valueType = (FieldType)read.ReadByte();
                    var valueLen = read.ReadUInt16();

                    switch (valueType)
                    {
                        default: break;

                        case FieldType.Null: return new Variable(oid);
                        case FieldType.Int32: return new Variable(oid, new Integer32(read.ReadInt32()));
                        case FieldType.String: return new Variable(oid, new OctetString(read.ReadBytes(valueLen)));
                        case FieldType.OID:
                            oidIntCount = valueLen / sizeof(Int32);
                            oidInts = new UInt32[oidIntCount];
                            for (int i = 0; i < oidIntCount; ++i)
                                oidInts[i] = (UInt32)read.ReadInt32();
                            return new Variable(oid, new ObjectIdentifier(oidInts));
                        case FieldType.IPAddress:
                            if (valueLen != 4)
                                throw new Exception("Invalid IP Address length");
                            return new Variable(oid, new IP(read.ReadBytes(4)));
                        case FieldType.Counter32: return new Variable(oid, new Counter32((UInt32)read.ReadInt32()));
                        case FieldType.Gauge32: return new Variable(oid, new Gauge32((UInt32)read.ReadInt32()));
                        case FieldType.TimeTicks: return new Variable(oid, new TimeTicks((UInt32)read.ReadInt32()));
                    }
                }

                return null;
            }

            public static byte[] EncodeVarBind(Variable vb)
            {
                var stream = new MemoryStream();

                using (var write = new BigEndianWriter(stream))
                {
                    var idNums = vb.Id.ToNumerical();

                    write.Write((byte)FieldType.OID);
                    write.Write((Int16)(idNums.Length * sizeof(Int32)));
                    foreach (var part in idNums)
                        write.Write((Int32)part);

                    write.Write((byte)vb.Data.TypeCode);
                    switch ((FieldType)vb.Data.TypeCode)
                    {
                        default:
                            write.Write((Int16)0);
                            break;
                        case FieldType.Int32:
                            write.Write((Int16)sizeof(Int32));
                            write.Write((Int32)(vb.Data as Integer32).ToInt32());
                            break;
                        case FieldType.String:
                            write.Write((Int16)(vb.Data as OctetString).ToBytes().Length);
                            write.Write((vb.Data as OctetString).ToBytes());
                            break;
                        case FieldType.OID:
                            write.Write((Int16)((vb.Data as ObjectIdentifier).ToNumerical().Length * sizeof(Int32)));
                            foreach (var part in (vb.Data as ObjectIdentifier).ToNumerical())
                                write.Write((Int32)part);
                            break;
                        case FieldType.IPAddress:
                            write.Write((Int16)(vb.Data as IP).GetRaw().Length);
                            write.Write((vb.Data as IP).GetRaw());
                            break;
                        case FieldType.Counter32:
                            write.Write((Int16)sizeof(Int32));
                            write.Write((Int32)(vb.Data as Counter32).ToUInt32());
                            break;
                        case FieldType.Gauge32:
                            write.Write((Int16)sizeof(Int32));
                            write.Write((Int32)(vb.Data as Gauge32).ToUInt32());
                            break;
                        case FieldType.TimeTicks:
                            write.Write((Int16)sizeof(Int32));
                            write.Write((Int32)(vb.Data as TimeTicks).ToUInt32());
                            break;
                    }
                }

                var buf = stream.ToArray();
                stream.Dispose();

                return buf;
            }

            public static List<Variable> DecodeVarBindList(byte[] Data)
            {
                List<Variable> ret = new List<Variable>();

                using (var stream = new MemoryStream(Data))
                using (var read = new BigEndianReader(stream))
                {
                    while (true)
                    {
                        byte fieldID = read.ReadByte();
                        if (fieldID == 255)
                            break;

                        ushort fieldLength = read.ReadUInt16();
                        switch (fieldID)
                        {
                            case 1: // Error status
                                {
                                    var errorStatus = read.ReadUInt16();
                                    Console.WriteLine($"Error status: {errorStatus}");
                                }
                                break;

                            case 2: // Error
                                {
                                    var errorType = read.ReadUInt16() == 1;
                                    Console.WriteLine($"Error: {errorType}");
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

            public static byte[] EncodeVarBindList(List<Variable> VarBindList)
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

                    write.Write((byte)255);
                }

                var data = stream.ToArray();
                stream.Dispose();

                return data;
            }
        }
    }
}
