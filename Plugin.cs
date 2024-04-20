using UnityEngine;
using System;
using System.Diagnostics;
using System.Reflection;
using Aki.Reflection.Patching;
using BepInEx;
using BepInEx.Configuration;
using Comfort.Common;
using EFT;
using System.Collections.Generic;

namespace MapFrames
{
    [BepInPlugin("com.hazelify.mapframes", "MapFrames", "1.0.0")]
    public class MapFramesPlugin : BaseUnityPlugin
    {
        private static bool placeholder = true;

        public static ConfigEntry<bool> toggleMod { get; set; }
        public static ConfigEntry<int> bigmap { get; set; }
        public static ConfigEntry<int> factory4_day { get; set; }
        public static ConfigEntry<int> factory4_night { get; set; }
        public static ConfigEntry<int> interchange { get; set; }
        public static ConfigEntry<int> laboratory { get; set; }
        public static ConfigEntry<int> lighthouse { get; set; }
        public static ConfigEntry<int> rezervbase { get; set; }
        public static ConfigEntry<int> shoreline { get; set; }
        public static ConfigEntry<int> tarkovstreets { get; set; }
        public static ConfigEntry<int> woods { get; set; }

        private void Awake()
        {
            InitConfig();
            new NewGamePatch().Enable();
        }

        private void InitConfig()
        {
            if (placeholder)
            {
                toggleMod = Config.Bind("A. Toggle mod", "Enable mod", false,
                    new ConfigDescription("Enable or disable the mod",
                    null,
                    new ConfigurationManagerAttributes { IsAdvanced = false, Order = 1 }));

                bigmap = Config.Bind("B. Framerates", "Customs", 60, new ConfigDescription("Set maximum framerate for Customs",
                new AcceptableValueRange<int>(30, 240), new ConfigurationManagerAttributes { IsAdvanced = false, ShowRangeAsPercent = false, Order = 1 }));

                factory4_day = Config.Bind("B. Framerates", "Factory Night", 60, new ConfigDescription("Set maximum framerate for Factory Day",
                new AcceptableValueRange<int>(30, 240), new ConfigurationManagerAttributes { IsAdvanced = false, ShowRangeAsPercent = false, Order = 2 }));

                factory4_night = Config.Bind("B. Framerates", "Factory Night", 60, new ConfigDescription("Set maximum framerate for Factory Night",
                new AcceptableValueRange<int>(30, 240), new ConfigurationManagerAttributes { IsAdvanced = false, ShowRangeAsPercent = false, Order = 3 }));

                interchange = Config.Bind("B. Framerates", "Interchange", 60, new ConfigDescription("Set maximum framerate for Interchange",
                new AcceptableValueRange<int>(30, 240), new ConfigurationManagerAttributes { IsAdvanced = false, ShowRangeAsPercent = false, Order = 4 }));

                laboratory = Config.Bind("B. Framerates", "Labs", 60, new ConfigDescription("Set maximum framerate for Labs",
                new AcceptableValueRange<int>(30, 240), new ConfigurationManagerAttributes { IsAdvanced = false, ShowRangeAsPercent = false, Order = 5 }));

                lighthouse = Config.Bind("B. Framerates", "Lighthouse", 60, new ConfigDescription("Set maximum framerate for Lighthouse",
                new AcceptableValueRange<int>(30, 240), new ConfigurationManagerAttributes { IsAdvanced = false, ShowRangeAsPercent = false, Order = 6 }));

                rezervbase = Config.Bind("B. Framerates", "Reserve", 60, new ConfigDescription("Set maximum framerate for Reserve",
                new AcceptableValueRange<int>(30, 240), new ConfigurationManagerAttributes { IsAdvanced = false, ShowRangeAsPercent = false, Order = 7 }));

                shoreline = Config.Bind("B. Framerates", "Snoreline", 60, new ConfigDescription("Set maximum framerate for Snoreline",
                new AcceptableValueRange<int>(30, 240), new ConfigurationManagerAttributes { IsAdvanced = false, ShowRangeAsPercent = false, Order = 8 }));

                tarkovstreets = Config.Bind("B. Framerates", "Streets of Tarkov", 60, new ConfigDescription("Set maximum framerate for Streets of Tarkov",
                new AcceptableValueRange<int>(30, 240), new ConfigurationManagerAttributes { IsAdvanced = false, ShowRangeAsPercent = false, Order = 9 }));

                woods = Config.Bind("B. Framerates", "Woods", 60, new ConfigDescription("Set maximum framerate for Woods",
                new AcceptableValueRange<int>(30, 240), new ConfigurationManagerAttributes { IsAdvanced = false, ShowRangeAsPercent = false, Order = 10 }));
            }
        }
    }
    internal class NewGamePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod() => typeof(GameWorld).GetMethod("OnGameStarted");

        [PatchPrefix]
        public static void PatchPrefix()
        {
            InRaidMono.Init();
        }
    }
    public class InRaidMono : MonoBehaviour
    {
        public int targetFramerate;
        public static InRaidMono Instance;

        public static void Init()
        {
            if (Singleton<GameWorld>.Instantiated)
            {
                GameWorld gameworld = Singleton<GameWorld>.Instance;
                Instance = gameworld.GetOrAddComponent<InRaidMono>();
                UnityEngine.Debug.Log($"MapFrames active on map {gameworld.LocationId}");

                switch (gameworld.LocationId.ToLower())
                {
                    case "bigmap":
                        Instance.targetFramerate = 120;
                        break;
                    case "factory4_day":
                        Instance.targetFramerate = 144;
                        break;
                    case "factory4_night":
                        Instance.targetFramerate = 144;
                        break;
                    case "interchange":
                        Instance.targetFramerate = 120;
                        break;
                    case "laboratory":
                        Instance.targetFramerate = 144;
                        break;
                    case "lighthouse":
                        Instance.targetFramerate = 60;
                        break;
                    case "rezervbase":
                        Instance.targetFramerate = 120;
                        break;
                    case "shoreline":
                        Instance.targetFramerate = 120;
                        break;
                    case "tarkovstreets":
                        Instance.targetFramerate = 60;
                        break;
                    case "woods":
                        Instance.targetFramerate = 120;
                        break;
                    default:
                        Instance.targetFramerate = 144;
                        break;
                }
            }
        }

        void Update()
        {
            Application.targetFrameRate = targetFramerate;
        }
    }
    internal sealed class ConfigurationManagerAttributes
    {
        public bool? ShowRangeAsPercent;
        public System.Action<BepInEx.Configuration.ConfigEntryBase> CustomDrawer;
        public CustomHotkeyDrawerFunc CustomHotkeyDrawer;
        public delegate void CustomHotkeyDrawerFunc(BepInEx.Configuration.ConfigEntryBase setting, ref bool isCurrentlyAcceptingInput);
        public bool? Browsable;
        public string Category;
        public object DefaultValue;
        public bool? HideDefaultButton;
        public bool? HideSettingName;
        public string Description;
        public string DispName;
        public int? Order;
        public bool? ReadOnly;
        public bool? IsAdvanced;
        public System.Func<object, string> ObjToStr;
        public System.Func<string, object> StrToObj;
    }
}
