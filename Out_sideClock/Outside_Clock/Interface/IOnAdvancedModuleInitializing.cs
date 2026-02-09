namespace Outside_Clock.Interface.IOnAdvancedModuleInitializing
{
    [ModCore.Events.Event(true)]
    public interface IOnAdvancedModuleInitializing
    {
        void OnAdvancedModuleInitializing(Outside_Main MODMAN);
    }
}
