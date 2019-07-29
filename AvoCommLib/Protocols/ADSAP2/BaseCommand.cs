using AvoCommLib.Protocols.Base;

namespace AvoCommLib
{
    namespace Protocols
    {
        namespace ADSAP2
        {
            public abstract class BaseCommand : ICommand
            {
                public FieldCollection Fields { get; } = new FieldCollection();
            }
        }
    }
}
