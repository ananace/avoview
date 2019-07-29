using System.Linq;
using System.Net;
using System.Text;
using AvoCommLib.Util;
using BinaryEncoding;
using Lextm.SharpSnmpLib;

namespace AvoCommLib
{
    namespace Protocols
    {
        namespace Base
        {
            public class CommandField
            {
                public byte FieldID { get; set; }
                public byte[] FieldData { get; set; }

                public CommandField(byte ID)
                {
                    FieldID = ID;
                }

                public CommandField(byte ID, byte[] Data)
                {
                    FieldID = ID;
                    FieldData = Data;
                }

                public CommandField(byte ID, byte Data)
                {
                    FieldID = ID;
                    FieldData = new[] { Data };
                }

                public CommandField(byte ID, short Data)
                {
                    FieldID = ID;
                    FieldData = Binary.BigEndian.GetBytes(Data);
                }

                public CommandField(byte ID, ushort Data)
                {
                    FieldID = ID;
                    FieldData = Binary.BigEndian.GetBytes(Data);
                }

                public CommandField(byte ID, int Data)
                {
                    FieldID = ID;
                    FieldData = Binary.BigEndian.GetBytes(Data);
                }

                public CommandField(byte ID, uint Data)
                {
                    FieldID = ID;
                    FieldData = Binary.BigEndian.GetBytes(Data);
                }

                public CommandField(byte ID, long Data)
                {
                    FieldID = ID;
                    FieldData = Binary.BigEndian.GetBytes(Data);
                }

                public CommandField(byte ID, ulong Data)
                {
                    FieldID = ID;
                    FieldData = Binary.BigEndian.GetBytes(Data);
                }

                public CommandField(byte ID, IPAddress Data)
                {
                    FieldID = ID;
                    FieldData = Data.GetAddressBytes();
                }

                public CommandField(byte ID, string Data)
                {
                    FieldID = ID;
                    FieldData = Encoding.GetEncoding("UTF-8").GetBytes(Data);
                }

                public CommandField(byte ID, Variable Data)
                {
                    FieldID = ID;
                    FieldData = SNMP.EncodeVarBind(Data);
                }

                public byte AsByte()
                {
                    return FieldData.First();
                }

                public ushort AsUInt16()
                {
                    return Binary.BigEndian.GetUInt16(FieldData);
                }

                public short AsInt16()
                {
                    return Binary.BigEndian.GetInt16(FieldData);
                }

                public uint AsUInt32()
                {
                    return Binary.BigEndian.GetUInt32(FieldData);
                }

                public int AsInt32()
                {
                    return Binary.BigEndian.GetInt32(FieldData);
                }

                public ulong AsUInt64()
                {
                    return Binary.BigEndian.GetUInt64(FieldData);
                }

                public long AsInt64()
                {
                    return Binary.BigEndian.GetInt64(FieldData);
                }

                public IPAddress AsIPAddress()
                {
                    return new IPAddress(FieldData);
                }

                public string AsString()
                {
                    return Encoding.GetEncoding("UTF-8").GetString(FieldData);
                }

                public Variable AsVarBind()
                {
                    return SNMP.DecodeVarBind(FieldData);
                }
            }
        }
    }
}