using System;
using System.Collections.Generic;
using System.IO;
using AvoCommLib.Util;

namespace AvoCommLib
{
    namespace Protocols
    {
        namespace Base
        {
            public abstract class Command : ICommand
            {
                public virtual byte CommandID { get; set; }
                public abstract string Header { get; }

                public IReadOnlyList<Tuple<byte,byte[]>> Fields { get { return _Data; } }
                readonly List<Tuple<byte, byte[]>> _Data = new List<Tuple<byte, byte[]>>();

                public void AddField(byte FieldID, byte[] FieldData)
                {
                    _Data.Add(new Tuple<byte,byte[]>(FieldID, FieldData));
                }

                public byte[] CommandData {
                    get {
                        MemoryStream ms = new MemoryStream();

                        using (var write = new BigEndianWriter(ms))
                        {
                            foreach (var field in Fields)
                            {
                                write.Write(field.Item1);
                                write.Write((Int16)field.Item2.Length);
                                write.Write(field.Item2);
                            }

                            if (Fields.Count > 0)
                                write.Write((byte)255);
                        }

                        var bytes = ms.ToArray();
                        ms.Dispose();

                        return bytes;
                    }
                }
            }
        }
    }
}