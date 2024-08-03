using BepInEx.Logging;
using HarmonyLib;
using HostInventoryStash.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static MapMagic.ObjectPool;

namespace HostInventoryStash.Patches
{
    [HarmonyPatch(typeof(CharacterUI))]
    internal static class CharacterUiPatches
    {
        
        public delegate void ShowMenuDelegate(CharacterUI characterUI, CharacterUI.MenuScreens menu, Item item);
        public static event ShowMenuDelegate BeforeShowMenu;
        //Run this prefix before showing the inventory menu
        
        [HarmonyPatch(nameof(CharacterUI.ShowMenu))]
        [HarmonyPatch(new Type[] { typeof(CharacterUI.MenuScreens), typeof(Item) })]
        [HarmonyPrefix]
        private static void ShowMenuPrefix(CharacterUI __instance, CharacterUI.MenuScreens _menu, Item _item)
        {
            HostInventoryStash.Log.LogInfo("ShowMenuHarmonyPatch");
            try
            {
                if (__instance.TargetCharacter == null || __instance.TargetCharacter.OwnerPlayerSys == null || !__instance.TargetCharacter.IsLocalPlayer)
                    return;
                if (_menu != CharacterUI.MenuScreens.Inventory)
                    return;
                HostInventoryStash.Log.LogInfo("Create Stash Menu Patch");
                HostInventoryStash.inventoryStashService.createStashMenu(__instance, _menu, _item);
            } catch (Exception ex) {
                HostInventoryStash.Log.LogInfo("Failed to Run Open Inventory Is Menu Focused");
            }
        }
        
    }
}
