using ModCore.Events;

namespace MoreSettings.API.KeyBinding;

[Event]
public interface IInputApi
{
    void InputApi(IInputApiService input);
}


