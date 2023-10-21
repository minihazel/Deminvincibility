using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Aki.Reflection.Patching;
using Deminvincibility;
using EFT;
using EFT.HealthSystem;
using EFT.UI;
using HarmonyLib;

namespace Deminvincibility.Patches
{
    internal class DestroyBodyPartPatch : ModulePatch
    {
        private static readonly EBodyPart[] critBodyParts = { EBodyPart.Stomach, EBodyPart.Head, EBodyPart.Chest };
        private static DamageInfo tmpDmg;
        private static ActiveHealthController healthController;
        private static ValueStruct currentHealth;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ActiveHealthController), "DestroyBodyPart");
        }

        [PatchPrefix]
        private static bool Prefix(ActiveHealthController __instance, EBodyPart bodyPart, EDamageType damageType)
        {
            try
            {
                if (__instance.Player != null && __instance.Player.IsYourPlayer)
                {
                    healthController = __instance.Player.ActiveHealthController;
                    currentHealth = healthController.GetBodyPartHealth(bodyPart, false);

                    return false;
                    /*
                    if (DeminvicibilityPlugin.hpDeathBool.Value && !DeminvicibilityPlugin.Keep1Health.Value)
                    {
                        return false;
                    }
                    else if (DeminvicibilityPlugin.Keep1Health.Value && !DeminvicibilityPlugin.hpDeathBool.Value)
                    {
                        if (DeminvicibilityPlugin.Keep1HealthSelection.Value == "Head And Thorax")
                        {
                            if (bodyPart == EBodyPart.Head || bodyPart == EBodyPart.Chest)
                            {
                                ConsoleScreen.Log("Player shouldn\'t black out here");
                                Logger.LogMessage("Player shouldn't black out here");
                                return false;
                            }
                            else
                            {
                                ConsoleScreen.Log("Player blacked out here");
                                Logger.LogMessage("Player blacked out here");
                                Logger.LogMessage(" ===================================== ");
                                Logger.LogMessage(" ===================================== ");
                                Logger.LogMessage(" ===================================== ");
                                return true;
                            }
                        }
                        else if (DeminvicibilityPlugin.Keep1HealthSelection.Value == "All")
                        {
                            return false;
                        }
                    }
                    */
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
            return true;
        }

    }
}
