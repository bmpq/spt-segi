using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Reflection;

namespace tarkin.SEGI.Bep
{
    internal class Patch_GameWorld_OnGameStarted : ModulePatch
    {
        public static event Action OnPostfix;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GameWorld), nameof(GameWorld.OnGameStarted));
        }

        [PatchPostfix]
        private static void PatchPostfix(GameWorld __instance)
        {
            OnPostfix?.Invoke();
        }
    }
}
