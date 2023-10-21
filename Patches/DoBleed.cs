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
    internal class DoBleed : ModulePatch
    {
        private static ActiveHealthController healthController;
        private static ValueStruct currentHealth;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ActiveHealthController), "DoBleed");
        }

        [PatchPrefix]
        private static bool Prefix(ActiveHealthController __instance, EBodyPart bodyPart)
        {
            try
            {
                if (__instance.Player != null && __instance.Player.IsYourPlayer)
                {
                    if (DeminvicibilityPlugin.Keep1Health.Value && DeminvicibilityPlugin.medicineBool.Value)
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
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
            return true;
        }
    }
}
