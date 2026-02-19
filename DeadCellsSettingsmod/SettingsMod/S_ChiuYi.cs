using System;
using dc.ui;
using static dc.ui.OptionsSection;

namespace S_ChiuYi;

public class S_ChiuYi : S_Food
{
    public override OptionsSection.Indexes Index
    {
        get
        {
            return (OptionsSection.Indexes)20;
        }
    }

    public S_ChiuYi() : base()
    {

    }

    public new enum Indexes
    {
        S_Main,
        // Token: 0x040000A4 RID: 164
        S_Gamepad,
        // Token: 0x040000A5 RID: 165
        S_GamepadRebind,
        // Token: 0x040000A6 RID: 166
        S_Keyboard,
        // Token: 0x040000A7 RID: 167
        S_KeyboardRebind,
        // Token: 0x040000A8 RID: 168
        S_Video,
        // Token: 0x040000A9 RID: 169
        S_Accessibility,
        // Token: 0x040000AA RID: 170
        S_Sound,
        // Token: 0x040000AB RID: 171
        S_Lang,
        // Token: 0x040000AC RID: 172
        S_Food,
        // Token: 0x040000AD RID: 173
        S_Music,
        // Token: 0x040000AE RID: 174
        S_SfxVol,
        // Token: 0x040000AF RID: 175
        S_SfxAdv,
        // Token: 0x040000B0 RID: 176
        S_Mods,
        // Token: 0x040000B1 RID: 177
        S_GP,
        // Token: 0x040000B2 RID: 178
        S_Credits,
        // Token: 0x040000B3 RID: 179
        S_Stream,
        // Token: 0x040000B4 RID: 180
        AM_Main,
        // Token: 0x040000B5 RID: 181
        AM_Tuto,

        S_ChiuYi
    }

}


