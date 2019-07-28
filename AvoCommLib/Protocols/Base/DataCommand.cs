using System;
using System.IO;
using AvoCommLib.Util;

namespace AvoCommLib
{
    namespace Protocols
    {
        namespace Base
        {
            public abstract class DataCommand : ICommand
            {
                public virtual byte CommandID { get; set; }
                public virtual byte[] CommandData { get; set; }

                public abstract string Header { get; }

                public virtual byte[] ToByteArray()
                {
                    MemoryStream ms = new MemoryStream();

                    using (var write = new BigEndianWriter(ms))
                    {
                        write.Write((byte)1);
                        write.Write(Header.ToCharArray());
                        write.Write((byte)CommandID);

                        write.Write((Int32)CommandData.Length);
                        write.Write(CommandData);

                        write.Write((byte)13);
                    }

                    var bytes = ms.ToArray();
                    ms.Dispose();

                    return bytes;
                }
            }
        }
    }
}