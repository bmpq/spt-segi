using Comfort.Common;
using EFT;
using EFT.Weather;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using UnityEngine;

namespace tarkin.SEGI.Bep
{
    internal class Patch_GameWorld_OnGameStarted : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GameWorld), nameof(GameWorld.OnGameStarted));
        }

        [PatchPostfix]
        private static void PatchPostfix(GameWorld __instance)
        {
            Light mainLight = GetMainLight();
            if (mainLight == null)
                return;

            PrepareWorld();

            SEGIRenderer segi = CameraClass.Instance.Camera.gameObject.AddComponent<SEGIRenderer>();

            segi.assetResources = SEGIAssetManager.GetAssets();
            segi.giCullingMask = 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Terrain");
            segi.sun = Singleton<TOD_Sky>.Instance.Components.LightSource;
            segi.skyIntensity = 0;

            segi.enabled = true;

            segi.gameObject.AddComponent<SEGIEFT>();
        }

        static Light GetMainLight()
        {
            if (Singleton<TOD_Sky>.Instantiated)
                return Singleton<TOD_Sky>.Instance.Components.LightSource;

            if (Singleton<TODSkySimple>.Instantiated)
                return Singleton<TODSkySimple>.Instance.LightSource;

            Plugin.Logger.LogError("Could not find main light source, disabling SEGI.");

            return null;
        }

        static void PrepareWorld()
        {

        }
    }
}
