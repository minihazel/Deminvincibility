using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Aki.Reflection.Patching;
using Deminvincibility;
using EFT;
using EFT.HealthSystem;
using EFT.UI;
using HarmonyLib;
using UnityEngine;

namespace Deminvincibility.Patches
{
    internal class ChangeHealth : ModulePatch
    {
        private static ActiveHealthController healthController;
        private static ValueStruct currentHealth;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ActiveHealthController), "ChangeHealth");
        }

        [PatchPrefix]
        private static bool Prefix(ActiveHealthController __instance, EBodyPart bodyPart, ref float value, DamageInfo damageInfo)
        {
            try
            {
                if (__instance.Player != null && __instance.Player.IsYourPlayer)
                {
                    healthController = __instance.Player.ActiveHealthController;
                    currentHealth = healthController.GetBodyPartHealth(bodyPart, false);

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
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
            return true;
        }
    }
}
