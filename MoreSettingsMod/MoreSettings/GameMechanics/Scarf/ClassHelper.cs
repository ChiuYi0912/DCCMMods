using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLibrary.Core.Extensions;
using dc.ui;
using Hashlink.Virtuals;
using ScarfData = Hashlink.Virtuals.virtual_attachOffX_attachOffY_color_cosOffset_count_extraSprLength_friction_gravity_maxLength_minLength_onFront_props_sprId_thickness_;


namespace MoreSettings.GameMechanics.Scarf
{
    public class AttributeEntry
    {
        public string Name = default!;
        public Func<ScarfData, object> Getter = default!;
        public Action<ScarfData, object> Setter = default!;
        public System.Type ValueType = default!;
        public FlowBox Box = default!;
        public dc.ui.Text Text = default!;
    }

    public class PropEntry
    {
        public string Name = default!;
        public Func<object> Getter = default!;
        public Action<object> Setter = default!;
        public Type ValueType = default!;
        public FlowBox Box = default!;
        public dc.ui.Text Text = default!;
    }


    public class ScarfList
    {
        public List<AttributeEntry> all = new();
        public List<AttributeEntry> left = new();
        public List<AttributeEntry> right = new();

    }

    public class ScarfListInitialisation
    {
        public ScarfList baseAttributes = new();
        public ScarfList propsAttributes = new();

        public ScarfData scarfData = null!;

