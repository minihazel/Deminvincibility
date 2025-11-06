using SPT.Reflection.Patching;
using EFT.HealthSystem;
using HarmonyLib;
using System;
using System.Reflection;
using EFT;

namespace Deminvincibility.Patches
{
    internal class DoFracture : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ActiveHealthController), nameof(ActiveHealthController.DoFracture));
        }

        [PatchPrefix]
        private static bool Prefix(ActiveHealthController __instance, EBodyPart bodyPart, Player ___Player)
        {
            try
            {
                // Target is not our player - don't do anything
                if (___Player == null || !___Player.IsYourPlayer && ___Player.IsAI)
                {
                    return true;
                }

                // If Keep1Health and MedicineBool are enabled, then the method to apply a Fracture effect is skipped
                if (DeminvicibilityPlugin.MedicineBool.Value && DeminvicibilityPlugin.Keep1Health.Value)
                {
                    return false;
                }

                // If Keep1Health is enabled, but MedicineBool is disabled, then we only protect the head and chest
                if (!DeminvicibilityPlugin.MedicineBool.Value)
                {
                    // If it's not Head or Chest, then run the original method. Otherwise, skip it
                    return bodyPart != EBodyPart.Head && bodyPart != EBodyPart.Chest;
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
