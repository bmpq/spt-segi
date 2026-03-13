using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Comfort.Common;
using EFT.Weather;
using SPT.Reflection.Patching;
using System.IO;
using UnityEngine;

namespace tarkin.SEGI.Bep
{
    [BepInPlugin("com.tarkin.segi", MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static new ManualLogSource Logger;

        internal SEGIConfig segiConfig;

        private PatchManager patchManager;

        AssetBundleManager assetBundleManager;
        SEGIManager segiManager;

        internal ConfigEntry<KeyboardShortcut> KeybindToggle;

        void Start()
        {
            Logger = base.Logger;

            KeybindToggle = Config.Bind("_Keybinds_", "Toggle SEGI renderer", new KeyboardShortcut(KeyCode.PageUp), "");

            segiConfig = new SEGIConfig();
            segiConfig.Bind(this.Config);

            patchManager = new PatchManager(this, autoPatch: true);
            patchManager.EnablePatches();

            assetBundleManager = new AssetBundleManager(Path.Combine(BepInEx.Paths.PluginPath, "SEGI"));
            segiManager = new SEGIManager(assetBundleManager.GetSEGIResources());
        }

        void Update()
        {
            if (Singleton<TOD_Sky>.Instantiated)
            {
                Color skyColor = Singleton<TOD_Sky>.Instance.SampleAtmosphere(Vector3.zero, false);
                skyColor = ToDController.SaturateColor(skyColor * 1.3f, 0.3f);
                
                segiManager.SetReflectionSkyColor(skyColor);
            }
            else
            { 
                segiManager.SetReflectionSkyColor(Color.black);
            }

            segiManager.ApplyConfig(segiConfig);

            if (KeybindToggle.Value.IsDown())
            {
                segiManager.Toggle();
            }
        }

        void OnDestroy()
        {
            assetBundleManager.Dispose();
            assetBundleManager = null;

            segiManager.Dispose();
            segiManager = null;

            patchManager.DisablePatches();
            patchManager = null;

            segiConfig = null;
            KeybindToggle = null;

            Logger = null;
        }
    }
}