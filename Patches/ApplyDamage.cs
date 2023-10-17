using System;
using System.Reflection;
using Aki.Reflection.Patching;
using Deminvincibility;
using EFT;
using EFT.HealthSystem;
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

                    if (DeminvicibilityPlugin.CustomDamageModeVal.Value != 100)
                    {
                        damage = damage * ((float)DeminvicibilityPlugin.CustomDamageModeVal.Value / 100);
                    }

                    if (DeminvicibilityPlugin.Keep1Health.Value && ((currentHealth.Current - damage) <= 0))
                    {
                        if (DeminvicibilityPlugin.Keep1HealthSelection.Value == "Head And Thorax")
                        {
                            if (bodyPart == EBodyPart.Head || bodyPart == EBodyPart.Chest)
                            {
                                damage = currentHealth.Current - 2f;
                                currentHealth.Current = 2f;
                                return false;
                            }
                            else
                            {
                                if (currentHealth.AtMinimum)
                                {
                                    Logger.LogDebug("Destroyed body part: " + bodyPart.ToString());
                                    healthController.DestroyBodyPart(bodyPart, EDamageType.Bullet);
                                    return false;
                                }
                            }
                        }
                        else if (DeminvicibilityPlugin.Keep1HealthSelection.Value == "All")
                        {
                            damage = currentHealth.Current - 2f;
                            currentHealth.Current = 2f;

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
