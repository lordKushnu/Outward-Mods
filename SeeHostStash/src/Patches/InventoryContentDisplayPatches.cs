using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostInventoryStash.Patches
{
    [HarmonyPatch(typeof(InventoryContentDisplay))]
    internal class InventoryContentDisplayPatches
    {
        [HarmonyPatch(nameof(InventoryContentDisplay.RefreshContainerDisplays))]
        [HarmonyPatch(new Type[] { typeof(bool) })]
        [HarmonyPostfix]
        private static void RefreshContainerDisplaysPostfix(InventoryContentDisplay __instance, bool _clearAssignedDisplay)
        {
            try
            {
                HostInventoryStash.Log.LogInfo("Refreshing Container Display");
                HostInventoryStash.inventoryStashService.ConfigureStashDisplay(__instance);
            }
            catch (Exception ex)
            {
                HostInventoryStash.Log.LogInfo("Failed To Refresh Container Display");
            }
        }

        [HarmonyPatch(nameof(InventoryContentDisplay.FocusMostRelevantItem))]
        [HarmonyPatch(new Type[] { typeof(ItemListDisplay) })]
        [HarmonyPostfix]
        private static void FocusMostRelevantItemPostfix(InventoryContentDisplay __instance, ItemListDisplay _excludedList, bool __result)
        {
            HostInventoryStash.Log.LogInfo("FocusingMostRelevantItem");
        }
    }
}
