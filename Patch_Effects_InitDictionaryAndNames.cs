using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using Systems.Effects;
using UnityEngine;

namespace tarkin.SEGI.Bep
{
    internal class Patch_Effects_InitDictionaryAndNames : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Effects), nameof(Effects.InitDictionaryAndNames));
        }

        [PatchPostfix]
        static void Postfix(Effects __instance)
        {
            int newLayerForEffects = 1;

            foreach (var effect in __instance.EffectsArray)
            {
                if (effect.BasicParticleSystemMediator != null)
                {
                    effect.BasicParticleSystemMediator.gameObject.layer = newLayerForEffects;
                }

                foreach (var particleSys in effect.Particles)
                {
                    if (particleSys.Particle != null)
                    {
                        particleSys.Particle.gameObject.layer = newLayerForEffects;
                    }
                }
            }
        }
    }
}
