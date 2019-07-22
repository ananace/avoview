using System;
using System.IO;
using BinaryEncoding;

namespace AvoCommLib
{
    namespace Util
    {
        public class BigEndianWriter : BinaryWriter
        {
            public BigEndianWriter(Stream stream) : base(stream)
            {
            }

            public override void Write(Int16 data)
            {
                Write(Binary.BigEndian.GetBytes(data));
            }
            public override void Write(Int32 data)
            {
                Write(Binary.BigEndian.GetBytes(data));
            }
            public override void Write(Int64 data)
            {
                Write(Binary.BigEndian.GetBytes(data));
            }
            public override void Write(UInt16 data)
            {
                Write(Binary.BigEndian.GetBytes(data));
            }
            public override void Write(UInt32 data)
            {
                Write(Binary.BigEndian.GetBytes(data));
            }
            public override void Write(UInt64 data)
            {
                Write(Binary.BigEndian.GetBytes(data));
            }
        }
    }
}
