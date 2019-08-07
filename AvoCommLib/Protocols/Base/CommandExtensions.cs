using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AvoCommLib.Util;

namespace AvoCommLib
{
    namespace Protocols
    {
        namespace Base
        {
            public static class CommandExtensions
            {
                public static byte[] ToByteArray(this ICommand command)
                {
                    MemoryStream ms = new MemoryStream();

                    using (var write = new BigEndianWriter(ms))
                    {
                        var info = command.GetCommandInfo();

                        write.Write((byte)1);
                        write.Write(info.Header.ToCharArray());
                        if (command.GetType().GetInterface(nameof(ICommandSequence)) != null)
                            write.Write((command as ICommandSequence).CommandSequence);
                        write.Write(info.CommandID);

                        var data = command.GetCommandData();
                        write.Write((Int32)data.Length);
                        write.Write(data);

                        write.Write((byte)13);
                    }

                    var bytes = ms.ToArray();
                    ms.Dispose();

                    return bytes;
                }

                public static byte[] GetCommandData(this ICommand command) {
                    MemoryStream ms = new MemoryStream();

                    using (var write = new BigEndianWriter(ms))
                    {
                        var info = command.GetCommandInfo();
                        var fields = command.GetFields();
                        foreach (var field in fields)
                        {
                            if (field.FieldData == null)
                                continue;

                            write.Write(field.FieldID);
                            write.Write((Int16)field.FieldData.Length);
                            write.Write(field.FieldData);
                        }

                        if (fields.Any() || info.AlwaysAddEOFField)
                            write.Write((byte)255);
                    }

                    var bytes = ms.ToArray();
                    ms.Dispose();

                    return bytes;
                }

                // TODO: Use regular 'object' and have a field creation method to switch between them
                public static IEnumerable<CommandField> GetFields(this ICommand command)
                {
                    var properties = command.GetType().GetProperties()
                        .Where(f => f.GetCustomAttribute<CommandFieldAttribute>() != null);

                    foreach (var property in properties)
                    {
                        var metadata = property.GetCustomAttribute<CommandFieldAttribute>();
                        var serializationType = metadata.SerializeAs;
                        if (serializationType == null)
                            serializationType = property.PropertyType;
                        dynamic data = property.GetValue(command);
                        if (data != null)
                        {
                            if (serializationType != data.GetType())
                                data = Convert.ChangeType(data, serializationType);
                            if (data is IEnumerable)
                            {
                                foreach (var obj in (IEnumerable)data)
                                {
                                    dynamic dobj = obj;
                                    yield return new CommandField(metadata.FieldID, dobj);
                                }
                            }
                            else
                                yield return new CommandField(metadata.FieldID, data);
                        }
                    }
                }

                public static void ReadFromReader(this ICommand command, BigEndianReader read)
                {
                    var properties = command.GetType().GetProperties()
                        .Where(f => f.GetCustomAttribute<CommandFieldAttribute>() != null)
                        .Select(f => new { Property = f, Metadata = f.GetCustomAttribute<CommandFieldAttribute>() });

                    while (true)
                    {
                        byte fieldID = read.ReadByte();
                        if (fieldID == 255 || read.PeekChar() < 0)
                            break;

                        ushort fieldLength = read.ReadUInt16();

                        var fieldInfo = properties.First(f => f.Metadata.FieldID == fieldID);
                        var serializationType = fieldInfo.Property.PropertyType;
                        if (fieldInfo.Metadata.SerializeAs != null)
                            serializationType = fieldInfo.Metadata.SerializeAs;

                        if (Nullable.GetUnderlyingType(serializationType) != null)
                            serializationType = Nullable.GetUnderlyingType(serializationType);

                        if (fieldInfo.Metadata.Repeated)
                        {
                            if (!serializationType.GetInterfaces().Any(i => i == typeof(IList)))
                                throw new Exception("Repeated field is not of a list type");

                            var listType = serializationType.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>));
                            var internalSerializationType = listType.GetGenericArguments()[0];

                            var cur = fieldInfo.Property.GetValue(command) as IList;
                            if (!serializationType.IsArray)
                            {
                                if (cur == null)
                                    cur = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(internalSerializationType));
                                cur.Add(read.ReadOfType(fieldLength, internalSerializationType));
                            }
                            else
                            {
                                if (cur == null)
                                    cur = Array.CreateInstance(internalSerializationType, 1);
                                else
                                {
                                    var arr = Array.CreateInstance(internalSerializationType, cur.Count + 1);
                                    Array.Copy((Array)cur, arr, cur.Count);
                                    cur = arr;
                                }

                                cur[cur.Count - 1] = read.ReadOfType(fieldLength, internalSerializationType);
                            }

                            fieldInfo.Property.SetValue(command, cur);
                        }
                        else
                        {
                            var value = read.ReadOfType(fieldLength, serializationType);
                            if (serializationType != fieldInfo.Property.PropertyType)
                            {
                                if (!fieldInfo.Property.PropertyType.IsEnum)
                                    value = Convert.ChangeType(value, fieldInfo.Property.PropertyType);
                            }

                            fieldInfo.Property.SetValue(command, value);
                        }
                    }
                }

                public static CommandAttribute GetCommandInfo(this ICommand command)
                {
                    return command.GetType().GetCustomAttribute(typeof(CommandAttribute)) as CommandAttribute;
                }
            }
        }
    }
}
