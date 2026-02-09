using dc.level;

namespace Outside_Clock.level_clock.BuildMapTool;

public static class BuildMapTool
{
    public static RoomNode AddFlags(this RoomNode node, params RoomFlag[] flags)
    {
        foreach (RoomFlag flag in flags)
        {
            node.addFlag(flag);
        }
        return node;
    }
}
