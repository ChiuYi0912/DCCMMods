using dc;
using dc.cine;
using dc.en;
using dc.pr;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Mods;
using ModCore.Utilities;
using Serilog;
using ModCore.Modules;
using dc.en.inter;
using dc.ui.popd;
using dc.tool.atk;
using ModCore.Events.Interfaces.Game;
using dc.tool;
using ModCore.Storage;
using CiuYiUI;

namespace ChiuYiUI;

public class CHIUYIMain : ModBase,
IOnGameEndInit,
IOnAfterLoadingCDB
{

    private ChiuYiUI _ui;
    private Scraf _toscarf;
#pragma warning disable CS8618 
    public CHIUYIMain(ModInfo info) : base(info)
    {
        _instance = this;
    }
#pragma warning restore CS8618 
    private static CHIUYIMain? _instance;
    private Config _config;
    private LevelMusic _levelMusic;
    public static Config<PothionsConfig> config = new("ScarafConfig");
    public override void Initialize()
    {
        Log.Information("整合包运行成功！by ChiuYi.秋");
        base.Initialize();
        _config = ConfigManager.LoadConfig();
        _skinEnabled = _config.SkinEnabled;
        _skinkatana = _config.skinkatana;
        _teleport = _config.teleport;
        _pop = _config.pop;
        _sty = _config.rsty;
        _useScarfGray = _config.UseScarfGray;

        _levelMusic = new LevelMusic(this);
        _levelMusic.HookInitialize();

        _ui = new ChiuYiUI(this);
        _ui.Viewporthook();
        _toscarf = new Scraf(this);


        Hook_Hero.hasSkin += allaskin;
        Hook_Hero.hasSkin += katanaskin;
        Hook_Teleport.startTeleport += yuteleport;
        Hook_Entity.popDamage += popmiami;
        Hook_Entity.popDamage += sty;

        Hook__ScarfManager.create += _toscarf.ScarfManager_create;

        dc.ui.Hook_Options.showMain += _ui.Hook_Options_showmain;
        dc.ui.Hook_Options.buildCurSection += _ui.Hook_Options_buildCurSection;



    }


    public static List<int> GetAllScarfIndices()
    {
        return _scarfConfigs.Keys.ToList();
    }
    void IOnAfterLoadingCDB.OnAfterLoadingCDB(dc._Data_ cdb)
    {
        #region 加载围巾配置
        _scarfConfigs = _config.ScarfConfigs;
        GetAllScarfIndices().ForEach(index =>
        {
            var config = _scarfConfigs[index];
            SetScarfConfig(index, config);
        });
        UseScarfGray = _config.UseScarfGray;
        ApplyAllScarfConfigs();
        CreateDefaultConfig();
        #endregion
    }


    public void OnGameEndInit()
    {
        Log.Information("[Lang]lodar语言切换加入");
        Log.Information("游戏初始化完成！");
        var res = this.Info.ModRoot!.GetFilePath("res.pak");
        FsPak.Instance.FileSystem.loadPak(res.AsHaxeString());
    }


    #region 储存配置
    public void SaveConfiguration()
    {
        _config.SkinEnabled = _skinEnabled;
        _config.skinkatana = _skinkatana;
        _config.teleport = _teleport;
        _config.pop = _pop;
        _config.rsty = _sty;

        _config.UseScarfGray = UseScarfGray;

        _config.ScarfConfigs = _scarfConfigs;
        ConfigManager.SaveConfig(_config);
    }
    #endregion

    #region 装束效果
    private static bool _skinEnabled = true;
    private bool allaskin(Hook_Hero.orig_hasSkin orig, Hero self, dc.String model, dc.String itemId)
    {
        return SkinEnabled;
    }

    public static bool SkinEnabled
    {
        get { return _skinEnabled; }
        set
        {
            if (_skinEnabled != value)
            {
                _skinEnabled = value;
                Instance.SaveConfiguration();
            }
        }
    }
    public static bool skinkatana
    {
        get { return _skinkatana; }
        set
        {
            if (_skinkatana != value)
            {
                _skinkatana = value;
                Instance.SaveConfiguration();
            }
        }
    }
    public static bool teleport
    {
        get { return _teleport; }
        set
        {
            if (_teleport != value)
            {
                _teleport = value;
                Instance.SaveConfiguration();
            }
        }
    }
    public static bool pop
    {
        get { return _pop; }
        set
        {
            if (_pop != value)
            {
                _pop = value;
                Instance.SaveConfiguration();
            }
        }
    }
    public static bool rsty
    {
        get { return _sty; }
        set
        {
            if (_sty != value)
            {
                _sty = value;
                Instance.SaveConfiguration();
            }
        }
    }


#pragma warning disable CS8603
    public static CHIUYIMain Instance => _instance;
#pragma warning restore CS8603 
    private static bool _skinkatana = true;
    private static bool katanaskin(Hook_Hero.orig_hasSkin orig, Hero self, dc.String model, dc.String itemId)
    {
        if (itemId != null && itemId.ToString() == "KatanaZero")
        {
            return skinkatana;
        }

        bool a = orig(self, model, itemId);
        return a;

    }
    private void zerokatana(Hook_Game.orig_loadMainLevel orig, dc.pr.Game self, LevelTransition id, dc.String activate, Ref<bool> forcedSeed, int? activat)
    {

        if (self.hero.hasSkin(null, "KatanaZero".AsHaxeString()) != false)
        {
            orig(self, id, activate, forcedSeed, activat);
        }


    }
    private static bool _teleport = true;

    private void yuteleport(Hook_Teleport.orig_startTeleport orig, Teleport self, Hero hero, Entity to)
    {
        if (to == null) return;

        if (_teleport == true)
        {
            TeleportationRoR teleportationRoR = new TeleportationRoR(hero, self, to);
            return;
        }
        Teleportation teleportation = new Teleportation(hero, self, to);
    }

    private static bool _pop = true;
    private void popmiami(Hook_Entity.orig_popDamage orig, Entity self, AttackData a)
    {
        if (dc.ui.Console.Class.ME.flags.exists("NoPopText".AsHaxeString())) return;
        if (_pop)
        {
            a.hasTag(2);
            virtual_chars_font_ virtual_chars_font_ = new virtual_chars_font_();
            virtual_chars_font_.chars = "numbers".AsHaxeString();
            virtual_chars_font_.font = "sts".AsHaxeString();
            _PopDamageSts DamageSts = PopDamageSts.Class;
            PopDamageSts popDamageHotline = DamageSts.create(self, a, self.dmgIdx, new Ref<bool>(), virtual_chars_font_);
        }
        else
        {
            orig(self, a);
        }
    }

    private static bool _sty = true;
    private void sty(Hook_Entity.orig_popDamage orig, Entity self, AttackData a)
    {
        if (dc.ui.Console.Class.ME.flags.exists("NoPopText".AsHaxeString())) return;
        if (_sty)
        {
            a.hasTag(2);
            virtual_chars_font_ virtual_chars_font_ = new virtual_chars_font_();
            virtual_chars_font_.chars = "numbers".AsHaxeString();
            virtual_chars_font_.font = "hotline".AsHaxeString();
            _PopDamageHotline hotlineInstance = PopDamageHotline.Class;
            PopDamageHotline popDamageHotline = hotlineInstance.create(self, a, self.dmgIdx, new Ref<bool>(), virtual_chars_font_);
        }
        else
        {
            orig(self, a);
        }
    }
    #endregion

    #region 围巾初始化配置
    private static Dictionary<int, ScarfConfig> _scarfConfigs = new Dictionary<int, ScarfConfig>
    {
    };


    public static ScarfConfig GetScarfConfig(int scarfIndex)
    {
        if (_scarfConfigs.ContainsKey(scarfIndex))
            return _scarfConfigs[scarfIndex];

        var defaultConfig = new ScarfConfig { MaxLength = 30 };
        _scarfConfigs[scarfIndex] = defaultConfig;
        return defaultConfig;
    }

    public static void SetScarfConfig(int scarfIndex, ScarfConfig config)
    {
        if (config != null)
        {
            _scarfConfigs[scarfIndex] = config;
            Instance.SaveConfiguration();
        }
    }

    public static T GetScarfProperty<T>(int scarfIndex, Func<ScarfConfig, T> propertySelector)
    {
        var config = GetScarfConfig(scarfIndex);
        return propertySelector(config);
    }

    public static void SetScarfProperty(int scarfIndex, Action<ScarfConfig> propertySetter)
    {
        var config = GetScarfConfig(scarfIndex);
        propertySetter(config);
        SetScarfConfig(scarfIndex, config);
    }



    #region 单个围巾长度配置属性
    public static double Scarf0MaxLength
    {
        get { return GetScarfProperty(0, c => c.MaxLength); }
        set { SetScarfProperty(0, c => c.MaxLength = value); }
    }

    public static double Scarf1MaxLength
    {
        get { return GetScarfProperty(1, c => c.MaxLength); }
        set { SetScarfProperty(1, c => c.MaxLength = value); }
    }

    public static double Scarf2MaxLength
    {
        get { return GetScarfProperty(2, c => c.MaxLength); }
        set { SetScarfProperty(2, c => c.MaxLength = value); }
    }

    public static double Scarf3MaxLength
    {
        get { return GetScarfProperty(3, c => c.MaxLength); }
        set { SetScarfProperty(3, c => c.MaxLength = value); }
    }

    public static double Scarf4MaxLength
    {
        get { return GetScarfProperty(4, c => c.MaxLength); }
        set { SetScarfProperty(4, c => c.MaxLength = value); }
    }

    #endregion
    #endregion


    #region 重力属性访问器
    public static double Scarf0Gravity
    {
        get { return GetScarfProperty(0, c => c.Gravity); }
        set
        {
            if (GetScarfProperty(0, c => c.Gravity) != value)
            {
                SetScarfProperty(0, c => c.Gravity = value);
                Instance.SaveConfiguration();
            }
        }
    }

    public static double Scarf1Gravity
    {
        get { return GetScarfProperty(1, c => c.Gravity); }
        set
        {
            if (GetScarfProperty(1, c => c.Gravity) != value)
            {
                SetScarfProperty(1, c => c.Gravity = value);
                Instance.SaveConfiguration();
            }
        }
    }

    public static double Scarf2Gravity
    {
        get { return GetScarfProperty(2, c => c.Gravity); }
        set
        {
            if (GetScarfProperty(2, c => c.Gravity) != value)
            {
                SetScarfProperty(2, c => c.Gravity = value);
                Instance.SaveConfiguration();
            }
        }
    }

    public static double Scarf3Gravity
    {
        get { return GetScarfProperty(3, c => c.Gravity); }
        set
        {
            if (GetScarfProperty(3, c => c.Gravity) != value)
            {
                SetScarfProperty(3, c => c.Gravity = value);
                Instance.SaveConfiguration();
            }
        }
    }

    public static double Scarf4Gravity
    {
        get { return GetScarfProperty(4, c => c.Gravity); }
        set
        {
            if (GetScarfProperty(4, c => c.Gravity) != value)
            {
                SetScarfProperty(4, c => c.Gravity = value);
                Instance.SaveConfiguration();
            }
        }
    }
    #endregion

    #region 厚度属性
    public static double Scarf0Thickness
    {
        get { return GetScarfProperty(0, c => c.Thickness); }
        set
        {
            if (GetScarfProperty(0, c => c.Thickness) != value)
            {
                SetScarfProperty(0, c => c.Thickness = value);
                Instance.SaveConfiguration();
            }
        }
    }
    public static double Scarf1Thickness
    {
        get { return GetScarfProperty(1, c => c.Thickness); }
        set
        {
            if (GetScarfProperty(1, c => c.Thickness) != value)
            {
                SetScarfProperty(1, c => c.Thickness = value);
                Instance.SaveConfiguration();
            }
        }
    }
    public static double Scarf2Thickness
    {
        get { return GetScarfProperty(2, c => c.Thickness); }
        set
        {
            if (GetScarfProperty(2, c => c.Thickness) != value)
            {
                SetScarfProperty(2, c => c.Thickness = value);
                Instance.SaveConfiguration();
            }
        }
    }
    public static double Scarf3Thickness
    {
        get { return GetScarfProperty(3, c => c.Thickness); }
        set
        {
            if (GetScarfProperty(3, c => c.Thickness) != value)
            {
                SetScarfProperty(3, c => c.Thickness = value);
                Instance.SaveConfiguration();
            }
        }
    }
    public static double Scarf4Thickness
    {
        get { return GetScarfProperty(4, c => c.Thickness); }
        set
        {
            if (GetScarfProperty(4, c => c.Thickness) != value)
            {
                SetScarfProperty(4, c => c.Thickness = value);
                Instance.SaveConfiguration();
            }
        }
    }
    #endregion

    #region 围巾节数
    public static int Scarf0Count
    {
        get { return GetScarfProperty(0, c => c.Count); }
        set
        {
            if (GetScarfProperty(0, c => c.Count) != value)
            {
                SetScarfProperty(0, c => c.Count = value);
                Instance.SaveConfiguration();
            }
        }
    }
    public static int Scarf1Count
    {
        get { return GetScarfProperty(1, c => c.Count); }
        set
        {
            if (GetScarfProperty(1, c => c.Count) != value)
            {
                SetScarfProperty(1, c => c.Count = value);
                Instance.SaveConfiguration();
            }
        }
    }
    public static int Scarf2Count
    {
        get { return GetScarfProperty(2, c => c.Count); }
        set
        {
            if (GetScarfProperty(2, c => c.Count) != value)
            {
                SetScarfProperty(2, c => c.Count = value);
                Instance.SaveConfiguration();
            }
        }
    }
    public static int Scarf3Count
    {
        get { return GetScarfProperty(3, c => c.Count); }
        set
        {
            if (GetScarfProperty(3, c => c.Count) != value)
            {
                SetScarfProperty(3, c => c.Count = value);
                Instance.SaveConfiguration();
            }
        }
    }
    public static int Scarf4Count
    {
        get { return GetScarfProperty(4, c => c.Count); }
        set
        {
            if (GetScarfProperty(4, c => c.Count) != value)
            {
                SetScarfProperty(4, c => c.Count = value);
                Instance.SaveConfiguration();
            }
        }
    }
    #endregion

    #region 围巾摩擦力
    public static double Scarf0Friction
    {
        get { return GetScarfProperty(0, c => c.Friction); }
        set
        {
            if (GetScarfProperty(0, c => c.Friction) != value)
            {
                SetScarfProperty(0, c => c.Friction = value);
                Instance.SaveConfiguration();
            }
        }
    }
    public static double Scarf1Friction
    {
        get { return GetScarfProperty(1, c => c.Friction); }
        set
        {
            if (GetScarfProperty(1, c => c.Friction) != value)
            {
                SetScarfProperty(1, c => c.Friction = value);
                Instance.SaveConfiguration();
            }
        }
    }
    public static double Scarf2Friction
    {
        get { return GetScarfProperty(2, c => c.Friction); }
        set
        {
            if (GetScarfProperty(2, c => c.Friction) != value)
            {
                SetScarfProperty(2, c => c.Friction = value);
                Instance.SaveConfiguration();
            }
        }
    }
    public static double Scarf3Friction
    {
        get { return GetScarfProperty(3, c => c.Friction); }
        set
        {
            if (GetScarfProperty(3, c => c.Friction) != value)
            {
                SetScarfProperty(3, c => c.Friction = value);
                Instance.SaveConfiguration();
            }
        }
    }
    public static double Scarf4Friction
    {
        get { return GetScarfProperty(4, c => c.Friction); }
        set
        {
            if (GetScarfProperty(4, c => c.Friction) != value)
            {
                SetScarfProperty(4, c => c.Friction = value);
                Instance.SaveConfiguration();
            }
        }
    }
    #endregion

    #region 围巾颜色
    public static int Scarf0Color
    {
        get { return GetScarfProperty(0, c => c.Color); }
        set
        {
            if (GetScarfProperty(0, c => c.Color) != value)
            {
                SetScarfProperty(0, c => c.Color = value);
                Instance.SaveConfiguration();
            }
        }
    }
    public static int Scarf1Color
    {
        get { return GetScarfProperty(1, c => c.Color); }
        set
        {
            if (GetScarfProperty(1, c => c.Color) != value)
            {
                SetScarfProperty(1, c => c.Color = value);
                Instance.SaveConfiguration();
            }
        }
    }
    public static int Scarf2Color
    {
        get { return GetScarfProperty(2, c => c.Color); }
        set
        {
            if (GetScarfProperty(2, c => c.Color) != value)
            {
                SetScarfProperty(2, c => c.Color = value);
                Instance.SaveConfiguration();
            }
        }
    }
    public static int Scarf3Color
    {
        get { return GetScarfProperty(3, c => c.Color); }
        set
        {
            if (GetScarfProperty(3, c => c.Color) != value)
            {
                SetScarfProperty(3, c => c.Color = value);
                Instance.SaveConfiguration();
            }
        }
    }
    public static int Scarf4Color
    {
        get { return GetScarfProperty(4, c => c.Color); }
        set
        {
            if (GetScarfProperty(4, c => c.Color) != value)
            {
                SetScarfProperty(4, c => c.Color = value);
                Instance.SaveConfiguration();
            }
        }
    }
    #endregion
    #region 围巾最小长度
    public static double scarf0MinLangh
    {
        get { return GetScarfProperty(0, c => c.MinLength); }
        set
        {
            if (GetScarfProperty(0, c => c.MinLength) != value)
            {
                SetScarfProperty(0, c => c.MinLength = value);
                Instance.SaveConfiguration();
            }
        }
    }
    public static double scarf1MinLangh
    {
        get { return GetScarfProperty(1, c => c.MinLength); }
        set
        {
            if (GetScarfProperty(1, c => c.MinLength) != value)
            {
                SetScarfProperty(1, c => c.MinLength = value);
                Instance.SaveConfiguration();
            }
        }
    }
    public static double scarf2MinLangh
    {
        get { return GetScarfProperty(2, c => c.MinLength); }
        set
        {
            if (GetScarfProperty(2, c => c.MinLength) != value)
            {
                SetScarfProperty(2, c => c.MinLength = value);
                Instance.SaveConfiguration();
            }
        }
    }
    public static double scarf3MinLangh
    {
        get { return GetScarfProperty(3, c => c.MinLength); }
        set
        {
            if (GetScarfProperty(3, c => c.MinLength) != value)
            {
                SetScarfProperty(3, c => c.MinLength = value);
                Instance.SaveConfiguration();
            }
        }
    }
    public static double scarf4MinLangh
    {
        get { return GetScarfProperty(4, c => c.MinLength); }
        set
        {
            if (GetScarfProperty(4, c => c.MinLength) != value)
            {
                SetScarfProperty(4, c => c.MinLength = value);
                Instance.SaveConfiguration();
            }
        }
    }

    private static bool _useScarfGray = true;
    public static bool UseScarfGray
    {
        get { return _useScarfGray; }
        set
        {
            if (_useScarfGray != value)
            {
                _useScarfGray = value;
                Instance.SaveConfiguration();
                ApplyAllScarfConfigs();
            }
        }
    }

    #endregion
    public static void ApplyScarfConfig(int scarfIndex)
    {
        try
        {
            var config = CHIUYIMain.GetScarfConfig(scarfIndex);

            _Data_ cdb = dc.Data.Class;
            dynamic cdb_ = cdb.skin.all.array.getDyn(44);
            dynamic scarf = cdb_.scarfs.getDyn(scarfIndex);


            if (CHIUYIMain.UseScarfGray)
            {
                scarf.sprId = "scarfGray";
            }
            else
            {
                scarf.sprId = "capeGray";
            }


            //scarf.sprId = config.SprId;
            scarf.cosOffset = config.CosOffset;
            scarf.attachOffX = config.AttachOffX;
            scarf.attachOffY = config.AttachOffY;
            scarf.maxLength = config.MaxLength;
            scarf.friction = config.Friction;
            scarf.minLength = config.MinLength;
            scarf.onFront = config.OnFront;
            scarf.color = config.Color;
            scarf.count = config.Count;
            scarf.gravity = config.Gravity;
            scarf.thickness = config.Thickness;

            foreach (var prop in config.Props)
            {
                scarf.props[prop.Key] = prop.Value;
            }

            Log.Information($"应用围巾{scarfIndex}配置成功");
        }
        catch (Exception)
        {

        }
    }

    public static void ApplyAllScarfConfigs()
    {
        foreach (var scarfIndex in CHIUYIMain.GetAllScarfIndices())
        {
            ApplyScarfConfig(scarfIndex);
        }
    }

    public ScarfConfig LoadScarfConfigFromCDB(int scarfIndex)
    {
        try
        {
            _Data_ cdb = dc.Data.Class;
            dynamic cdb_ = cdb.skin.all.array.getDyn(44);
            dynamic scarf = cdb_.scarfs.getDyn(scarfIndex);

            var config = new ScarfConfig
            {
                SprId = scarf.sprId,
                CosOffset = scarf.cosOffset,
                AttachOffX = scarf.attachOffX,
                AttachOffY = scarf.attachOffY,
                MaxLength = scarf.maxLength,
                Friction = scarf.friction,
                MinLength = scarf.minLength,
                OnFront = scarf.onFront,
                Color = scarf.color,
                Count = scarf.count,
                Gravity = scarf.gravity,
                Thickness = scarf.thickness
            };

            // 加载props
            dynamic props = scarf.props;
            if (props != null)
            {
                foreach (var key in props.keys())
                {
                    config.Props[key] = props[key];
                }
            }

            Log.Information($"从CDB加载围巾{scarfIndex}配置成功");
            return config;
        }
        catch (Exception)
        {
            return new ScarfConfig();
        }
    }

    public static ScarfConfig CreateDefaultConfig()
    {
        return new ScarfConfig
        {
            SprId = "scarfGlow",
            CosOffset = 3,
            Props = new Dictionary<string, object>(),
            AttachOffX = -4,
            AttachOffY = 2,
            MaxLength = 2,
            Friction = 0.6,
            MinLength = 2,
            OnFront = false,
            Color = 8724512,
            Count = 6,
            Gravity = 1.5,
            Thickness = 2
        };
    }

    public static ScarfConfig CreateCustomConfig(string sprId, int maxLength, double friction, int color, int count, double gravity, int thickness)
    {
        return new ScarfConfig(sprId, maxLength, friction, color, count, gravity, thickness);
    }


}