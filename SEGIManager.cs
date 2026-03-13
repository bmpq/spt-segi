using Comfort.Common;
using EFT;
using EFT.Weather;
using System;
using UnityEngine;

namespace tarkin.SEGI.Bep
{
    internal class SEGIManager : MonoBehaviour
    {
        private SEGIRenderer segi;

        void Start()
        {
            Light mainLight = GetMainLight();
            if (mainLight == null)
            {
                Destroy(this);
                return;
            }

            PrepareWorld();

            if (gameObject.TryGetComponent<SEGIRenderer>(out var oldInstance))
                Component.Destroy(oldInstance);

            segi = gameObject.AddComponent<SEGIRenderer>();

            segi.assetResources = Plugin.Instance.AssetBundleManager.GetSEGIResources();

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

        void Update()
        {
            if (Singleton<TOD_Sky>.Instantiated)
            {
                Color skyColor = Singleton<TOD_Sky>.Instance.SampleAtmosphere(Vector3.zero, false);
                skyColor = ToDController.SaturateColor(skyColor * 1.3f, 0.3f);

                segi.reflectionSkyColor = skyColor;
            }
            else
            {
                segi.reflectionSkyColor = Color.black;
            }

            Plugin.Instance.SegiConfig.Apply(segi);

            if (Plugin.Instance.KeybindToggle.Value.IsDown())
            {
                segi.enabled = !segi.enabled;
            }
        }

        void OnDestroy()
        {
            if (segi != null)
                Component.Destroy(segi);
        }
    }
}
