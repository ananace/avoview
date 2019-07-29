using AvoCommLib.Protocols.Base;

namespace AvoCommLib
{
    namespace Protocols
    {
        namespace ASMP
        {
            public abstract class BaseCommand : ICommand
            {
                public FieldCollection Fields { get; } = new FieldCollection();
            }
        }
    }
}
