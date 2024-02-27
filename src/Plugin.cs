using BepInEx;
using Configgy;
using HarmonyLib;

namespace SlabManBuff
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    [BepInDependency("Hydraxous.ULTRAKILL.Configgy")]
    public class Plugin : BaseUnityPlugin
    {
        public const string pluginGuid = "yuu.slabman_buff";
        public const string pluginName = "Alt Marksman Buff";
        public const string pluginVersion = "1.0.0";
        
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {pluginName} is loaded!");

            //Load assets
            AssetManager.LoadAssetBundle();

            //Configgy
            ConfigBuilder config = new ConfigBuilder(pluginGuid, pluginName);
            config.BuildAll();

            //Patch with harmony
            Harmony harmony = new Harmony(pluginGuid);
            harmony.PatchAll();
        }
    }
}
