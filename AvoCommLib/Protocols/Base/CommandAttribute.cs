using System;

namespace AvoCommLib
{
    namespace Protocols
    {
        namespace Base
        {
            [AttributeUsage(AttributeTargets.Class)]
            public class CommandAttribute : Attribute
            {
                public CommandAttribute(string Header, byte CommandID)
                {
                    this.Header = Header;
                    this.CommandID = CommandID;
                }

                public string Header { get; private set; }
                public byte CommandID { get; private set; }
                public bool AlwaysAddEOFField { get; set; }
            }
        }
    }
}