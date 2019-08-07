using System;

namespace AvoCommLib
{
    namespace Protocols
    {
        namespace Base
        {
            [AttributeUsage(AttributeTargets.Property)]
            public class CommandFieldAttribute : Attribute
            {
                public CommandFieldAttribute(byte FieldID)
                {
                    this.FieldID = FieldID;
                }

                public byte FieldID { get; private set; }
                public Type SerializeAs { get; set; }
                public bool Required { get; set; }
                public bool Repeated { get; set; }
            }
        }
    }
}

