using CoreLibrary.Utilities;
using ModCore.Events;
using MoreSettings.Configuration;

namespace MoreSettings.API.KeyBinding;

[Event]
public interface IInputApi
{
    void InputApi(ControllerHelperSuper<MainConfig> input);
}


