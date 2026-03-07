using System;

namespace EquipWeapons
{
    public static class BoneNames
    {
        private const string Bip001LToe0 = "Bip001 L Toe0";
        private const string Bip001RFinger41 = "Bip001 R Finger41";
        private const string Bip001RFinger32 = "Bip001 R Finger32";
        private const string Bip001RFinger42 = "Bip001 R Finger42";
        private const string Bip001LForearm = "Bip001 L Forearm";
        private const string Bip001RFoot = "Bip001 R Foot";
        private const string Bip001RFinger1Nub = "Bip001 R Finger1Nub";
        private const string Bip001RFinger0Nub = "Bip001 R Finger0Nub";
        private const string Bip001RFinger2Nub = "Bip001 R Finger2Nub";
        private const string Bip001Spine1 = "Bip001 Spine1";
        private const string Bip001RCalf = "Bip001 R Calf";
        private const string Bip001RFinger3Nub = "Bip001 R Finger3Nub";
        private const string Bip001RThigh = "Bip001 R Thigh";
        private const string Bip001RFinger4Nub = "Bip001 R Finger4Nub";
        private const string Bip001Spine2 = "Bip001 Spine2";
        private const string HeadBone = "headBone";
        private const string Bip001RClavicle = "Bip001 R Clavicle";
        private const string Bip001RHand = "Bip001 R Hand";
        private const string Bip001RUpperArm = "Bip001 R UpperArm";
        private const string Bip001Pelvis = "Bip001 Pelvis";
        private const string Bip001LFinger01 = "Bip001 L Finger01";
        private const string Bip001LToe0Nub = "Bip001 L Toe0Nub";
        private const string Bip001LFinger11 = "Bip001 L Finger11";
        private const string Bip001LFinger0 = "Bip001 L Finger0";
        private const string Bip001LFinger02 = "Bip001 L Finger02";
        private const string Bip001LFinger21 = "Bip001 L Finger21";
        private const string Bip001LFinger12 = "Bip001 L Finger12";
        private const string Bip001LFinger1 = "Bip001 L Finger1";
        private const string Bip001LFinger31 = "Bip001 L Finger31";
        private const string Bip001LFinger22 = "Bip001 L Finger22";
        private const string Bip001LFinger2 = "Bip001 L Finger2";
        private const string Bip001LFinger3 = "Bip001 L Finger3";
        private const string Bip001LThigh = "Bip001 L Thigh";
        private const string Bip001RForearm = "Bip001 R Forearm";
        private const string Bip001LFinger41 = "Bip001 L Finger41";
        private const string Bip001LFinger32 = "Bip001 L Finger32";
        private const string Bip001LFinger4 = "Bip001 L Finger4";
        private const string Bip001LFinger42 = "Bip001 L Finger42";
        private const string Bip001Head = "Bip001 Head";
        private const string Bone001 = "Bone001";
        private const string Bone002 = "Bone002";
        private const string Bip001RToe0 = "Bip001 R Toe0";
        private const string Bone003 = "Bone003";
        private const string Bip001LClavicle = "Bip001 L Clavicle";
        private const string Bone004 = "Bone004";
        private const string Bip001Spine = "Bip001 Spine";
        private const string Bone005 = "Bone005";
        private const string Bip001LFoot = "Bip001 L Foot";
        private const string Bip001LFinger0Nub = "Bip001 L Finger0Nub";
        private const string Bip001Prop1 = "Bip001 Prop1";
        private const string Bip001LFinger1Nub = "Bip001 L Finger1Nub";
        private const string Bone006 = "Bone006";
        private const string Bip001LUpperArm = "Bip001 L UpperArm";
        private const string Bip001Prop2 = "Bip001 Prop2";
        private const string Bip001RToe0Nub = "Bip001 R Toe0Nub";
        private const string Bip001LFinger2Nub = "Bip001 L Finger2Nub";
        private const string Bip001RFinger0 = "Bip001 R Finger0";
        private const string Bip001LFinger3Nub = "Bip001 L Finger3Nub";
        private const string Bip001LCalf = "Bip001 L Calf";
        private const string WeaponBone0 = "weaponBone0";
        private const string Bip001LFinger4Nub = "Bip001 L Finger4Nub";
        private const string Bip001RFinger1 = "Bip001 R Finger1";
        private const string Bip001RFinger2 = "Bip001 R Finger2";
        private const string WeaponBone1 = "weaponBone1";
        private const string Bip001RFinger3 = "Bip001 R Finger3";
        private const string Bip001RFinger01 = "Bip001 R Finger01";
        private const string Bip001RFinger4 = "Bip001 R Finger4";
        private const string Bip001RFinger02 = "Bip001 R Finger02";
        private const string Bip001RFinger11 = "Bip001 R Finger11";
        private const string Bip001LHand = "Bip001 L Hand";
        private const string Bip001RFinger12 = "Bip001 R Finger12";
        private const string Bip001RFinger21 = "Bip001 R Finger21";
        private const string Bip001RFinger22 = "Bip001 R Finger22";
        private const string Bip001RFinger31 = "Bip001 R Finger31";
        private const string Bip001Neck = "Bip001 Neck";

