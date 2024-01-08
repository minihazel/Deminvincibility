using System;
using System.Reflection;
using Aki.Reflection.Patching;
using EFT.HealthSystem;
using HarmonyLib;

namespace Deminvincibility.Patches
{
    internal class ApplyDamage : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ActiveHealthController), nameof(ActiveHealthController.ApplyDamage));
        }

        [PatchPrefix]
        private static bool Prefix(ActiveHealthController __instance, ref float damage, EBodyPart bodyPart, DamageInfo damageInfo)
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

                // Scale damage based on our set damage %
                if (DeminvicibilityPlugin.CustomDamageModeVal.Value != 100)
                {
                    damage *= (float)DeminvicibilityPlugin.CustomDamageModeVal.Value / 100;
                }

                // Remove negative health effects
                if (DeminvicibilityPlugin.MedicineBool.Value)
                {
                    EBodyPart[] bParts =
                    {
                        EBodyPart.Head,
                        EBodyPart.Chest,
                        EBodyPart.Stomach,
                        EBodyPart.LeftArm,
                        EBodyPart.RightArm,
                        EBodyPart.LeftLeg,
                        EBodyPart.RightLeg
                    };

                    foreach (var bPart in bParts)
                    {
                        healthController.RemoveNegativeEffects(bPart);
                    }
                }

                var healthSubtractedByDamage = currentHealth.Current - damage;
                if (DeminvicibilityPlugin.Keep1Health.Value && healthSubtractedByDamage < 1f)
                {
                    // Head and Chest will never be allowed below 1HP if Keep1Health is enabled
                    // Other limbs will also be protected if Allow0HpLimbs is disabled
                    if (bodyPart == EBodyPart.Head || bodyPart == EBodyPart.Chest || !DeminvicibilityPlugin.Allow0HpLimbs.Value)
                    {
                        damage = Math.Max(0, currentHealth.Current - 1f);
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
