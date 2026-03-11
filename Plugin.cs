using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
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

        internal static ConfigEntry<KeyboardShortcut> KeybindToggle;

        void Start()
        {
            Logger = base.Logger;

            segiConfig = new SEGIConfig();
            segiConfig.Bind(this.Config);

            patchManager = new PatchManager(this, autoPatch: true);
            patchManager.EnablePatches();

            KeybindToggle = Config.Bind("_Keybinds_", "Toggle SEGI renderer", new KeyboardShortcut(KeyCode.PageUp), "");

            assetBundleManager = new AssetBundleManager(Path.Combine(BepInEx.Paths.PluginPath, "SEGI"));
            segiManager = new SEGIManager(assetBundleManager.GetSEGIResources());
        }

        void Update()
        {
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
            Logger = null;
        }
    }
}