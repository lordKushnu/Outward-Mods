using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Linq;
using System.Text;
using UnityEngine;
using static MapMagic.ObjectPool;
using static PunTeams;

// RENAME 'OutwardModTemplate' TO SOMETHING ELSE
namespace ReplaceItemDescriptions
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class ReplaceItemDescriptions : BaseUnityPlugin
    {
        // Choose a GUID for your project. Change "myname" and "mymod".
        public const string GUID = "theGrungler.ReplaceItemDescriptions";
        // Choose a NAME for your project, generally the same as your Assembly Name.
        public const string NAME = "ReplaceItemDescriptions";
        // Increment the VERSION when you release a new version of your mod.
        public const string VERSION = "1.0.0";

        public const string DISPLAY_NAME = "ReplaceItemDescriptions";

        // For accessing your BepInEx Logger from outside of this class (eg Plugin.Log.LogMessage("");)
        internal static ManualLogSource Log;

        private static Harmony _harmony;

        // If you need settings, define them like so:
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
            //inventoryStashService = new InventoryStashService();
        }
        [HarmonyPatch(typeof(ResourcesPrefabManager), nameof(ResourcesPrefabManager.Load))]
        public class ReplacePrefabDescriptions
        {
            [HarmonyPostfix]
            public static void Load()
            {
                Log.LogMessage("Loading ResourcesPrefabManager");
                ResourcesPrefabManager prefabManager = ResourcesPrefabManager.Instance;
                if (prefabManager.Loaded != true)
                {
                    Log.LogMessage("Manager not Initialized");
                    return;
                }
                System.Collections.Generic.Dictionary<string, Item> itemPrefabs = ResourcesPrefabManager.ITEM_PREFABS;
                foreach (var kvp in itemPrefabs.ToArray())
                {
                    //if (kvp.Value.ItemID != 4100310)
                    //Log.LogMessage("Not the right Item");
                    //continue;
                    Item item = prefabManager.GetItemPrefab(kvp.Value.ItemID);
                    if (!item.IsUsable)
                        continue;
                    Transform itemTransform = item.transform;
                    if (itemTransform == null)
                    {
                        continue;
                    }
                    var effectTransform = itemTransform.Find("Effects");
                    if (effectTransform == null)
                    {
                        continue;
                    }
                    Effect[] addedStatusEffects = effectTransform.GetComponents<Effect>();
                    if (addedStatusEffects.Length == 0)
                        continue;
                    StringBuilder sb = new StringBuilder();
                    foreach (var effect in addedStatusEffects)
                    {
                        appendStatusText(sb, effect);

                    }
                    if (sb.Length == 0)
                    {
                        continue;
                    }
                    sb.Append("\n");
                    sb.Append(item.Description);

                    AccessTools.Field(typeof(Item), "m_localizedDescription").SetValue(item, sb.ToString());
                }
    
        }
        }

        static void appendStatusText(StringBuilder sb, Effect effect)
        {
            if (effect.GetType() == typeof(AddStatusEffect))
            {
                var effectName = ((AddStatusEffect)effect).Status;
                sb.AppendLine(effectName.IdentifierName);
            }
            else if (effect.GetType() == typeof(AffectBurntHealth))
            {
                var effectName = ((AffectBurntHealth)effect).AffectQuantity;
                sb.AppendLine("Burnt Health " + effectName.ToString("-#;+#;0"));
            }
            else if (effect.GetType() == typeof(AffectBurntMana))
            {
                var effectName = ((AffectBurntMana)effect).AffectQuantity;
                sb.AppendLine("Burnt Mana " + effectName.ToString("-#;+#;0"));
            }
            else if (effect.GetType() == typeof(AffectBurntStamina))
            {
                var effectName = ((AffectBurntStamina)effect).AffectQuantity;
                sb.AppendLine("Burnt Stamina " + effectName.ToString("-#;+#;0"));
            }
            else if (effect.GetType() == typeof(AffectCorruption))
            {
                var effectName = ((AffectCorruption)effect).AffectQuantity;
                sb.AppendLine("Corruption " + (effectName / 10).ToString("+#;-#;0") + "%");
            }
            else if (effect.GetType() == typeof(AffectHealth))
            {
                var effectName = ((AffectHealth)effect).AffectQuantity;
                sb.AppendLine("Health " + effectName.ToString("+#;-#;0"));
            }

            else if (effect.GetType() == typeof(AffectMana))
            {
                var effectName = ((AffectMana)effect).Value;
                sb.AppendLine("Mana " + effectName.ToString("+#;-#;0") + "%");
            }
            else if (effect.GetType() == typeof(AffectStamina))
            {
                var effectName = ((AffectStamina)effect).AffectQuantity;
                sb.AppendLine("Stamina " + effectName.ToString("+#;-#;0"));
            }

            else if (effect.GetType() == typeof(RemoveStatusEffect))
            {
                appendRemoveStatusEffect((RemoveStatusEffect)effect, sb);

            }
            else if (effect.GetType() == typeof(AffectFood))
            {
                var effectName = ((AffectFood)effect).m_affectQuantity / 10;
                sb.AppendLine("Food " + effectName + "%");
            }
            else if (effect.GetType() == typeof(AffectDrink))
            {
                var effectName = ((AffectDrink)effect).m_affectQuantity / 10;
                sb.AppendLine("Drink " + effectName + "%");
            }
        }

        static void appendRemoveStatusEffect(RemoveStatusEffect effect, StringBuilder sb)
        {
            var cleanseType = ((RemoveStatusEffect)effect).CleanseType;
            if (cleanseType == RemoveStatusEffect.RemoveTypes.StatusType)
            {
                sb.AppendLine("Remove " + effect.StatusType.Tag.TagName);
            }
            else if (cleanseType == RemoveStatusEffect.RemoveTypes.StatusFamily)
            {
                sb.AppendLine("Remove " + effect.StatusFamily.Internal_Get().Name);
            }
            else if (cleanseType == RemoveStatusEffect.RemoveTypes.StatusSpecific)
            {
                sb.AppendLine("Remove " + effect.StatusEffect.IdentifierName);
            }
            else if (cleanseType == RemoveStatusEffect.RemoveTypes.StatusNameContains)
            {
                sb.AppendLine("Remove " + effect.StatusName);
            }
            else if (cleanseType == RemoveStatusEffect.RemoveTypes.NegativeStatuses)
            {
                sb.AppendLine("Remove negative statuses");
            }
        }

        [HarmonyPatch(typeof(Item), nameof(Item.BaseInit))]
        public class Item_BaseInit
        {
            [HarmonyPostfix]
            public static void BaseInit(Item __instance)
            {
                if (!__instance.IsUsable)
                    return;
                Transform itemTransform = __instance.transform;
                if (itemTransform == null)
                {
                    return;
                }
                var effectTransform = itemTransform.Find("Effects");
                if (effectTransform == null)
                {
                    return;
                }
                Effect[] addedStatusEffects = effectTransform.GetComponents<Effect>();
                if (addedStatusEffects.Length == 0)
                    return;
                StringBuilder sb = new StringBuilder();
                Log.LogMessage(__instance.name);
                foreach (var effect in addedStatusEffects)
                {
                    appendStatusText(sb, effect);

                }
                if (sb.Length == 0)
                {
                    return;
                }
                sb.Append("\n");
                sb.Append(__instance.Description);

                AccessTools.Field(typeof(Item), "m_localizedDescription").SetValue(__instance, sb.ToString());

            }
        }

        internal void OnDestroy()
        {
            //inventoryStashService.deleteStash();
            _harmony?.UnpatchSelf();
            Log.LogMessage($"Destoryed");
            Config.Clear();
        }

        private void InitializeConfig()
        {
        }

        // Update is called once per frame. Use this only if needed.
        // You also have all other MonoBehaviour methods available (OnGUI, etc)
        internal void Update()
        {
            //Log.LogMessage("This is an update");
        }

    }
}
