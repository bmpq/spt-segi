using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using UnityEngine;

using CameraManager = CameraClass;

namespace tarkin.SEGI.Bep
{
    internal class Patch_CameraManager_SetSSAO : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(CameraManager), nameof(CameraManager.SetSSAO));
        }

        [PatchPrefix]
        static bool Prefix(CameraManager __instance)
        {
            return true;
            __instance.Hbao_0.enabled = false;
            __instance.AmbientOcclusion_0.enabled = false;

            return false;
        }
    }
}
