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
                        foreach (var field in command.GetFields())
                        {
                            if (field.FieldData == null)
                                continue;

                            write.Write(field.FieldID);
                            write.Write((Int16)field.FieldData.Length);
                            write.Write(field.FieldData);
                        }

                        if (command.Fields.Any() || info.AlwaysAddEOFField)
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

                public static CommandAttribute GetCommandInfo(this ICommand command)
                {
                    return command.GetType().GetCustomAttribute(typeof(CommandAttribute)) as CommandAttribute;
                }
            }
        }
    }
}
