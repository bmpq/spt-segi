using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace tarkin.SEGI.Bep
{
    internal class SEGIAssetManager
    {
        private static SEGIAssets assets;
        private static Dictionary<SEGIPresetQuality, SEGIPreset> presets;

        static void TryLoadBundle()
        {
            if (assets != null)
                return;

            AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(BepInEx.Paths.PluginPath, "tarkin", "bundles", "segi"));

            assets = bundle.LoadAllAssets<SEGIAssets>()[0];

            presets = new Dictionary<SEGIPresetQuality, SEGIPreset>();

            foreach (var preset in bundle.LoadAllAssets<SEGIPreset>())
            {
                if (System.Enum.TryParse<SEGIPresetQuality>(preset.name, ignoreCase: true, out var quality))
                {
                    presets[quality] = preset;
                }
                else
                {
                    Plugin.Logger.LogWarning($"[SEGI] Could not map preset '{preset.name}' to a {nameof(SEGIPresetQuality)} value.");
                }
            }
        }

        public static SEGIAssets GetAssets()
        {
            TryLoadBundle();

            return assets;
        }

        public static SEGIPreset GetPreset(SEGIPresetQuality quality)
        {
            TryLoadBundle();

            if (!presets.TryGetValue(quality, out var preset))
            {
                Debug.LogError($"[SEGI] No preset found for quality '{quality}'.");
                return null;
            }

            return preset;
        }
    }

    enum SEGIPresetQuality
    {
        Low,
        Medium,
        High,
        Ultra
    }
}
