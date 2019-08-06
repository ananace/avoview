using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using AvoCommLib.Util;
using BinaryEncoding;

namespace AvoCommLib
{
    namespace Util
    {
        public class BigEndianReader : BinaryReader
        {
            public BigEndianReader(Stream stream) : base(stream)
            {
            }

            public override short ReadInt16()
            {
                return Binary.BigEndian.GetInt16(ReadBytes(2));
            }
            public override int ReadInt32()
            {
                return Binary.BigEndian.GetInt32(ReadBytes(4));
            }
            public override long ReadInt64()
            {
                return Binary.BigEndian.GetInt64(ReadBytes(8));
            }
            public override ushort ReadUInt16()
            {
                return Binary.BigEndian.GetUInt16(ReadBytes(2));
            }
            public override uint ReadUInt32()
            {
                return Binary.BigEndian.GetUInt32(ReadBytes(4));
            }
            public override ulong ReadUInt64()
            {
                return Binary.BigEndian.GetUInt64(ReadBytes(8));
            }

            Dictionary<Type, Func<int, object>> _readers;
            Dictionary<Type, Func<int, object>> Readers {
                get
                {
                    if (_readers == null)
                        _readers = new Dictionary<Type, Func<int, object>>{
                            [typeof(byte)] = s => { AssertSize(s, 1); return ReadByte(); },
                            [typeof(char)] = s => ReadChar(),
                            [typeof(UInt16)] = s => ReadUInt16(),
                            [typeof(Int16)] = s => ReadInt16(),
                            [typeof(UInt32)] = s => ReadUInt32(),
                            [typeof(Int32)] = s => ReadInt32(),
                            [typeof(UInt64)] = s => ReadUInt64(),
                            [typeof(Int64)] = s => ReadInt64(),

                            [typeof(byte[])] = s => ReadBytes(s),
                            [typeof(string)] = s => ReadChars(s),
                            [typeof(IPAddress)] = s => new IPAddress(ReadBytes(s)),
                            [typeof(Lextm.SharpSnmpLib.Variable)] = s => SNMP.DecodeVarBind(ReadBytes(s))
                        };
                    return _readers;
                }
            }

            // TODO: Asserts on size
            public object ReadOfType(int size, Type type)
            {
                if (Readers.ContainsKey(type))
                    return Readers[type](size);
                
                throw new ArgumentException("Unable to read data of given type", nameof(type));
            }

            void AssertSize(int size, int required)
            {
                if (size != required)
                    throw new ArgumentException($"Size is not {required}", nameof(size));
            }
        }
    }
}
