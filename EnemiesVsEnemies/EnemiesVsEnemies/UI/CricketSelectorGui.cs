using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc;
using dc.en;
using dc.en.mob;
using dc.h2d;
using dc.hl.types;
using dc.hxd;
using dc.hxd.res;
using dc.hxd.snd;
using dc.libs;
using dc.libs.heaps.slib;
using dc.pr;
using dc.tool;
using dc.ui;
using dc.ui.icon;
using dc.ui.sel;
using EnemiesVsEnemies.Configuration;
using EnemiesVsEnemies.Core;
using EnemiesVsEnemies.UI.Utilities;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Storage;
using Serilog;
using Data = dc.Data;
using Interactive = dc.h2d.Interactive;
using Text = dc.ui.Text;
using CoreLibrary.Utilities;
using CoreLibrary.Core.Utilities;
using ModCore.Modules;
using dc.h2d.col;
using ModCore.Utilities;
using dc.ui.hud;
using dc.libs.misc;
using dc.haxe.io;
using Math = System.Math;

namespace EnemiesVsEnemies.UI
{
    public class CricketSelectorGui : GridSelector
    {
        public NumberInput numberInput = null!;
        public TeamManager teamManager = null!;
        public FlowBox teamFlowBox = null!;
        public string UserSelectedteamid = null!;

        public UITextHelper textHelper = null!;

        public Config<ModConfig> GetConfig = EnemiesVsEnemiesMod.config;


        public Mask rightMask = null!;
        private Interactive rightInter = null!;


        public class MonsterSelectionEventArgs
        {
            public string MobId { get; set; } = null!;
            public string Teamid { get; set; } = null!;
        }
        public Action<MonsterSelectionEventArgs> OnMonsterSelected { get; set; }

        public CricketSelectorGui(TeamManager teammanager)
        {

            teamManager = teammanager;
            OnMonsterSelected = (data) => { };
        }


        public override int get_wid() => 10;
        public override int get_entryWid() => 24;
        public override int get_entryHei() => 24;

        public const string AudioError = "sfx/ui/menu_error2.wav";
        public const string Audiocurse = "sfx/ps5/curse_end_SE.wav";
        public const string Audioclick = "sfx/ui/menu_click1.wav";


        public override bool isEntryLocked(int i)
        {
            string data = Data.Class.mob.all.array.getDyn(i).id.ToString();
            return EnemiesVsEnemiesMod.GetMobGroupHelper().IsRealBoss(data);
        }



        public override void initGrid()
        {
            curX = curY = 0;
            initEntries(UIMobHelper.getmoblength());
        }



        public override void initRightFlow()
        {
            textHelper = new UITextHelper(this);


            double padH = 5.0;
            double padV = 5.0;
            base.rightFlow = FlowBox.Class.createBoxValidation(null, Ref<double>.From(ref padH), Ref<double>.From(ref padV), Ref<bool>.Null, null);
            base.rightFlow.set_isVertical(true);
            base.rightFlow.set_horizontalAlign(new FlowAlign.Right());
            base.rightFlow.set_verticalAlign(new FlowAlign.Bottom());


            base.mainFlow.addChild(base.rightFlow);


            var nameText = Assets.Class.makeText(Lang.Class.t.untranslated(""), null, true, null);
            nameText.set_textColor(Text.Class.COLORS.get("ST".ToHaxeString()));
            nameText.set_textAlign(new Align.MultilineCenter());

            textHelper.AlluiText.Add("nameText", nameText);

            base.root.addChild(nameText);

            rightMask = new Mask(200, 300, null);
            rightMask.addChild(rightFlow);
            base.mainFlow.addChild(rightMask);

            rightInter = new Interactive(rightMask.width, rightMask.height, rightMask, null);
            rightInter.propagateEvents = true;
            rightInter.onWheel = new HlAction<Event>(OnRightWheel);



            textHelper.AddConfigInfoToRightFlow();

            // GetU = new NumberInput(this);
            // var action = new Action<int>((value) =>
            // {
            //     istest = value;
            // });
            // var inpu = GetU.OpenNumberInput("TEST", "hello", 1, action);
        }

        private void OnRightWheel(Event e)
        {
            if (controller == null) return;
            if (controller.parent.exclusiveId != controller.id) return;

            double delta = e.wheelDelta;
            if (delta == 0) return;

            double step = 20;
            double contentHeight = base.rightFlow.get_outerHeight();
            double maskHeight = rightMask.height;
            double maxScroll = Math.Max(0, contentHeight - maskHeight);
            double newY = base.rightFlow.y - delta * step;
            newY = Math.Max(-maxScroll, Math.Min(0, newY));

            base.rightFlow.y = newY;
            base.rightFlow.posChanged = true;
        }


        public override void onValidate()
        {
            var entry = getEntryAt(curX, curY);
            if (entry == null)
                return;


            string mobId = UIMobHelper.getmobs(entry.i).id.ToString();
            var args = new MonsterSelectionEventArgs { MobId = mobId, Teamid = UserSelectedteamid };
            if (string.IsNullOrEmpty(args.MobId) || string.IsNullOrEmpty(args.Teamid) || entry.isLocked)
            {
                AudioHelper.LoadAudioFormString(AudioError);
                return;
            }


            AddMonsterToTeam(args);


            UIAnimHelper.doMovementIcon(this, textHelper.AlluiText[args.Teamid], entry, args);
        }



