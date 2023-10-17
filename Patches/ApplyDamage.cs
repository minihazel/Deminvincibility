using System;
using System.Reflection;
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
                    Logger.LogDebug(damage);
                    ConsoleScreen.Log(" ============================= ");
                    ConsoleScreen.Log(" ============================= ");
                    ConsoleScreen.Log(" ============================= ");
                    ConsoleScreen.Log($"INITIAL DAMAGE: {Convert.ToString(damage)}");
                    ConsoleScreen.Log("");

                    healthController = __instance.Player.ActiveHealthController;
                    currentHealth = healthController.GetBodyPartHealth(bodyPart, false);
                    ConsoleScreen.Log($"CURRENT HEALTH: {Convert.ToString(currentHealth.Current)}");
                    ConsoleScreen.Log("");

                    if (DeminvicibilityPlugin.CustomDamageModeVal.Value != 100)
                    {
                        damage = damage * ((float)DeminvicibilityPlugin.CustomDamageModeVal.Value / 100);
                    }

                    if (DeminvicibilityPlugin.Keep1Health.Value && ((currentHealth.Current - damage) <= 0))
                    {
                        float control = currentHealth.Current - damage;
                        ConsoleScreen.Log(Convert.ToString(control));

                        if (DeminvicibilityPlugin.Keep1HealthSelection.Value == "Head")
                        {
                            if (bodyPart == EBodyPart.Head)
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
                        else if (DeminvicibilityPlugin.Keep1HealthSelection.Value == "Thorax")
                        {
                            if (bodyPart == EBodyPart.Chest)
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
                        else if (DeminvicibilityPlugin.Keep1HealthSelection.Value == "Head And Thorax")
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
                            ConsoleScreen.Log($"KEEP1HEALTH DAMAGE: {Convert.ToString(damage)}");
                            ConsoleScreen.Log("");
                            ConsoleScreen.Log($"BODYPART HIT: {bodyPart.ToString()}");

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
