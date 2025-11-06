using System;
using System.Reflection;
using SPT.Reflection.Patching;
using EFT.HealthSystem;
using EFT.UI;
using HarmonyLib;
using EFT;

namespace Deminvincibility.Patches
{
    internal class ChangeHealth : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ActiveHealthController), nameof(ActiveHealthController.ChangeHealth));
        }

        [PatchPrefix]
        private static bool Prefix(ActiveHealthController __instance, Player ___Player, EBodyPart bodyPart, ref float value, DamageInfoStruct damageInfo)
        {
            try
            {
                // Target is not our player - don't do anything
                if (___Player == null || !___Player.IsYourPlayer && ___Player.IsAI)
                {
                    return true;
                }

                var healthController = ___Player.ActiveHealthController;
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
