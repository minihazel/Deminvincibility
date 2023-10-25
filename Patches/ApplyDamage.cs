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

                    if (DeminvicibilityPlugin.medicineBool.Value)
                    {
                        EBodyPart[] bPs = {
                            EBodyPart.Head,
                            EBodyPart.Chest,
                            EBodyPart.Stomach,
                            EBodyPart.LeftArm,
                            EBodyPart.RightArm,
                            EBodyPart.LeftLeg,
                            EBodyPart.RightLeg
                        };

                        for (int i = 0; i < bPs.Length; i++)
                        {
                            healthController.RemoveNegativeEffects(bPs[i]);
                        }
                    }

                    if (DeminvicibilityPlugin.CustomDamageModeVal.Value != 100)
                    {
                        damage = damage * ((float)DeminvicibilityPlugin.CustomDamageModeVal.Value / 100);
                    }

                    if (DeminvicibilityPlugin.Keep1Health.Value && !DeminvicibilityPlugin.hpDeathBool.Value && ((healthSubtractedByDamage) <= 0))
                    {
                        if (damage > currentHealth.Current)
                        {
                            if (!DeminvicibilityPlugin.allowBlackedLimbs.Value)
                            {
                                damage = currentHealth.Current - 1f;
                            }
                            else
                            {
                                if (bodyPart == EBodyPart.Head || bodyPart == EBodyPart.Chest)
                                {
                                    damage = currentHealth.Current - 1f;
                                }
                                else
                                {
                                    if (DeminvicibilityPlugin.allowBlacking.Value && currentHealth.Current <= 0f)
                                    {
                                        healthController.DestroyBodyPart(bodyPart, damageInfo.DamageType);
                                    }
                                }
                            }

                            return true;
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
