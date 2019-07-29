using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AvoCommLib
{
    namespace Protocols
    {
        namespace Base
        {
            public class FieldCollection : IEnumerable<CommandField>
            {
                public List<CommandField> Fields { get; private set; } = new List<CommandField>();

                public FieldCollection()
                {

                }

                public FieldCollection(IEnumerable<CommandField> Fields)
                {
                    this.Fields.AddRange(Fields);
                }

                public bool HasField(byte ID)
                {
                    return Fields.Any(f => f.FieldID == ID);
                }

                public void Remove(byte ID)
                {
                    Fields.RemoveAll(f => f.FieldID == ID);
                }

                public void Set(CommandField Field)
                {
                    Remove(Field.FieldID);
                    Fields.Add(Field);
                }

                public CommandField First(byte ID)
                {
                    return Fields.FirstOrDefault(f => f.FieldID == ID);
                }

                public IEnumerable<CommandField> Where(byte ID)
                {
                    return Fields.Where(f => f.FieldID == ID);
                }

                public IEnumerator<CommandField> GetEnumerator()
                {
                    return ((IEnumerable<CommandField>)Fields).GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return ((IEnumerable<CommandField>)Fields).GetEnumerator();
                }
            }
        }
    }
}