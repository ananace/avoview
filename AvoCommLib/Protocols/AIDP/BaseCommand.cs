using AvoCommLib.Protocols.Base;

namespace AvoCommLib
{
    namespace Protocols
    {
        namespace AIDP
        {
            public abstract class BaseCommand : ICommand, ICommandSequence
            {
                public ushort CommandSequence { get; set; }
                public FieldCollection Fields { get; } = new FieldCollection();
                public virtual bool IsValid { get { return true; } }
            }
        }
    }
}
