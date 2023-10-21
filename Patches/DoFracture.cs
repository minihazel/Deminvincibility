using Aki.Reflection.Patching;
using EFT.HealthSystem;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Deminvincibility.Patches
{
    internal class DoFracture : ModulePatch
    {
        private static ActiveHealthController healthController;
        private static ValueStruct currentHealth;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ActiveHealthController), "DoFracture");
        }

        [PatchPrefix]
        private static bool Prefix(ActiveHealthController __instance, EBodyPart bodyPart)
        {
            try
            {
                if (__instance.Player != null && __instance.Player.IsYourPlayer)
                {
                    if (DeminvicibilityPlugin.medicineBool.Value && DeminvicibilityPlugin.Keep1Health.Value)
                        return false;
                    else if (!DeminvicibilityPlugin.medicineBool.Value)
                    {
                        if (bodyPart == EBodyPart.Head || bodyPart == EBodyPart.Chest)
                            return false;
                        else
                            return true;
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
