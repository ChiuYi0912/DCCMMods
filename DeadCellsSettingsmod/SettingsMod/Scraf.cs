using System;
using System.ComponentModel.DataAnnotations;
using dc;
using dc.en;
using dc.h2d;
using dc.tool;
using HaxeProxy.Runtime;
using ModCore.Events.Interfaces.Game;
using ModCore.Modules;
using ModCore.Utitities;
using Serilog;

namespace ChiuYiUI;


public class Scraf
{
    private CHIUYIMain _mod;
    public Scraf(CHIUYIMain mod)
    {
        _mod = mod;
    }



    internal ScarfManager ScarfManager_create(Hook__ScarfManager.orig_create orig, Entity e, dc.String id)
    {
        if (CHIUYIMain.config.Value.Allscarf)
        {
            id = "KingWhite".AsHaxeString();
        }
        return orig(e, id);
    }


    public void AddSprIdToggle()
    {
        var options = dc.ui.Options.Class.ME;

        var scrollerFlow = options.scrollerFlow;
        bool scarfbool = CHIUYIMain.UseScarfGray;
        HlFunc<bool> sprIdToggleFunction = static () =>
        {
            bool newValue = !CHIUYIMain.UseScarfGray;
            CHIUYIMain.UseScarfGray = newValue;
            return newValue;

        };

        options.addToggleWidget(
            GetText.Instance.GetString("指定围巾使用的精灵").AsHaxeString(),
            GetText.Instance.GetString("打开为scarfGray。关闭为capeGray").AsHaxeString(),
            sprIdToggleFunction,
            Ref<bool>.From(ref scarfbool),
            scrollerFlow
        );
    }
    internal void AddScarfOption(int scarfIndex)
    {
        _Data_ cdb = Data.Class;
        var options = dc.ui.Options.Class.ME;
        var scrollerFlow = options.scrollerFlow;

        int value5 = (int)CHIUYIMain.GetScarfProperty<double>(scarfIndex, c => c.Color);
        int currentScarfIndex5 = scarfIndex;
        int currentScarfValue5 = value5;
        int scarfProxy5 = currentScarfValue5;
        Ref<int> scarfProxyRef5 = Ref<int>.From(ref scarfProxy5);

        HlFunc<bool> enabledFunc = new HlFunc<bool>(() =>
        {
            return true;
        });

        HlAction<int> gravityUpdateAction5 = new HlAction<int>((int value) =>
        {
            int Value = (int)value;
            if (currentScarfValue5 != Value)
            {
                currentScarfValue5 = Value;
                switch (currentScarfIndex5)
                {
                    case 0:
                        CHIUYIMain.Scarf0Color = (int)value;
                        break;
                    case 1:
                        CHIUYIMain.Scarf1Color = (int)value;
                        break;
                    case 2:
                        CHIUYIMain.Scarf2Color = (int)value;
                        break;
                    case 3:
                        CHIUYIMain.Scarf3Color = (int)value;
                        break;
                    case 4:
                        CHIUYIMain.Scarf4Color = (int)value;
                        break;
                }
                _mod.SaveConfiguration();
                try
                {
                    _Data_ cdb = Data.Class;
                    dynamic cdb_ = cdb.skin.all.array.getDyn(44);
                    dynamic up1 = cdb_.scarfs.getDyn(scarfIndex);
                    up1.color = Value;
                }
                catch (Exception ex)
                {
                    Log.Error($"更新围巾长度失败: {ex.Message}");
                }

            }
        });

        options.addHSVColorWidget(
            GetText.Instance.GetString("围巾的显示颜色").AsHaxeString(),
            GetText.Instance.GetString("注：修改后将在下一句生效").AsHaxeString(),
            enabledFunc,
            true,
            gravityUpdateAction5,
            currentScarfValue5,
            scrollerFlow
        );


        scrollerFlow = options.scrollerFlow;
        double value = CHIUYIMain.GetScarfProperty<double>(scarfIndex, c => c.MaxLength);
        double currentScarfIndex = scarfIndex;
        double currentScarfValue = value;

        HlAction<double> scarfUpdateAction = new HlAction<double>((double valuelength) =>
        {
            double intValue = (double)valuelength;
            if (currentScarfValue != intValue)
            {
                currentScarfValue = intValue;

                switch (currentScarfIndex)
                {
                    case 0:
                        CHIUYIMain.Scarf0MaxLength = intValue;
                        break;
                    case 1:
                        CHIUYIMain.Scarf1MaxLength = intValue;
                        break;
                    case 2:
                        CHIUYIMain.Scarf2MaxLength = intValue;
                        break;
                    case 3:
                        CHIUYIMain.Scarf3MaxLength = intValue;
                        break;
                    case 4:
                        CHIUYIMain.Scarf4MaxLength = intValue;
                        break;
                }
                _mod.SaveConfiguration();
                try
                {
                    _Data_ cdb = Data.Class;
                    dynamic cdb_ = cdb.skin.all.array.getDyn(44);
                    dynamic up1 = cdb_.scarfs.getDyn(scarfIndex);
                    up1.maxLength = intValue;
                }
                catch (Exception ex)
                {
                    Log.Error($"更新围巾长度失败: {ex.Message}");
                }
            }
        });
        bool showPercent = false;
        bool showRawValue = true;
        double minValue = -5;
        double maxValue = 30;
        double num7 = 0.1;
        options.addSliderWidget(
            GetText.Instance.GetString("围巾伸展时的最大长度限制").AsHaxeString(),                                  // 标签文本
            scarfUpdateAction,                           // 值更新
            currentScarfValue,                      // 初始值
            Ref<double>.From(ref num7),              // 步进
            scrollerFlow,                              // 父容器
            Ref<bool>.From(ref showPercent),         // 是否显示百分比
            Ref<bool>.From(ref showRawValue),         // 是否显示数字
            Ref<double>.From(ref minValue),          // 最小值
            Ref<double>.From(ref maxValue),          //最大值
            null,                                    //左侧填充
            Ref<int>.Null
        );



        scrollerFlow = options.scrollerFlow;
        double value1 = CHIUYIMain.GetScarfProperty<double>(scarfIndex, c => c.Gravity);
        double currentScarfIndex1 = scarfIndex;
        double currentScarfValue1 = value1;
        double scarfProxy1 = (double)currentScarfValue1;

        HlAction<double> gravityUpdateAction = new HlAction<double>((double value) =>
        {
            double Value = (double)value;
            if (currentScarfValue1 != Value)
            {
                currentScarfValue1 = Value;
                switch (currentScarfIndex1)
                {
                    case 0:
                        CHIUYIMain.Scarf0Gravity = value;
                        break;
                    case 1:
                        CHIUYIMain.Scarf1Gravity = value;
                        break;
                    case 2:
                        CHIUYIMain.Scarf2Gravity = value;
                        break;
                    case 3:
                        CHIUYIMain.Scarf3Gravity = value;
                        break;
                    case 4:
                        CHIUYIMain.Scarf4Gravity = value;
                        break;
                }
                _mod.SaveConfiguration();
                try
                {
                    _Data_ cdb = Data.Class;
                    dynamic cdb_ = cdb.skin.all.array.getDyn(44);
                    dynamic up1 = cdb_.scarfs.getDyn(scarfIndex);
                    up1.gravity = Value;
                }
                catch (Exception ex)
                {
                    Log.Error($"更新围巾长度失败: {ex.Message}");
                }

            }
        });

        // 滑块参数
        bool showPercent1 = false;
        bool showRawValue1 = true;
        double minValue1 = -10;
        double maxValue1 = 10;
        int stepInt = 1;
        double stepValue = 0.1;

        // 添加重力滑块
        options.addSliderWidget(
            GetText.Instance.GetString("围巾受到的重力影响程度").AsHaxeString(),
            gravityUpdateAction,
            currentScarfValue1,
            Ref<double>.From(ref stepValue),
            scrollerFlow,
            Ref<bool>.From(ref showPercent1),
            Ref<bool>.From(ref showRawValue1),
            Ref<double>.From(ref minValue1),
            Ref<double>.From(ref maxValue1),
            null,
            Ref<int>.From(ref stepInt)
        );



        scrollerFlow = options.scrollerFlow;
        double value2 = (double)CHIUYIMain.GetScarfProperty<double>(scarfIndex, c => c.Thickness);
        double currentScarfIndex2 = scarfIndex;
        double currentScarfValue2 = value2;
        double scarfProxy2 = (double)currentScarfValue2;

        HlAction<double> gravityUpdateAction2 = new HlAction<double>((double value) =>
        {
            double Value = (double)value;
            if (currentScarfValue2 != Value)
            {
                currentScarfValue2 = Value;
                switch (currentScarfIndex1)
                {
                    case 0:
                        CHIUYIMain.Scarf0Thickness = value;
                        break;
                    case 1:
                        CHIUYIMain.Scarf1Thickness = value;
                        break;
                    case 2:
                        CHIUYIMain.Scarf2Thickness = value;
                        break;
                    case 3:
                        CHIUYIMain.Scarf3Thickness = value;
                        break;
                    case 4:
                        CHIUYIMain.Scarf4Thickness = value;
                        break;
                }
                _mod.SaveConfiguration();
                try
                {
                    _Data_ cdb = Data.Class;
                    dynamic cdb_ = cdb.skin.all.array.getDyn(44);
                    dynamic up1 = cdb_.scarfs.getDyn(scarfIndex);
                    up1.thickness = Value;
                }
                catch (Exception ex)
                {
                    Log.Error($"更新围巾长度失败: {ex.Message}");
                }

            }
        });

        // 滑块参数
        bool showPercent2 = false;
        bool showRawValue2 = true;
        double minValue2 = 0;
        double maxValue2 = 10;
        int stepInt2 = 1;
        double stepValue2 = 0.1;

        // 添加重力滑块
        options.addSliderWidget(
            GetText.Instance.GetString("围巾的粗细程度").AsHaxeString(),
            gravityUpdateAction2,
            currentScarfValue2,
            Ref<double>.From(ref stepValue2),
            scrollerFlow,
            Ref<bool>.From(ref showPercent2),
            Ref<bool>.From(ref showRawValue2),
            Ref<double>.From(ref minValue2),
            Ref<double>.From(ref maxValue2),
            null,
            Ref<int>.From(ref stepInt2)
        );



        scrollerFlow = options.scrollerFlow;
        int value3 = (int)CHIUYIMain.GetScarfProperty<double>(scarfIndex, c => c.Count);
        int currentScarfIndex3 = scarfIndex;
        int currentScarfValue3 = value3;
        double scarfProxy3 = (double)currentScarfValue3;

        HlAction<double> gravityUpdateAction3 = new HlAction<double>((double value) =>
        {
            int Value = (int)value;
            if (currentScarfValue3 != Value)
            {
                currentScarfValue3 = Value;
                switch (currentScarfIndex1)
                {
                    case 0:
                        CHIUYIMain.Scarf0Count = (int)value;
                        break;
                    case 1:
                        CHIUYIMain.Scarf1Count = (int)value;
                        break;
                    case 2:
                        CHIUYIMain.Scarf2Count = (int)value;
                        break;
                    case 3:
                        CHIUYIMain.Scarf3Count = (int)value;
                        break;
                    case 4:
                        CHIUYIMain.Scarf4Count = (int)value;
                        break;
                }
                _mod.SaveConfiguration();
                try
                {
                    _Data_ cdb = Data.Class;
                    dynamic cdb_ = cdb.skin.all.array.getDyn(44);
                    dynamic up1 = cdb_.scarfs.getDyn(scarfIndex);
                    up1.count = Value;
                }
                catch (Exception ex)
                {
                    Log.Error($"更新围巾长度失败: {ex.Message}");
                }

            }
        });

        bool showPercent3 = false;
        bool showRawValue3 = true;
        double minValue3 = 1;
        double maxValue3 = 30;
        int stepInt3 = 1;
        double stepValue3 = 1;

        options.addSliderWidget(
            GetText.Instance.GetString("组成围巾的片段数量").AsHaxeString(),
            gravityUpdateAction3,
            currentScarfValue3,
            Ref<double>.From(ref stepValue3),
            scrollerFlow,
            Ref<bool>.From(ref showPercent3),
            Ref<bool>.From(ref showRawValue3),
            Ref<double>.From(ref minValue3),
            Ref<double>.From(ref maxValue3),
            null,
            Ref<int>.From(ref stepInt3)
        );


        scrollerFlow = options.scrollerFlow;
        double value4 = CHIUYIMain.GetScarfProperty<double>(scarfIndex, c => c.Friction);
        double currentScarfIndex4 = scarfIndex;
        double currentScarfValue4 = value4;
        double scarfProxy4 = (double)currentScarfValue4;

        HlAction<double> gravityUpdateAction4 = new HlAction<double>((double value) =>
        {
            double Value = (double)value;
            if (currentScarfValue4 != Value)
            {
                currentScarfValue4 = Value;
                switch (currentScarfIndex1)
                {
                    case 0:
                        CHIUYIMain.Scarf0Friction = value;
                        break;
                    case 1:
                        CHIUYIMain.Scarf1Friction = value;
                        break;
                    case 2:
                        CHIUYIMain.Scarf2Friction = value;
                        break;
                    case 3:
                        CHIUYIMain.Scarf3Friction = value;
                        break;
                    case 4:
                        CHIUYIMain.Scarf4Friction = value;
                        break;
                }
                _mod.SaveConfiguration();
                try
                {
                    _Data_ cdb = Data.Class;
                    dynamic cdb_ = cdb.skin.all.array.getDyn(44);
                    dynamic up1 = cdb_.scarfs.getDyn(scarfIndex);
                    up1.friction = Value;
                }
                catch (Exception ex)
                {
                    Log.Error($"更新围巾长度失败: {ex.Message}");
                }

            }
        });



        bool showPercent4 = false;
        bool showRawValue4 = true;
        double minValue4 = 0;
        double maxValue4 = 3;
        int stepInt4 = 1;
        double stepValue4 = 0.1;

        options.addSliderWidget(
            GetText.Instance.GetString("围巾移动时受到的阻力大小").AsHaxeString(),
            gravityUpdateAction4,
            currentScarfValue4,
            Ref<double>.From(ref stepValue4),
            scrollerFlow,
            Ref<bool>.From(ref showPercent4),
            Ref<bool>.From(ref showRawValue4),
            Ref<double>.From(ref minValue4),
            Ref<double>.From(ref maxValue4),
            null,
            Ref<int>.From(ref stepInt4)
        );


        scrollerFlow = options.scrollerFlow;
        double value6 = CHIUYIMain.GetScarfProperty<double>(scarfIndex, c => c.MinLength);
        double currentScarfIndex6 = scarfIndex;
        double currentScarfValue6 = value6;
        double scarfProxy6 = (double)currentScarfValue6;

        HlAction<double> gravityUpdateAction6 = new HlAction<double>((double value) =>
        {
            double Value = (double)value;
            if (currentScarfValue6 != Value)
            {
                currentScarfValue6 = Value;
                switch (currentScarfIndex1)
                {
                    case 0:
                        CHIUYIMain.scarf0MinLangh = value;
                        break;
                    case 1:
                        CHIUYIMain.scarf1MinLangh = value;
                        break;
                    case 2:
                        CHIUYIMain.scarf2MinLangh = value;
                        break;
                    case 3:
                        CHIUYIMain.scarf3MinLangh = value;
                        break;
                    case 4:
                        CHIUYIMain.scarf4MinLangh = value;
                        break;
                }
                _mod.SaveConfiguration();
                try
                {
                    _Data_ cdb = Data.Class;
                    dynamic cdb_ = cdb.skin.all.array.getDyn(44);
                    dynamic up1 = cdb_.scarfs.getDyn(scarfIndex);
                    up1.minLength = Value;
                }
                catch (Exception ex)
                {
                    Log.Error($"更新围巾长度失败: {ex.Message}");
                }

            }
        });



        bool showPercent6 = false;
        bool showRawValue6 = true;
        double minValue6 = -1;
        double maxValue6 = 15;
        int stepInt6 = 1;
        double stepValue6 = 0.1;

        options.addSliderWidget(
           GetText.Instance.GetString("围巾收缩时的最小长度限制").AsHaxeString(),
            gravityUpdateAction6,
            currentScarfValue6,
            Ref<double>.From(ref stepValue6),
            scrollerFlow,
            Ref<bool>.From(ref showPercent6),
            Ref<bool>.From(ref showRawValue6),
            Ref<double>.From(ref minValue6),
            Ref<double>.From(ref maxValue6),
            null,
            Ref<int>.From(ref stepInt6)
        );


    }

}
