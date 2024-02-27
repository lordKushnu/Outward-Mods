using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using HostInventoryStash.Services;

// RENAME 'OutwardModTemplate' TO SOMETHING ELSE
namespace HostInventoryStash
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class HostInventoryStash : BaseUnityPlugin
    {
        // Choose a GUID for your project. Change "myname" and "mymod".
        public const string GUID = "theGrungler.HostInventoryStash";
        // Choose a NAME for your project, generally the same as your Assembly Name.
        public const string NAME = "HostInventoryStash";
        // Increment the VERSION when you release a new version of your mod.
        public const string VERSION = "1.0.0";

        public const string DISPLAY_NAME = "Host Inventory Stash";

        // For accessing your BepInEx Logger from outside of this class (eg Plugin.Log.LogMessage("");)
        internal static ManualLogSource Log;

        private static Harmony _harmony;

        internal static InventoryStashService inventoryStashService;
        // If you need settings, define them like so:
        public static ConfigEntry<bool> ShowStash;
        public static ConfigEntry<bool> ShowStashOutsideOfTown;
        // Awake is called when your plugin is created. Use this to set up your mod.
        internal void Awake()
        {
            Log = this.Logger;
            Log.LogMessage($"Starting {NAME} {VERSION}!");
            InitializeConfig();
            // Any config settings you define should be set up like this:
            // Harmony is for patching methods. If you're not patching anything, you can comment-out or delete this line.
            _harmony = new Harmony(GUID);
            _harmony.PatchAll();
            inventoryStashService = new InventoryStashService();
        }

        internal void OnDestroy()
        {
            inventoryStashService.deleteStash();
            _harmony?.UnpatchSelf();
            Log.LogMessage($"Destoryed");
            Config.Clear();
        }

        private void InitializeConfig()
        {
            ShowStash = Config.Bind(DISPLAY_NAME, "Show Host inventory stash", false, "Allows host's stash to be shown while in inventory");
            ShowStashOutsideOfTown = Config.Bind(DISPLAY_NAME, "Show outside of town", false, "Allows Host's stash to be accessable while out of town");
        }

        // Update is called once per frame. Use this only if needed.
        // You also have all other MonoBehaviour methods available (OnGUI, etc)
        internal void Update()
        {
            //Log.LogMessage("This is an update");
        }

    }
}
