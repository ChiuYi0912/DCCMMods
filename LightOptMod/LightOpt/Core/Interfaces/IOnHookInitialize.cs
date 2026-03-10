using LightOpt;
using ModCore.Events;

namespace Midjourney.Core.Interfaces
{
    [Event]
    public interface IOnHookInitialize
    {
        void HookInitialize(LightOptEntry entry);
    }
}