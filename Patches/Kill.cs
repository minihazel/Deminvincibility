using System;
using System.Reflection;
using System.Threading.Tasks;
using SPT.Reflection.Patching;
using Deminvincibility.Model;
using EFT;
using EFT.HealthSystem;
using EFT.UI;
using HarmonyLib;

namespace Deminvincibility.Patches
{
    internal class Kill : ModulePatch
    {
        public static bool HasSecondChance = true;
        public static bool IsDebouncing = false;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ActiveHealthController), nameof(ActiveHealthController.Kill));
        }

        [PatchPrefix]
        public static bool Prefix(ActiveHealthController __instance, EDamageType damageType, Player ___Player)
        {
            try
            {
                // Target is not our player - don't do anything
                if (___Player == null || !___Player.IsYourPlayer && ___Player.IsAI)
                {
                    return true;
                }

                // If Keep1Health is enabled, prevent the original method from running and don't bother running the second chance protection code
                if (DeminvicibilityPlugin.Keep1Health.Value)
                {
                    return false;
                }

                // Second chance protection is currently being debounced, so we just skip the original method execution this time
                if (IsDebouncing)
                {
                    return false;
                }

                var hc = ___Player.ActiveHealthController;
                var headHealth = hc.GetBodyPartHealth(EBodyPart.Head, false);
                var chestHealth = hc.GetBodyPartHealth(EBodyPart.Chest, false);
                // var stomachHealth = hc.GetBodyPartHealth(EBodyPart.Stomach, false);
                // var leftArmHealth = hc.GetBodyPartHealth(EBodyPart.LeftArm, false);
                // var rightArmHealth = hc.GetBodyPartHealth(EBodyPart.RightArm, false);
                // var leftLegHealth = hc.GetBodyPartHealth(EBodyPart.LeftLeg, false);
                // var rightLegHealth = hc.GetBodyPartHealth(EBodyPart.RightLeg, false);
                //
                // float totalHealth =
                //     headHealth.Current +
                //     chestHealth.Current +
                //     stomachHealth.Current +
                //     leftArmHealth.Current +
                //     rightArmHealth.Current +
                //     leftLegHealth.Current +
                //     rightLegHealth.Current;

                if (DeminvicibilityPlugin.SecondChanceProtection.Value)
                {
                    if (HasSecondChance)
                    {
                        if (DeminvicibilityPlugin.SecondChanceEffectRemoval.Value)
                        {
                            // Iterate over every body part (including Common) and remove all negative effects
                            foreach (var bodyPart in (EBodyPart[])Enum.GetValues(typeof(EBodyPart)))
                            {
                                hc.RemoveNegativeEffects(bodyPart);
                            }
                        }

                        if (DeminvicibilityPlugin.SecondChanceHealthRestoreAmount.Value > SecondChanceRestoreEnum.None)
                        {
                            var targetHeadHealth = 0f;
                            var targetChestHealth = 0f;

                            switch (DeminvicibilityPlugin.SecondChanceHealthRestoreAmount.Value)
                            {
                                case SecondChanceRestoreEnum.OneHealth:
                                    targetHeadHealth = 1;
                                    targetChestHealth = 1;
                                    break;
                                case SecondChanceRestoreEnum.Half:
                                    targetHeadHealth = headHealth.Maximum / 2;
                                    targetChestHealth = chestHealth.Maximum / 2;
                                    break;
                                case SecondChanceRestoreEnum.Full:
                                    targetHeadHealth = headHealth.Maximum;
                                    targetChestHealth = chestHealth.Maximum;
                                    break;
                                default:
                                    Logger.LogError($"Unhandled SecondChanceRestoreEnum value: {DeminvicibilityPlugin.SecondChanceHealthRestoreAmount.Value}");
                                    break;
                            }

                            // Easiest way to set a limb to a specific HP seems to be healing it to full and damaging it back down
                            // Get the protected FullRestoreBodyPart(EBodyPart) method
                            var bodyPartRestoreMethod = AccessTools.Method(typeof(ActiveHealthController), "FullRestoreBodyPart");

                            if (headHealth.Current < targetHeadHealth)
                            {
                                // Invoke the FullRestoreBodyPart method. Should fully heal the limb
                                bodyPartRestoreMethod.Invoke(hc, new object[] { EBodyPart.Head });
                                // Damage the limb with an undefined damage type down to the desired HP
                                hc.ApplyDamage(EBodyPart.Head, Math.Abs(targetHeadHealth - headHealth.Maximum), new DamageInfoStruct { DamageType = EDamageType.Undefined });
                            }

                            if (chestHealth.Current < targetChestHealth)
                            {
                                bodyPartRestoreMethod.Invoke(hc, new object[] { EBodyPart.Chest });
                                hc.ApplyDamage(EBodyPart.Chest, Math.Abs(targetHeadHealth - headHealth.Maximum), new DamageInfoStruct { DamageType = EDamageType.Undefined });
                            }
                        }

                        // Sometimes damage gets applied multiple times and it will feel like protection didn't work - this debouncing should help negate that slightly
                        // We create an async task that will wait for a small amount of time before executing the code inside
                        IsDebouncing = true;
                        _ = Task.Run(async () =>
                        {
                            await Task.Delay(350); // 0.35 seconds of GodMode before protection ends
                            
                            ConsoleScreen.Log("SECOND CHANCE PROTECTION SAVED YOU, GOOD LUCK!");
                            Logger.LogMessage("SECOND CHANCE PROTECTION SAVED YOU, GOOD LUCK!");
                            HasSecondChance = false;
                            IsDebouncing = false;

                            /*
                                TODO: Right now, you will get healed right away and then 0.35 seconds later protection will end.
                                This means that if you keep getting damaged over those 0.35 seconds, any health gained or effects removed might be negated.
                                The issue could be solved by delaying all the code above from running until after the debounce,
                                but that would require executing all that code inside an async task, which could be problematic.
                                Main concern: horrible exception handling - nothing will be thrown by default and extra code would be required to catch exceptions from the async context.
                                Secondary concern: using instances to call methods and check data in an async context might have various side effects which could result in mysterious and hard-to-reproduce issues.
                             */
                        });

                        return false; // Skip original method
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
