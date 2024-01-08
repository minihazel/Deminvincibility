using System;
using System.Diagnostics;
using BepInEx;
using BepInEx.Configuration;
using Deminvincibility.Model;
using Deminvincibility.Patches;
using EFT.UI;
using static VersionChecker.TarkovVersion;

namespace Deminvincibility
{
    [BepInPlugin("com.hazelify.deminvincibility", "Deminvincibility", "1.4.0")]
    public class DeminvicibilityPlugin : BaseUnityPlugin
    {
        private static bool placeholder = true;
        private static string credits = "Thanks Props <3 Ily https://github.com/dvize/DadGamerMode";

        public static ConfigEntry<bool> Keep1Health { get; set; }
        public static ConfigEntry<bool> Allow0HpLimbs { get; set; }
        public static ConfigEntry<bool> AllowBlacking { get; set; }
        public static ConfigEntry<bool> AllowBlackingHeadAndThorax { get; set; }
        // public static ConfigEntry<string> Keep1HealthSelection { get; set; }
        // public string[] Keep1HealthSelectionList = new string[] { "All", "Head & Thorax" };
        public static ConfigEntry<bool> MedicineBool { get; set; }
        public static ConfigEntry<int> CustomDamageModeVal { get; set; }
        public static ConfigEntry<bool> SecondChanceProtection { get; set; }
        public static ConfigEntry<bool> SecondChanceEffectRemoval { get; set; }
        public static ConfigEntry<SecondChanceRestoreEnum> SecondChanceHealthRestoreAmount { get; set; }
        // public static ConfigEntry<string> hpDeath { get; set; }
        // public string[] hpList = { "15", "50 HP", "100 HP", "150 HP" };

        private void Awake()
        {
            // CheckSPTVersion();
            InitConfig();
            ApplyPatches();
        }

        private void ApplyPatches()
        {
            // new NewGamePatch().Enable();
            new DestroyBodyPartPatch().Enable();
            new ApplyDamage().Enable();
            new DoFracture().Enable();
            new Kill().Enable();
        }

        private void InitConfig()
        {
            // - Keep1Health section
            if (placeholder)
            {
                Keep1Health = Config.Bind("1. Health", "1 HP Mode", false,
                    new ConfigDescription("Enable to keep yourself from dying",
                    null,
                    new ConfigurationManagerAttributes { IsAdvanced = false, Order = 7 }));

                Allow0HpLimbs = Config.Bind("1. Health", "Allow 0hp on limbs", false,
                    new ConfigDescription("If enabled, Keep 1 Health on will allow arms and legs to hit 0 hp.",
                    null,
                    new ConfigurationManagerAttributes { IsAdvanced = false, Order = 6 }));

                AllowBlacking = Config.Bind("1. Health", "Allow blacking of limbs", false,
                    new ConfigDescription("If enabled, Keep 1 Health on will cause blacking of limbs when they reach 0hp.",
                    null,
                    new ConfigurationManagerAttributes { IsAdvanced = false, Order = 5 }));

                AllowBlackingHeadAndThorax = Config.Bind("1. Health", "Allow blacking of Head & Thorax", false,
                    new ConfigDescription("If enabled, Head & Thorax will be blacked out when they reach 0hp. This setting is ignored if \'Allow blacking of limbs\' is disabled.",
                        null,
                        new ConfigurationManagerAttributes { IsAdvanced = false, Order = 4 }));

                /*
                Keep1HealthSelection = Config.Bind("1. Health", "Keep 1 Health Selection", "All",
                    new ConfigDescription("Select which body parts to keep above 1 health",
                    new AcceptableValueList<string>(Keep1HealthSelectionList),
                    new ConfigurationManagerAttributes { IsAdvanced = false, Order = 5 }));
                */

                MedicineBool = Config.Bind("1. Health", "Ignore health side effects?", false,
                    new ConfigDescription("If enabled, fractures, bleeds and other forms of side effects to your health will be auto-healed.\n\nPSA: Disabling this could cause unintended side effects, use caution.",
                    null,
                    new ConfigurationManagerAttributes { IsAdvanced = false, Order = 3 }));

                CustomDamageModeVal = Config.Bind("1. Health", "% Damage received", 100, new ConfigDescription("Set perceived damage in percent",
                new AcceptableValueRange<int>(1, 100), new ConfigurationManagerAttributes { IsAdvanced = false, ShowRangeAsPercent = true, Order = 2 }));
            }

            // Protection disable
            if (placeholder)
            {
                SecondChanceProtection = Config.Bind("2. Death", "Enable second chance protection?", false,
                    new ConfigDescription("If enabled, if you take a hit that would normally kill you, you\'ll be given a second chance to survive.",
                    null,
                    new ConfigurationManagerAttributes { IsAdvanced = false, Order = 3 }));

                SecondChanceEffectRemoval = Config.Bind("2. Death", "Second chance health effect removal", true,
                    new ConfigDescription("If enabled, all negative health effects (bleeds, fractures, etc.) will be removed from your character when second chance triggers.",
                        null,
                        new ConfigurationManagerAttributes { IsAdvanced = false, Order = 2 }));

                SecondChanceHealthRestoreAmount = Config.Bind("2. Death", "Second chance health restore on critical limbs", SecondChanceRestoreEnum.OneHealth,
                    new ConfigDescription("Choose what HP should be set on the Head and Thorax when second chance protection triggers. Will never remove health if the limb has more than the set amount. \n\n\'None\' and \'1HP\' options are risky if effect removal is disabled.", 
                        null,
                        new ConfigurationManagerAttributes { IsAdvanced = false, Order = 1 }));

                // hpDeath = Config.Bind("2. Death", "Disable protection below:", "15 HP",
                // new ConfigDescription("",
                // new AcceptableValueList<string>(hpList),
                // new ConfigurationManagerAttributes { IsAdvanced = false, Order = 1 }));
            }
        }

        private void CheckSPTVersion()
        {
            var currentVersion = FileVersionInfo.GetVersionInfo(BepInEx.Paths.ExecutablePath).FilePrivatePart;
            var buildVersion = BuildVersion;

            if (currentVersion != buildVersion)
            {
                Logger.LogError($"ERROR: This version of {Info.Metadata.Name} v{Info.Metadata.Version} was built for Tarkov {buildVersion}, but you are running {currentVersion}. Please download the correct plugin version.");
                ConsoleScreen.LogError($"ERROR: This version of {Info.Metadata.Name} v{Info.Metadata.Version} was built for Tarkov {buildVersion}, but you are running {currentVersion}. Please download the correct plugin version.");
                throw new Exception($"Invalid EFT Version ({currentVersion} != {buildVersion})");
            }
        }

        // internal class NewGamePatch : ModulePatch
        // {
        //     protected override MethodBase GetTargetMethod() => typeof(GameWorld).GetMethod(nameof(GameWorld.OnGameStarted));
        //
        //     [PatchPrefix]
        //     private static void PatchPrefix()
        //     {
        //     }
        // }
    }
}
