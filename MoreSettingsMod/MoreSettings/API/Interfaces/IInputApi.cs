using CoreLibrary.Utilities;
using ModCore.Events;
using MoreSettings.Configuration;

namespace MoreSettings.API.Interfaces;

[Event]
public interface IInputApi
{
    /// <summary>
    /// Called when the input system is initialized.
    /// Provides access to the controller helper for registering actions.
    /// When registering custom keys, they are automatically added to the settings under 'More Settings'
    /// </summary>
    /// <param name="controller">
    /// The game's controller helper instance.
    /// </param>
    void InputApi(ControllerHelperSuper<MainConfig> controller);
}