        // 分类结构
        public static class SpineBones
        {
            public const string Main = Bip001Spine;
            public const string Spine1 = Bip001Spine1;
            public const string Spine2 = Bip001Spine2;
        }

        public static class HeadBones
        {
            public const string Main = Bip001Head;
            public const string Bone = "headBone";
        }

        public static class NeckBones
        {
            public const string Main = Bip001Neck;
        }

        public static class PelvisBones
        {
            public const string Main = Bip001Pelvis;
        }

        public static class ArmBones
        {
            public static class Left
            {
                public const string UpperArm = Bip001LUpperArm;
                public const string Forearm = Bip001LForearm;
                public const string Clavicle = Bip001LClavicle;
            }

            public static class Right
            {
                public const string UpperArm = Bip001RUpperArm;
                public const string Forearm = Bip001RForearm;
                public const string Clavicle = Bip001RClavicle;
            }
        }

        public static class LegBones
        {
            public static class Left
            {
                public const string Thigh = Bip001LThigh;
                public const string Calf = Bip001LCalf;
            }

            public static class Right
            {
                public const string Thigh = Bip001RThigh;
                public const string Calf = Bip001RCalf;
            }
        }

        public static class HandBones
        {
            public static class Left
            {
                public const string Main = Bip001LHand;
            }

            public static class Right
            {
                public const string Main = Bip001RHand;
            }
        }

        public static class FootBones
        {
            public static class Left
            {
                public const string Main = Bip001LFoot;
            }

            public static class Right
            {
                public const string Main = Bip001RFoot;
            }
        }

        public static class FingerBones
        {
            public static class Left
            {
                public const string Finger0 = Bip001LFinger0;
                public const string Finger1 = Bip001LFinger1;
                public const string Finger2 = Bip001LFinger2;
                public const string Finger3 = Bip001LFinger3;
                public const string Finger4 = Bip001LFinger4;
                public const string Finger0Nub = Bip001LFinger0Nub;
                public const string Finger1Nub = Bip001LFinger1Nub;
                public const string Finger2Nub = Bip001LFinger2Nub;
                public const string Finger3Nub = Bip001LFinger3Nub;
                public const string Finger4Nub = Bip001LFinger4Nub;
            }

            public static class Right
            {
                public const string Finger0 = Bip001RFinger0;
                public const string Finger1 = Bip001RFinger1;
                public const string Finger2 = Bip001RFinger2;
                public const string Finger3 = Bip001RFinger3;
                public const string Finger4 = Bip001RFinger4;

                public const string Finger0Nub = Bip001RFinger0Nub;
                public const string Finger1Nub = Bip001RFinger1Nub;
                public const string Finger2Nub = Bip001RFinger2Nub;
                public const string Finger3Nub = Bip001RFinger3Nub;
                public const string Finger4Nub = Bip001RFinger4Nub;
            }
        }

        public static class ToeBones
        {
            public static class Left
            {
                public const string Toe0 = Bip001LToe0;
                public const string Toe0Nub = Bip001LToe0Nub;
            }

            public static class Right
            {
                public const string Toe0 = Bip001RToe0;
                public const string Toe0Nub = Bip001RToe0Nub;
            }
        }

        public static class PropBones
        {
            public const string Prop1 = Bip001Prop1;
            public const string Prop2 = Bip001Prop2;
        }

        public static class WeaponBones
        {
            public const string Bone0 = "weaponBone0";
            public const string Bone1 = "weaponBone1";
        }

        public static class GenericBones
        {
            public const string Bone001 = "Bone001";
            public const string Bone002 = "Bone002";
            public const string Bone003 = "Bone003";
            public const string Bone004 = "Bone004";
            public const string Bone005 = "Bone005";
            public const string Bone006 = "Bone006";
        }
    }
}