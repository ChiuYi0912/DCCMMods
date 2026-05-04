

using dc.hl.types;
using dc.ui.pause;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using Moq;

namespace DCDebug.Tests;

public class HookDefaultPauseTests
{
    [Fact]
    public void Update_When_Cb_Is_Null_Should_Throw_NullReferenceException()
    {
        var entry = new virtual_cb_inter_t_();
        var options = new ArrayObj();
        options.push(entry);

        var pause = new DefaultPause();
        pause.options = options;
        pause.curOptionId = 0;
    }
}
