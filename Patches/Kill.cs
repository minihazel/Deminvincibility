using System;
using System.Collections.Generic;
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
    internal class Kill : ModulePatch
    {
        public static bool hasSecondChance = true;
        private static ActiveHealthController hc;
        private static ValueStruct currentHeadHealth;
        private static ValueStruct currentChestHealth;
        private static ValueStruct currentStomachHealth;
        private static ValueStruct currentLeftArmHealth;
        private static ValueStruct currentRightArmHealth;
        private static ValueStruct currentLeftLegHealth;
        private static ValueStruct currentRightLegHealth;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ActiveHealthController), "Kill");
        }

        [PatchPrefix]
        public static bool Prefix(ActiveHealthController __instance, EDamageType damageType)
        {
            try
            {
                if (__instance.Player != null && __instance.Player.IsYourPlayer)
                {
                    hc = __instance.Player.ActiveHealthController;
                    currentHeadHealth = hc.GetBodyPartHealth(EBodyPart.Head, false);
                    currentChestHealth = hc.GetBodyPartHealth(EBodyPart.Chest, false);
                    currentStomachHealth = hc.GetBodyPartHealth(EBodyPart.Stomach, false);
                    currentLeftArmHealth = hc.GetBodyPartHealth(EBodyPart.LeftArm, false);
                    currentRightArmHealth = hc.GetBodyPartHealth(EBodyPart.RightArm, false);
                    currentLeftLegHealth = hc.GetBodyPartHealth(EBodyPart.LeftLeg, false);
                    currentRightLegHealth = hc.GetBodyPartHealth(EBodyPart.RightLeg, false);

                    float totalHealth =
                        currentHeadHealth.Current +
                        currentChestHealth.Current +
                        currentStomachHealth.Current +
                        currentLeftArmHealth.Current +
                        currentRightArmHealth.Current +
                        currentLeftLegHealth.Current +
                        currentRightLegHealth.Current;

                    if (DeminvicibilityPlugin.Keep1Health.Value)
                    {
                        return false;
                    }
                    else if (DeminvicibilityPlugin.hpDeathBool.Value && !DeminvicibilityPlugin.Keep1Health.Value)
                    {
                        if (hasSecondChance)
                        {
                            if (damageType == EDamageType.Bullet ||
                                damageType == EDamageType.Explosion)
                            {
                                ConsoleScreen.Log("SECOND CHANCE PROTECTION SAVED YOU, GOOD LUCK!");
                                Logger.LogMessage("SECOND CHANCE PROTECTION SAVED YOU, GOOD LUCK!");
                                hasSecondChance = false;
                            }

                            return false;
                        }
                        else
                        {
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
