using System;
using System.Reflection;
using Aki.Reflection.Patching;
using EFT;
using EFT.HealthSystem;
using HarmonyLib;

namespace Deminvincibility.Patches
{
    internal class DestroyBodyPartPatch : ModulePatch
    {
        // private static readonly EBodyPart[] critBodyParts = { EBodyPart.Stomach, EBodyPart.Head, EBodyPart.Chest };
        // private static DamageInfo tmpDmg;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ActiveHealthController), nameof(ActiveHealthController.DestroyBodyPart));
        }

        [PatchPrefix]
        private static bool Prefix(ActiveHealthController __instance, EBodyPart bodyPart, EDamageType damageType)
        {
            try
            {
                // Target is not our player - don't do anything
                if (__instance.Player == null || !__instance.Player.IsYourPlayer)
                {
                    return true;
                }

                // If Keep1Health is disabled, don't do anything
                if (!DeminvicibilityPlugin.Keep1Health.Value)
                {
                    return true;
                }

                // If AllowBlacking is disabled, we prevent the original method from running regardless
                if (!DeminvicibilityPlugin.AllowBlacking.Value)
                {
                    return false;  // skip original method
                } 

                // If limb blacking is enabled, but Head & Thorax are protected (and being currently hit), we'll skip the original method here
                if (!DeminvicibilityPlugin.AllowBlackingHeadAndThorax.Value && (bodyPart == EBodyPart.Head || bodyPart == EBodyPart.Chest))
                {
                    return false; // skip original method
                }

                // In all other cases, we let the original method run
                return true; // run original method

                // var healthController = __instance.Player.ActiveHealthController;
                // var currentHealth = healthController.GetBodyPartHealth(bodyPart, false);

                // if (DeminvicibilityPlugin.SecondChanceProtection.Value && !DeminvicibilityPlugin.Keep1Health.Value)
                // {
                //     return false;
                // }
                // else if (DeminvicibilityPlugin.Keep1Health.Value && !DeminvicibilityPlugin.SecondChanceProtection.Value)
                // {
                //     if (DeminvicibilityPlugin.Keep1HealthSelection.Value == "Head And Thorax")
                //     {
                //         if (bodyPart == EBodyPart.Head || bodyPart == EBodyPart.Chest)
                //         {
                //             ConsoleScreen.Log("Player shouldn\'t black out here");
                //             Logger.LogMessage("Player shouldn't black out here");
                //             return false;
                //         }
                //         else
                //         {
                //             ConsoleScreen.Log("Player blacked out here");
                //             Logger.LogMessage("Player blacked out here");
                //             Logger.LogMessage(" ===================================== ");
                //             Logger.LogMessage(" ===================================== ");
                //             Logger.LogMessage(" ===================================== ");
                //             return true;
                //         }
                //     }
                //     else if (DeminvicibilityPlugin.Keep1HealthSelection.Value == "All")
                //     {
                //         return false;
                //     }
                // }

            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }

            return true;
        }

    }
}
