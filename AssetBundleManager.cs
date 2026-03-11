using System;
using System.IO;
using UnityEngine;

namespace tarkin.SEGI.Bep
{
    internal class AssetBundleManager : IDisposable
    {
        private readonly string pathBundleDir;

        private AssetBundle bundle;
        private SEGIAssets assets;

        internal AssetBundleManager(string bundleDir)
        {
            pathBundleDir = bundleDir;

            LoadBundle();
        }

        void LoadBundle()
        {
            string bundleName = "segiassets";

            string fullBundlePath = Path.Combine(pathBundleDir, bundleName);
            if (!File.Exists(fullBundlePath))
            {
                Plugin.Logger.LogError($"'{fullBundlePath}' not found!");
                return;
            }

            bundle = AssetBundle.LoadFromFile(fullBundlePath);
            if (bundle == null)
            {
                Plugin.Logger.LogError($"Failed to load '{fullBundlePath}'!");
                return;
            }

            assets = bundle.LoadAllAssets<SEGIAssets>()[0];
        }

        public SEGIAssets GetSEGIResources()
        {
            return assets;
        }

        public void Dispose()
        {
            if (bundle != null)
            {
                bundle.Unload(true);
            }
        }
    }
}