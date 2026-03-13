using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Comfort.Common;
using EFT;
using EFT.Weather;
using SPT.Reflection.Patching;
using System.IO;
using UnityEngine;

using CameraManager = CameraClass;

namespace tarkin.SEGI.Bep
{
    [BepInPlugin("com.tarkin.segi", MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }
        public static new ManualLogSource Logger { get; private set; }

        internal SEGIConfig SegiConfig { get; private set; }
        internal AssetBundleManager AssetBundleManager { get; private set; }

        private SEGIManager segiManager;

        private PatchManager patchManager;

        internal ConfigEntry<KeyboardShortcut> KeybindToggle;

        void Start()
        {
            Instance = this;

            Logger = base.Logger;

            KeybindToggle = Config.Bind("_Keybinds_", "Toggle SEGI renderer", new KeyboardShortcut(KeyCode.PageUp), "");

            SegiConfig = new SEGIConfig();
            SegiConfig.Bind(this.Config);

            patchManager = new PatchManager(this, autoPatch: true);
            patchManager.EnablePatches();

            AssetBundleManager = new AssetBundleManager(Path.Combine(BepInEx.Paths.PluginPath, "SEGI"));

            Patch_GameWorld_OnGameStarted.OnPostfix += TryAddSEGI;
            if (Singleton<GameWorld>.Instantiated)
                TryAddSEGI();
        }

        void TryAddSEGI()
        {
            Camera cam = CameraManager.Instance.Camera;
            segiManager = cam.gameObject.AddComponent<SEGIManager>();
        }

        void OnDestroy()
        {
            Patch_GameWorld_OnGameStarted.OnPostfix -= TryAddSEGI;

            AssetBundleManager.Dispose();
            AssetBundleManager = null;

            if (segiManager != null)
                Component.Destroy(segiManager);

            patchManager.DisablePatches();
            patchManager = null;

            SegiConfig = null;
            KeybindToggle = null;

            Logger = null;
            Instance = null;
        }
    }
}