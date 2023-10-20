using Aki.Reflection.Patching;
using BepInEx;
using BepInEx.Configuration;
using EFT;
using System;
using System.Diagnostics;
using System.Reflection;
using VersionChecker;
using static VersionChecker.TarkovVersion;

namespace Deminvincibility
{
    [BepInPlugin("com.hazelify.deminvincibility", "Deminvincibility", "1.0.0")]
    public class DeminvicibilityPlugin : BaseUnityPlugin
    {
        private static bool placeholder = true;
        private static string credits = "Thanks Props <3 Ily https://github.com/dvize/DadGamerMode";

        public static ConfigEntry<Boolean> Keep1Health
        {
            get; set;
        }
        public static ConfigEntry<string> Keep1HealthSelection
        {
            get; set;
        }
        public string[] Keep1HealthSelectionList = new string[] { "All", "Head And Thorax" };
        public static ConfigEntry<int> CustomDamageModeVal
        {
            get; set;
        }

        public static ConfigEntry<Boolean> hpDeathBool
        {
            get; set;
        }
        public static ConfigEntry<string> hpDeath
        {
            get; set;
        }

        public string[] hpList = new string[] { "15", "50 HP", "100 HP", "150 HP"};

        private void Awake()
        {
            // checkSPTVersion();

            // - Keep1Health section
            if (placeholder)
            {
                Keep1Health = Config.Bind("1. Health", "Keep 1 Health", false,
                    new ConfigDescription("Enable to keep yourself from dying",
                    null,
                    new ConfigurationManagerAttributes { IsAdvanced = false, Order = 6 }));

                // Keep1HealthSelection = Config.Bind("1. Health", "Keep 1 Health Selection", "All",
                //     new ConfigDescription("Select which body parts to keep above 1 health",
                // new AcceptableValueList<string>(Keep1HealthSelectionList),
                // new ConfigurationManagerAttributes { IsAdvanced = false, Order = 5 }));

                // CustomDamageModeVal = Config.Bind("1. Health", "% Damage received", 100, new ConfigDescription("Set perceived damage in percent",
                // new AcceptableValueRange<int>(1, 100), new ConfigurationManagerAttributes { IsAdvanced = false, ShowRangeAsPercent = true, Order = 3 }));
            }

            // Protection disable
            if (placeholder)
            {
                // hpDeathBool = Config.Bind("2. Death", "Enable hp-based protection?", false,
                // new ConfigDescription("If enabled, if your health goes below the certain value specified, protection will stop working.\n\nThis will disable \"Keep 1 Health\"",
                // null,
                // new ConfigurationManagerAttributes { IsAdvanced = false, Order = 4 }));

                // hpDeath = Config.Bind("2. Death", "Disable protection below:", "15 HP",
                // new ConfigDescription("",
                // new AcceptableValueList<string>(hpList),
                // new ConfigurationManagerAttributes { IsAdvanced = false, Order = 3 }));
            }

            // Patches
            new NewGamePatch().Enable();
            // new Deminvincibility.Patches.ApplyDamage().Enable();
            // new Deminvincibility.Patches.ChangeHealth().Enable();
            new Deminvincibility.Patches.Kill().Enable();
            // new Deminvincibility.Patches.DestroyBodyPartPatch().Enable();
        }

        private void checkSPTVersion()
        {
            int currentVersion = FileVersionInfo.GetVersionInfo(BepInEx.Paths.ExecutablePath).FilePrivatePart;
            int buildVersion = TarkovVersion.BuildVersion;
            if (currentVersion != buildVersion)
            {
                Logger.LogError($"ERROR: This version of {Info.Metadata.Name} v{Info.Metadata.Version} was built for Tarkov {buildVersion}, but you are running {currentVersion}. Please download the correct plugin version.");
                EFT.UI.ConsoleScreen.LogError($"ERROR: This version of {Info.Metadata.Name} v{Info.Metadata.Version} was built for Tarkov {buildVersion}, but you are running {currentVersion}. Please download the correct plugin version.");
                throw new Exception($"Invalid EFT Version ({currentVersion} != {buildVersion})");
            }
        }
        internal class NewGamePatch : ModulePatch
        {
            protected override MethodBase GetTargetMethod() => typeof(GameWorld).GetMethod(nameof(GameWorld.OnGameStarted));

            [PatchPrefix]
            private static void PatchPrefix()
            {
            }
        }
    }
}
