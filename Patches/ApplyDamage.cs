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

namespace Deminvincibility.Patches
{
    internal class ApplyDamage : ModulePatch
    {
        private static ActiveHealthController healthController;
        private static ValueStruct currentHealth;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ActiveHealthController), "ApplyDamage");
        }

        [PatchPrefix]
        private static bool Prefix(ActiveHealthController __instance, ref float damage, EBodyPart bodyPart, DamageInfo damageInfo)
        {
            try
            {
                if (__instance.Player != null && __instance.Player.IsYourPlayer)
                {
                    healthController = __instance.Player.ActiveHealthController;
                    currentHealth = healthController.GetBodyPartHealth(bodyPart, false);
                    float healthSubtractedByDamage = currentHealth.Current - damage;

                    if (DeminvicibilityPlugin.CustomDamageModeVal.Value != 100)
                    {
                        damage = damage * ((float)DeminvicibilityPlugin.CustomDamageModeVal.Value / 100);
                    }

                    if (DeminvicibilityPlugin.Keep1Health.Value && !DeminvicibilityPlugin.hpDeathBool.Value && ((healthSubtractedByDamage) <= 0))
                    {
                        if (damage > currentHealth.Current)
                        {
                            if (DeminvicibilityPlugin.Keep1HealthSelection.Value == "All")
                            {
                                damage = currentHealth.Current - 1f;
                            }
                            else if (DeminvicibilityPlugin.Keep1HealthSelection.Value == "Head & Thorax")
                            {
                                if (bodyPart == EBodyPart.Head || bodyPart == EBodyPart.Chest)
                                {
                                    damage = currentHealth.Current - 1f;
                                }
                            }

                            if (DeminvicibilityPlugin.medicineBool.Value)
                            {
                                healthController.RemoveNegativeEffects(bodyPart);
                            }

                            return true;
                        }

                        /*
                        if (currentHealth.Current < 2f)
                        {
                           damage = currentHealth.Current - 2f;
                           healthController.ChangeHealth(bodyPart, 2f, damageInfo);

                           return false;
                        }
                        */
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
