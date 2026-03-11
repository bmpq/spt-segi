using Comfort.Common;
using EFT;
using EFT.Weather;
using System;
using UnityEngine;

namespace tarkin.SEGI.Bep
{
    internal class SEGIManager : IDisposable
    {
        readonly SEGIAssets resources;

        private SEGIRenderer segi;

        internal SEGIManager(SEGIAssets segiResources)
        {
            resources = segiResources;

            Patch_GameWorld_OnGameStarted.OnPostfix += Init;

            if (Singleton<GameWorld>.Instantiated)
                Init(Singleton<GameWorld>.Instance);
        }

        void Init(GameWorld gameWorld)
        {
            Light mainLight = GetMainLight();
            if (mainLight == null)
                return;

            PrepareWorld();

            Camera targetCamera = CameraClass.Instance.Camera;

            if (targetCamera.gameObject.TryGetComponent<SEGIRenderer>(out var oldInstance))
                Component.Destroy(oldInstance);

            segi = targetCamera.gameObject.AddComponent<SEGIRenderer>();

            segi.assetResources = resources;

            segi.sun = mainLight;

            segi.giCullingMask =
                1 << LayerMask.NameToLayer("Default") |
                1 << LayerMask.NameToLayer("Terrain") |
                1 << LayerMask.NameToLayer("Player") |
                1 << LayerMask.NameToLayer("CullingMask");

            segi.skyIntensity = 0;

            segi.enabled = true;
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
            int layerTransparent = LayerMask.NameToLayer("TransparentCollider");

            foreach (var item in GameObject.FindObjectsOfType<LODGroup>())
            {
                foreach (var rend in item.GetComponentsInChildren<MeshRenderer>())
                {
                    foreach (var mat in rend.sharedMaterials)
                    {
                        if (mat ==  null) continue;
                        if (mat.name.Contains("glass"))
                        {
                            rend.gameObject.layer = layerTransparent;
                            break;
                        }
                    }
                }
            }
        }

        public void Toggle()
        {
            if (segi != null)
                segi.enabled = !segi.enabled;
        }

        public void ApplyConfig(SEGIConfig segiConfig)
        {
            if (segi != null)
                segiConfig.Apply(segi);
        }

        public void Dispose()
        {
            Patch_GameWorld_OnGameStarted.OnPostfix -= Init;
        }
    }
}
