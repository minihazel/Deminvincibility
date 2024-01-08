using System;
using System.Reflection;
using Aki.Reflection.Patching;
using EFT.HealthSystem;
using EFT.UI;
using HarmonyLib;

namespace Deminvincibility.Patches
{
    internal class ChangeHealth : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ActiveHealthController), nameof(ActiveHealthController.ChangeHealth));
        }

        [PatchPrefix]
        private static bool Prefix(ActiveHealthController __instance, EBodyPart bodyPart, ref float value, DamageInfo damageInfo)
        {
            try
            {
                // Target is not our player - don't do anything
                if (__instance.Player == null || !__instance.Player.IsYourPlayer)
                {
                    return true;
                }

                var healthController = __instance.Player.ActiveHealthController;
                var currentHealth = healthController.GetBodyPartHealth(bodyPart, false);

                ConsoleScreen.Log("CHANGEHEALTH");
                Logger.LogMessage("CHANGEHEALTH");

                if (DeminvicibilityPlugin.Keep1Health.Value)
                {
                    if (currentHealth.Current == 1f)
                    {
                        ConsoleScreen.Log("SET HEALTH");
                        Logger.LogMessage("SET HEALTH");

                        value = currentHealth.Current + 1f;
                        currentHealth.Current = 1f;
                        return false;
                    }
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
