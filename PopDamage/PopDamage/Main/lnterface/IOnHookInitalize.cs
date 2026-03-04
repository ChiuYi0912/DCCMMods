

using ModCore.Events;

namespace PopDamage.Main.lnterface
{
    [Event]
    public interface IOnHookInitalize
    {
        void HookInitalize(PopDamageEntry entry);
    }
}