        private void AddMonsterToTeam(MonsterSelectionEventArgs args)
        {
            if (EnemiesVsEnemiesMod.GetMobGroupHelper().IsRealBoss(args.MobId))
                return;


            if (!GetConfig.Value.Teams.TryGetValue(args.Teamid, out var team))
            {
                AudioHelper.LoadAudioFormString(AudioError);
                Log.Logger.Warning($"队伍 {args.Teamid} 不存在");
                return;
            }


            team.DefaultEnemies.Add(args.MobId);
            GetConfig.Save();
            textHelper.UpdateTeamDisplay();


            AudioHelper.LoadAudioFormString(Audiocurse);

            Log.Logger.Debug($"选择选中怪物：{args.MobId}, teamid:{args.Teamid}");
        }

        public override void onResize()
        {
            base.onResize();

            double pixelScale = base.get_pixelScale.Invoke();
            double fbWidth = base.fbItems.get_outerWidth();
            double fbHeight = base.fbItems.get_outerHeight();
            double leftX = base.fbItems.x;
            double leftY = base.fbItems.y;


            double rightX = leftX + fbWidth + 20 * pixelScale;
            double rightY = leftY;

            rightMask.x = rightX;
            rightMask.y = rightY;
            rightMask.width = (int)fbWidth;
            rightMask.height = (int)fbHeight;


            rightInter.width = fbWidth;
            rightInter.height = fbHeight;

            base.rightFlow.reflow();
        }

        public override void setMainFlowPos()
        {
            base.setMainFlowPos();
            mainFlow.reflow();
            mainFlow.x = 50;
            mainFlow.posChanged = true;
        }


        public override void updateRightFlow() { }
        public override void beforeUpdateSelection() { }


        public override dc.h2d.Object getIconBmp(int i, dc.h2d.Object parent)
        {
            string name = UIMobHelper.getmobs(i).id.ToString();
            if (name.IsNullOrEmpty())
                return new Bitmap(Tile.Class.fromColor(0xFF0000, 32, 32, null, null), parent);


            var icon = Icon.Class.createMobIcon(name.ToHaxeString(), parent);
            icon.tile.scaleToSize(get_entryWid(), get_entryHei());


            return icon;
        }

        public override void postUpdate()
        {
            Main main = Main.Class.ME;
            int curX = this.curX;
            int curY = this.curY;


            Point entryGlobal = main.localToGlobal(this.getEntryAt(curX, curY).f, Ref<double>.Null, Ref<double>.Null);
            Point maskGlobal = main.localToGlobal(this.mask, Ref<double>.Null, Ref<double>.Null);


            double pixelScale = base.get_pixelScale.Invoke();
            double offset = (int)(pixelScale * 5.0);


            ScaleGrid selection = this.selectionSG;
            selection.posChanged = true;
            selection.x = entryGlobal.x - maskGlobal.x - offset;
            selection.y = entryGlobal.y - maskGlobal.y - offset;


            double timeFactor = base.ftime * 0.1;
            string speedKey = "co_blinkCursorSpeed";


            var speedData = Data.Class.gui.byId.get(speedKey.ToHaxeString()).v0;


            var angle = timeFactor * speedData;
            var cosValue = System.Math.Cos(angle);
            var alphaOffset = 0.2 * cosValue;


            this.selectionSG.alpha = 0.8 + alphaOffset;


            if (UserSelectedteamid.IsNullOrEmpty())
                return;


            if (textHelper.AlluiText.TryGetValue(UserSelectedteamid, out var text))
                text.alpha = 0.8 + alphaOffset;

        }



        public override void initEntries(int size)
        {
            this.sections = (ArrayObj)ArrayUtils.CreateDyn().array;
            this.entries = (ArrayObj)ArrayUtils.CreateDyn().array;

            int column = 0;
            int row = 0;
            int width = get_wid();

            for (int index = 0; index < size; index++)
            {
                _ = this.addEntryAt(index, column, row, Ref<int>.Null);
                column++;

                if (column >= width)
                {
                    column = 0;
                    row++;
                }
            }
        }

        public override void setControlLabel()
        {
            virtual_acts_cond_label_onAdd_ creataBCreateButton(int actionId, string labelText)
            {
                ArrayBytes_Int acts = ArrayUtils.CreateInt();
                acts.push(actionId);
                var buttonConfig = new virtual_acts_cond_label_
                {
                    acts = acts,
                    label = Lang.Class.t.get(labelText.ToHaxeString(), null),
                    cond = null
                };
                return buttonConfig.ToVirtual<virtual_acts_cond_label_onAdd_>();
            }

            ArrayObj btns = (ArrayObj)ArrayUtils.CreateDyn().array;
            btns.push(creataBCreateButton(14, "Valider"));
            btns.push(creataBCreateButton(16, "Retour"));
            btns.push(creataBCreateButton(KeyHelper.X, "X"));
            btns.push(creataBCreateButton(KeyHelper.C, "C"));


            createControlLabel(btns);
        }


    }
}