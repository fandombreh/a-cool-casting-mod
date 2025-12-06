using BepInEx;
using BepInEx.Configuration;
using GorillaTag.Cosmetics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using z3roCastingMod.Librarys;
using static sigmarizz.Class1;

namespace sigmarizz
{
    internal class Saves
    {
        public static ConfigFile cfgFile;
        public static ConfigFile cfgFile2;
        public static bool LoadingFont;
        public static void cfg()
        {
            cfgFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "castingsave".ToLower().Replace(" ", "") + ".CastingSave"), true);
            fov = cfgFile.Bind<float>("Settings", "fov", 60f);
            riglerping = cfgFile.Bind<bool>("Settings", "rig lerping", false);
            HideCastUI = cfgFile.Bind<bool>("Settings", "Hide Cast UI", false);
            SelfCast = cfgFile.Bind<bool>("Settings", "Force Self Cast", false);
            SelfLerp = cfgFile.Bind<bool>("Settings", "SelfLerp", false);
            LerpAmount = cfgFile.Bind<float>("Settings", "Lerp Amount", 0.18f);
            HeightOffset = cfgFile.Bind<float>("Settings", "Height Offset", 0f);
            MotionSmoothing = cfgFile.Bind<bool>("Settings", "Motion Smoothing", false);
            NametagsToCamera = cfgFile.Bind<bool>("Settings", "Nametags Face Camera", false);
            ShowPlatform = cfgFile.Bind<bool>("Settings", "Show Platform", false);
            PosSmooth = cfgFile.Bind<float>("Settings", "Position Smoothing", 0.6f);
            RotSmooth = cfgFile.Bind<float>("Settings", "Rotation Smoothing", 0.6f);
            CameraDistance = cfgFile.Bind<float>("Settings", "Camera Distance", -2f);
            FontIndex = cfgFile.Bind<int>("Settings", "Nametag Font", 0);
            AutoPilot = cfgFile.Bind<bool>("Settings", "Auto Pilot", false);
            HeadMounted = cfgFile.Bind<bool>("Settings", "Head Mounted", false);
            FirstPerson = cfgFile.Bind<bool>("Settings", "First Person", false);
            RigExa = cfgFile.Bind<bool>("Settings", "Rig Exaggration", false);
            ExaAmount = cfgFile.Bind<float>("Settings", "Exaggeration Amount", 0.02f);
            ExaAmount = cfgFile.Bind<float>("Settings", "Near Clip", 0.3f);
            NameTagScale = cfgFile.Bind<float>("Settings", "NametagScale", 0.4f);
            NameTagHeight = cfgFile.Bind<float>("Settings", "NametagHeight", 0.4f);

            bodyLerp = cfgFile.Bind<float>("Settings", "Body Lerp", 0.05f);
            HeadLerp = cfgFile.Bind<float>("Settings", "Head Lerp", 0.05f);
            handLerp = cfgFile.Bind<float>("Settings", "Hand Lerp", 0.05f);

            ShowFPS = cfgFile.Bind<bool>("Settings", "Show FPS", false);
            HeadRot = cfgFile.Bind<bool>("Settings", "Follow Head Rotation", false);
            Nametags = cfgFile.Bind<bool>("Settings", "Name Tags", false);
            NameTagFont = cfgFile.Bind<string>("Settings", "Font", "Firebird");


        }

       

        public static void Save()
        {
            cfg();
            fov.Value = cc.FieldOfView;
            riglerping.Value = rl;
            LerpAmount.Value = la;
            CameraDistance.Value = cc.CameraDistance;
            HeightOffset.Value = cc.CameraHeight;
            MotionSmoothing.Value = Class1.MS;
            PosSmooth.Value = cc.MotionSmoothing;
            RotSmooth.Value = cc.RotationSmoothing;
            Nametags.Value = Class1.Nametags;
            NameTagFont.Value = Class1.NameTagLoadFix;
            ShowFPS.Value = Class1.ShowFPS;
            NearClip.Value = cc.NearClip;
            cfgFile.Save();
        }
        public static void Load()
        {
            cfg();
            cc.FieldOfView = fov.Value;
            rl = riglerping.Value;
            la = LerpAmount.Value;
            cc.CameraDistance = CameraDistance.Value;
            cc.CameraHeight = HeightOffset.Value;
            Class1.MS = MotionSmoothing.Value;
            Class1.Slider2 = PosSmooth.Value;
            cc.RotationSmoothing = RotSmooth.Value;
            Class1.NT = Nametags.Value;
            Class1.ShowFPS = ShowFPS.Value;
            Class1.NameTagLoadFix = NameTagFont.Value;

            if (NameTagFont.Value == "Sakana")
            {
                Class1.CurrentFont = Class1.sakana;
            }
           
            if (NameTagFont.Value == "Firebird")
            {
                Class1.CurrentFont = Class1.firebird;
            }
            
            if (NameTagFont.Value == "Western")
            {
                Class1.CurrentFont = Class1.western;
            }

        }

        public static ConfigEntry<float> fov;
        public static ConfigEntry<bool> riglerping;
        public static ConfigEntry<bool> RigExa;
        public static ConfigEntry<bool> HideCastUI;
        public static ConfigEntry<bool> SelfCast;
        public static ConfigEntry<bool> MotionSmoothing;
        public static ConfigEntry<bool> NametagsToCamera;
        public static ConfigEntry<bool> ShowPlatform;
        public static ConfigEntry<bool> AutoPilot;
        public static ConfigEntry<bool> HeadMounted;
        public static ConfigEntry<bool> FirstPerson;
        public static ConfigEntry<bool> HeadRot;
        public static ConfigEntry<bool> ShowFPS;
        public static ConfigEntry<bool> Nametags;
        public static ConfigEntry<float> LerpAmount;
        public static ConfigEntry<float> HeightOffset;
        public static ConfigEntry<float> PosSmooth;
        public static ConfigEntry<float> RotSmooth;
        public static ConfigEntry<float> CameraDistance;
        public static ConfigEntry<float> NearClip;
        public static ConfigEntry<float> NameTagHeight;
        public static ConfigEntry<float> NameTagScale;

        public static ConfigEntry<bool> SelfLerp;
        public static ConfigEntry<float> bodyLerp;
        public static ConfigEntry<float> handLerp;
        public static ConfigEntry<float> HeadLerp;

        public static ConfigEntry<float> ExaAmount;
        public static ConfigEntry<int> FontIndex;
        public static ConfigEntry<string> NameTagFont;

        public static ConfigEntry<float> red;
        public static ConfigEntry<float> blue;
        public static ConfigEntry<float> green;

    }
}