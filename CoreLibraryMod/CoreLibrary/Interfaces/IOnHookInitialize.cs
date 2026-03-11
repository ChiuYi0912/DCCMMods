
using ModCore.Events;

namespace CoreLibrary.Core.Interfaces
{
    [Event]
    public interface IOnHookInitialize
    {
        void HookInitialize();
    }
}