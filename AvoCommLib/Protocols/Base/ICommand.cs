
namespace AvoCommLib
{
    namespace Protocols
    {
        namespace Base
        {
            public interface ICommand
            {
                string Header { get; }
                byte CommandID { get; }
                byte[] CommandData { get; }
            }
        }
    }
}