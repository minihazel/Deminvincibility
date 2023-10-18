using System;
using System.Reflection;
using Aki.Reflection.Patching;
using Deminvincibility;
using EFT;
using EFT.HealthSystem;
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

                    if (DeminvicibilityPlugin.Keep1Health.Value)
                    {
                        if (DeminvicibilityPlugin.Keep1HealthSelection.Value == "Head")
                        {
                            if (bodyPart == EBodyPart.Head)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else if (DeminvicibilityPlugin.Keep1HealthSelection.Value == "Thorax")
                        {
                            if (bodyPart == EBodyPart.Chest)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else if (DeminvicibilityPlugin.Keep1HealthSelection.Value == "Head And Thorax")
                        {
                            if (bodyPart == EBodyPart.Head || bodyPart == EBodyPart.Chest)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else if (DeminvicibilityPlugin.Keep1HealthSelection.Value == "All")
                        {
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
