using BepInEx;
using BepInEx.Logging;

namespace tarkin.SEGI.Bep
{
    [BepInPlugin("com.tarkin.segi", MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static new ManualLogSource Logger;

        void Start()
        {
            Logger = base.Logger;

            new Patch_GameWorld_OnGameStarted().Enable();

            SEGIConfig.Bind(Config);
        }
    }
}