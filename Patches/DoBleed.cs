using SPT.Reflection.Patching;
using EFT.HealthSystem;
using HarmonyLib;
using System;
using System.Reflection;
using EFT;

namespace Deminvincibility.Patches
{
    internal class DoBleed : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ActiveHealthController), "DoBleed");
        }

        [PatchPrefix]
        private static bool Prefix(ActiveHealthController __instance, EBodyPart bodyPart, Player ___Player)
        {
            try
            {
                // Target is not our player - don't do anything
                if (___Player == null || !___Player.IsYourPlayer)
                {
                    return true;
                }
                
                if (DeminvicibilityPlugin.Keep1Health.Value && DeminvicibilityPlugin.MedicineBool.Value)
                {
                    Logger.LogMessage("DoBleed: Keep1Health & MedicineBool RETURN FALSE");
                    return false;
                }
                else
                {
                    Logger.LogMessage("DoBleed: Keep1Health & MedicineBool RETURN TRUE");
                    return true;
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
