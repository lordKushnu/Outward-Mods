using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using RecipeBrowser.Services;
using System;

// RENAME 'OutwardModTemplate' TO SOMETHING ELSE
namespace RecipeBrowser
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class RecipeBrowser : BaseUnityPlugin
    {
        // Choose a GUID for your project. Change "myname" and "mymod".
        public const string GUID = "theGrungler.RecipeBrowser";
        // Choose a NAME for your project, generally the same as your Assembly Name.
        public const string NAME = "RecipeBrowser";
        // Increment the VERSION when you release a new version of your mod.
        public const string VERSION = "1.0.0";

        public const string DISPLAY_NAME = "Recipe Browser";

        // For accessing your BepInEx Logger from outside of this class (eg Plugin.Log.LogMessage("");)
        internal static ManualLogSource Log;

        private static Harmony _harmony;

        private static InventoryStashService inventoryStashService;
        // If you need settings, define them like so:
        public static ConfigEntry<bool> ShowUnlearnedRecipies;
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
            _harmony.UnpatchSelf();
            inventoryStashService.deleteStash();
        }

        private void InitializeConfig()
        {
            ShowUnlearnedRecipies = Config.Bind(DISPLAY_NAME, "Show Unlearned Recipies", false, "This allows the Recipie Browser to show Unlearned Recipies");
        }

        // Update is called once per frame. Use this only if needed.
        // You also have all other MonoBehaviour methods available (OnGUI, etc)
        internal void Update()
        {
            //Log.LogMessage("This is an update");
        }


        // This is an example of a Harmony patch.
        // If you're not using this, you should delete it.
        [HarmonyPatch(typeof(ResourcesPrefabManager), nameof(ResourcesPrefabManager.Load))]
        public class ResourcesPrefabManager_Load
        {
            static void Postfix()
            {
                // This is a "Postfix" (runs after original) on ResourcesPrefabManager.Load
                // For more documentation on Harmony, see the Harmony Wiki.
                // https://harmony.pardeike.net/
            }
        }

    }
}
