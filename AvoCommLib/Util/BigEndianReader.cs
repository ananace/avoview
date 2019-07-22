using System;
using System.IO;
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
        }
    }
}
