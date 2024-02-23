using BepInEx.Logging;
using HarmonyLib;
using RecipeBrowser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static MapMagic.ObjectPool;

namespace RecipeBrowser.Patches
{
    [HarmonyPatch(typeof(CharacterUI))]
    internal static class InventoryPatches
    {
        
        public delegate void ShowMenuDelegate(CharacterUI characterUI, CharacterUI.MenuScreens menu, Item item);
        public static event ShowMenuDelegate BeforeShowMenu;
        public static InventoryStashService inventoryStashService = new InventoryStashService();
        //Run this prefix before showing the inventory menu
        [HarmonyPatch(nameof(CharacterUI.ShowMenu))]
        [HarmonyPatch(new Type[] { typeof(CharacterUI.MenuScreens), typeof(Item) })]
        [HarmonyPrefix]
        private static void ShowMenuPrefix(CharacterUI __instance, CharacterUI.MenuScreens _menu, Item _item)
        {
            try
            {
                if (__instance.TargetCharacter == null || __instance.TargetCharacter.OwnerPlayerSys == null || !__instance.TargetCharacter.IsLocalPlayer)
                    return;
                if (_menu != CharacterUI.MenuScreens.Inventory)
                    return;
                inventoryStashService.createStashMenu(__instance, _menu, _item);
            } catch (Exception ex) {
                RecipeBrowser.Log.LogInfo("Failed to Run Open Inventory Is Menu Focused");
            }
            
        }
    }
}
