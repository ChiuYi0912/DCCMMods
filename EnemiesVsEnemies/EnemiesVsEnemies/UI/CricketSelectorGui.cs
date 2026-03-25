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
        public ScaleGrid selectionTeam = null!;
        public UITextHelper textHelper = null!;
        public Mask rightMask = null!;
        private Interactive rightInter = null!;

        public Config<ModConfig> GetConfig = EnemiesVsEnemiesMod.config;

        public static bool isLockedController = false;
        public string UserSelectedteamid = null!;

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


            base.mainFlow.addChild(rightFlow);


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

            Tile tile = Assets.Class.ui.getTile("boxSelect".ToHaxeString(), Ref<int>.Null, Ref<double>.Null, Ref<double>.Null, null);
            selectionTeam = new ScaleGrid(tile, 8, 8, rightMask);
            selectionTeam.alpha = 0;


            textHelper.AddConfigInfoToRightFlow();
        }

        private void OnRightWheel(Event e)
        {
            if (controller == null) return;
            if (controller.parent.exclusiveId != controller.id) return;

            double delta = e.wheelDelta;
            if (delta == 0) return;

            const double step = 20;

            double maxScroll = Math.Max(0, rightFlow.get_outerHeight() - rightMask.height);
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


            double rightX = fbItems.x + fbWidth + 20 * pixelScale;
            double rightY = fbItems.y;

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



        public override dc.h2d.Object getIconBmp(int i, dc.h2d.Object parent)
        {
            string name = UIMobHelper.getmobs(i).id.ToString();

            var icon = Icon.Class.createMobIcon(name.ToHaxeString(), parent);
            icon.tile.scaleToSize(get_entryWid(), get_entryHei());

            return icon;
        }

        public override void postUpdate()
        {
            Main main = Main.Class.ME;
            Point entryGlobal = main.localToGlobal(this.getEntryAt(curX, curY).f, Ref<double>.Null, Ref<double>.Null);
            Point maskGlobal = main.localToGlobal(this.mask, Ref<double>.Null, Ref<double>.Null);


            double offset = (int)(get_pixelScale.Invoke() * 5.0);
            ScaleGrid selection = this.selectionSG;
            selection.posChanged = true;
            selection.x = entryGlobal.x - maskGlobal.x - offset;
            selection.y = entryGlobal.y - maskGlobal.y - offset;


            if (UserSelectedteamid.IsNullOrEmpty())
                return;

            UptDateSelectionTeam();
        }

        public void UptDateSelectionTeam()
        {
            double timeFactor = base.ftime * 0.1;
            string speedKey = "co_blinkCursorSpeed";

            var speedData = Data.Class.gui.byId.get(speedKey.ToHaxeString()).v0;

            var angle = timeFactor * speedData;
            var cosValue = System.Math.Cos(angle);
            var alphaOffset = 0.2 * cosValue;

            this.selectionSG.alpha = 0.8 + alphaOffset;

            if (!textHelper.AlluiText.ContainsKey(UserSelectedteamid))
            {
                selectionTeam.alpha = 0;
                return;
            }

            if (textHelper.AlluiText.TryGetValue(UserSelectedteamid, out var text))
            {
                Point oldflowGlobal = Main.Class.ME.localToGlobal(text, Ref<double>.Null, Ref<double>.Null);
                Point localPos = rightMask.globalToLocal(oldflowGlobal);


                double scaleX = text.scaleX;
                double scaleY = text.scaleY;
                double actualWidth = text.textWidth * scaleX;
                double actualHeight = text.textHeight * scaleY;


                double padding = get_pixelScale.Invoke() * 10;
                double boxWidth = actualWidth + padding;
                double boxHeight = actualHeight + padding / 6;


                selectionTeam.set_width((int)boxWidth);
                selectionTeam.set_height((int)boxHeight);


                double offsetX = (boxWidth - actualWidth) / 2;
                double offsetY = (boxHeight - actualHeight) / 2;


                selectionTeam.x = localPos.x - offsetX;
                selectionTeam.y = localPos.y - offsetY;

                selectionTeam.alpha = 0.8 + alphaOffset;

                selectionTeam.posChanged = true;
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


            btns.push(creataBCreateButton(1, "添加新队伍"));
            btns.push(creataBCreateButton(3, "添加仇恨队伍"));
            btns.push(creataBCreateButton(4, ""));
            btns.push(creataBCreateButton(5, "移除当前选择队伍"));

            createControlLabel(btns);
        }

        public override bool controlsUpdate()
        {
            if (isLockedController)
                return false;

            Controller parent = controller.parent;


            if (ControllerHelper.ControlsUpdateFromProcess(parent, 5))
            {
                textHelper.RemoveTeamFromGui(UserSelectedteamid, teamManager);
            }

            if (ControllerHelper.ControlsUpdateFromProcess(parent, 1))
            {
                textHelper.AddNewTeamFromGui(teamManager);
                return true;
            }

            if (ControllerHelper.ControlsUpdateFromProcess(parent, 3))
            {
                textHelper.AddOpposingTeamFromGui(teamManager);
                return true;
            }


            return base.controlsUpdate();
        }
    }
}