        public void InitAttributes()
        {
            if (scarfData == null)
            {
                scarfData = new ScarfData();
                scarfData.props = new virtual_backColor_customAttach_depthScaleFactor_isCape_linkTo_lockBehind_oscilFactor_rotScale_();

                scarfData.props.backColor = null;
                scarfData.props.customAttach = "".ToHaxeString();
                scarfData.props.depthScaleFactor = 1.0;
                scarfData.props.isCape = false;
                scarfData.props.linkTo = null;
                scarfData.props.lockBehind = false;
                scarfData.props.oscilFactor = 0.0;
                scarfData.props.rotScale = 1.0;


                scarfData.attachOffX = 0.0;
                scarfData.attachOffY = 0.0;
                scarfData.color = 0x801234;
                scarfData.cosOffset = 0;
                scarfData.count = 11;
                scarfData.extraSprLength = null;
                scarfData.friction = 0.6;
                scarfData.gravity = 0.9;
                scarfData.maxLength = 4.0;
                scarfData.minLength = 3.0;
                scarfData.onFront = false;
                scarfData.sprId = "scarfGray".ToHaxeString();
                scarfData.thickness = 1.0;
            }


            baseAttributes.all = new List<AttributeEntry>
            {
                new AttributeEntry { Name = "attachOffX", ValueType = typeof(double), Getter = (d) => d.attachOffX, Setter = (d, v) => d.attachOffX = (double)v },
                new AttributeEntry { Name = "attachOffY", ValueType = typeof(double), Getter = (d) => d.attachOffY, Setter = (d, v) => d.attachOffY = (double)v },
                new AttributeEntry { Name = "color", ValueType = typeof(int?), Getter = (d) => d.color!, Setter = (d, v) => d.color = (int?)v },
                new AttributeEntry { Name = "cosOffset", ValueType = typeof(double), Getter = (d) => d.cosOffset, Setter = (d, v) => d.cosOffset = (double)v },
                new AttributeEntry { Name = "count", ValueType = typeof(int), Getter = (d) => d.count, Setter = (d, v) => d.count = (int)v },
                new AttributeEntry { Name = "extraSprLength", ValueType = typeof(int?), Getter = (d) => d.extraSprLength!, Setter = (d, v) => d.extraSprLength = (int?)v },
                new AttributeEntry { Name = "friction", ValueType = typeof(double), Getter = (d) => d.friction, Setter = (d, v) => d.friction = (double)v },
                new AttributeEntry { Name = "gravity", ValueType = typeof(double), Getter = (d) => d.gravity, Setter = (d, v) => d.gravity = (double)v },
                new AttributeEntry { Name = "maxLength", ValueType = typeof(double), Getter = (d) => d.maxLength, Setter = (d, v) => d.maxLength = (double)v },
                new AttributeEntry { Name = "minLength", ValueType = typeof(double), Getter = (d) => d.minLength, Setter = (d, v) => d.minLength = (double)v },
                new AttributeEntry { Name = "onFront", ValueType = typeof(bool), Getter = (d) => d.onFront, Setter = (d, v) => d.onFront = (bool)v },
                new AttributeEntry { Name = "sprId", ValueType = typeof(dc.String), Getter = (d) => d.sprId, Setter = (d, v) => d.sprId = (dc.String)v },
                new AttributeEntry { Name = "thickness", ValueType = typeof(double), Getter = (d) => d.thickness, Setter = (d, v) => d.thickness = (double)v },
                new AttributeEntry
                {
                    Name = "props",
                    ValueType = typeof(virtual_backColor_customAttach_depthScaleFactor_isCape_linkTo_lockBehind_oscilFactor_rotScale_),
                    Getter = (d) => {
                        return "8项设置";
                    },
                    Setter = (d, v) => {

                    },
                }
            };


            propsAttributes.all = new List<AttributeEntry>
            {
                new AttributeEntry
                {
                    Name = "backColor",
                    ValueType = typeof(int?),
                    Getter = d => d.props.backColor ?? 0,
                    Setter = (d, v) => {
                        d.props.backColor = (int?)v;
                    }
                },
                new AttributeEntry
                {
                    Name = "customAttach",
                    ValueType = typeof(dc.String),
                    Getter = d => d.props.customAttach,
                    Setter = (d, v) => {
                        d.props.customAttach = (dc.String)v;
                    }
                },
                new AttributeEntry
                {
                    Name = "depthScaleFactor",
                    ValueType = typeof(double),
                    Getter = d => d.props.depthScaleFactor ?? 1.0,
                    Setter = (d, v) => {
                        d.props.depthScaleFactor = (double)v;
                    }
                },
                new AttributeEntry
                {
                    Name = "isCape",
                    ValueType = typeof(bool),
                    Getter = d => d.props.isCape ?? false,
                    Setter = (d, v) => {
                        d.props.isCape = (bool)v;
                    }
                },
                new AttributeEntry
                {
                    Name = "linkTo",
                    ValueType = typeof(int?),
                    Getter = d => d.props.linkTo!,
                    Setter = (d, v) => {
                        d.props.linkTo = (int?)v;
                    }
                },
                new AttributeEntry
                {
                    Name = "lockBehind",
                    ValueType = typeof(bool),
                    Getter = d => d.props.lockBehind ?? false,
                    Setter = (d, v) => {
                        d.props.lockBehind = (bool)v;
                    }
                },
                new AttributeEntry
                {
                    Name = "oscilFactor",
                    ValueType = typeof(double),
                    Getter = d => d.props.oscilFactor ?? 0.0,
                    Setter = (d, v) => {
                        d.props.oscilFactor = (double)v;
                    }
                },
                new AttributeEntry
                {
                    Name = "rotScale",
                    ValueType = typeof(double),
                    Getter = d => d.props?.rotScale ?? 1.0,
                    Setter = (d, v) => {
                        d.props.rotScale = (double)v;
                    }
                }
            };


            int half = baseAttributes.all.Count / 2;
            baseAttributes.left = baseAttributes.all.Take(half).ToList();
            baseAttributes.right = baseAttributes.all.Skip(half).ToList();

            int halfprops = propsAttributes.all.Count / 2;
            propsAttributes.left = propsAttributes.all.Take(halfprops).ToList();
            propsAttributes.right = propsAttributes.all.Skip(halfprops).ToList();
        }
    }
}