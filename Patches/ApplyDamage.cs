using System;
using System.Globalization;
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

        private static ValueStruct headHealth;
        private static ValueStruct chestHealth;
        private static ValueStruct stomachHealth;
        private static ValueStruct rArmHealth;
        private static ValueStruct lArmHealth;
        private static ValueStruct rLegHealth;
        private static ValueStruct lLegHealth;

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
                    string convertedHP = DeminvicibilityPlugin.hpDeath.Value.Replace(" HP", "");

                    healthController = __instance.Player.ActiveHealthController;
                    currentHealth = healthController.GetBodyPartHealth(bodyPart, false);
                    float healthSubtractedByDamage = currentHealth.Current - damage;

                    headHealth = healthController.GetBodyPartHealth(EBodyPart.Head, false);
                    chestHealth = healthController.GetBodyPartHealth(EBodyPart.Chest, false);
                    stomachHealth = healthController.GetBodyPartHealth(EBodyPart.Stomach, false);
                    rArmHealth = healthController.GetBodyPartHealth(EBodyPart.RightArm, false);
                    lArmHealth = healthController.GetBodyPartHealth(EBodyPart.LeftArm, false);
                    rLegHealth = healthController.GetBodyPartHealth(EBodyPart.RightLeg, false);
                    lLegHealth = healthController.GetBodyPartHealth(EBodyPart.LeftLeg, false);

                    float totalHealth =
                        headHealth.Current +
                        chestHealth.Current +
                        stomachHealth.Current +
                        rArmHealth.Current +
                        lArmHealth.Current +
                        rLegHealth.Current +
                        lLegHealth.Current;

                    if (DeminvicibilityPlugin.CustomDamageModeVal.Value != 100)
                    {
                        damage = damage * ((float)DeminvicibilityPlugin.CustomDamageModeVal.Value / 100);
                    }

                    if (DeminvicibilityPlugin.hpDeathBool.Value)
                    {
                        int damageInt = (int)damage;
                        int intHP = int.Parse(convertedHP);
                        int totalHealthInt = (int)totalHealth;
                        float floatHP = float.Parse(convertedHP, CultureInfo.InvariantCulture.NumberFormat);

                        if (damageInt > totalHealthInt)
                        {
                            if (totalHealthInt > intHP)
                            {
                                if (damageInfo.DamageType == EDamageType.Bullet)
                                {
                                    Logger.LogMessage("Killshot on Player detected, halting damage");
                                    damage = currentHealth.Current - 2f;
                                    currentHealth.Current = 2f;
                                    if (damage < 0f)
                                        return false;
                                }
                            }
                            else
                            {
                                Logger.LogMessage("Pre-set HP exceeds Player HP, disabling killshot protection");
                                return true;
                            }
                        }
                    }
                    else if (DeminvicibilityPlugin.Keep1Health.Value && !DeminvicibilityPlugin.hpDeathBool.Value && ((healthSubtractedByDamage) <= 0))
                    {
                        
                        if (DeminvicibilityPlugin.Keep1HealthSelection.Value == "Head")
                        {
                            if (bodyPart == EBodyPart.Head)
                            {
                                damage = currentHealth.Current - 2f;
                                currentHealth.Current = 2f;
                                if (damage < 0f)
                                {
                                    return false;
                                }
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
                        else if (DeminvicibilityPlugin.Keep1HealthSelection.Value == "Thorax")
                        {
                            if (bodyPart == EBodyPart.Chest)
                            {
                                damage = currentHealth.Current - 2f;
                                currentHealth.Current = 2f;
                                if (damage < 0f)
                                {
                                    return false;
                                }
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
                        else if (DeminvicibilityPlugin.Keep1HealthSelection.Value == "Head And Thorax")
                        {
                            if (bodyPart == EBodyPart.Head || bodyPart == EBodyPart.Chest)
                            {
                                damage = currentHealth.Current - 2f;
                                currentHealth.Current = 2f;
                                if (damage < 0f)
                                {
                                    return false;
                                }
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
                            if (headHealth.Current == 1f &&
                                chestHealth.Current == 1f &&
                                stomachHealth.Current == 1f &&
                                rArmHealth.Current == 1f &&
                                lArmHealth.Current == 1f &&
                                rArmHealth.Current == 1f &&
                                lLegHealth.Current == 1f)
                            {
                                return false;
                            }

                            damage = currentHealth.Current - 2f;
                            currentHealth.Current = 2f;

                            if (damage < 0f)
                            {
                                return false;
                            }
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
