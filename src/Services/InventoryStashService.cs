using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RecipeBrowser.Patches.InventoryPatches;
using UnityEngine;
using RecipeBrowser.Patches;

namespace RecipeBrowser.Services
{
    internal class InventoryStashService
    {
        public enum StashDisplayTypes
        {
            Disabled,
            Inventory,
            Equipment,
            Merchant
        };
        public static readonly HashSet<AreaManager.AreaEnum> stashAreas = new HashSet<AreaManager.AreaEnum>()
        {
            AreaManager.AreaEnum.CierzoVillage,
            AreaManager.AreaEnum.Monsoon,
            AreaManager.AreaEnum.Berg,
            AreaManager.AreaEnum.Levant,
            AreaManager.AreaEnum.Harmattan,
            AreaManager.AreaEnum.NewSirocco,
        };
        private Character _character;
        private CharacterUI _characterUI => _character.CharacterUI;
        private const string _inventoryDisplayPath = "Canvas/GameplayPanels/Menus/CharacterMenus/MainPanel/Content/MiddlePanel/Inventory/Content/SectionContent";
        private ContainerDisplay storedStashDisplay;
        private int timesCalled = 0;

        public bool GetAreaContainsStash() => TryGetCurrentAreaEnum(out var area) && stashAreas.Contains(area);

        public int Start()
        {
            timesCalled++;
            return timesCalled;
        }

        public void createStashMenu(CharacterUI characterUI, CharacterUI.MenuScreens menu, Item item)
        {
            if (menu != CharacterUI.MenuScreens.Inventory && menu != CharacterUI.MenuScreens.Equipment
                && menu != CharacterUI.MenuScreens.Shop)
                return;

            ToggleStashes(characterUI);
        }
        private void ToggleStashes(CharacterUI _instance)
        {
            var inventoryContentDisplay = _instance.transform.Find(_inventoryDisplayPath).GetComponent<InventoryContentDisplay>();
            AddStashDisplay(_instance, inventoryContentDisplay);
        }

        private void AddStashDisplay(CharacterUI _instance, InventoryContentDisplay inventoryContentDisplay)
        {
            var inventoryPath = inventoryContentDisplay.transform.GetGameObjectPath();
            if (inventoryPath.EndsWith(_inventoryDisplayPath))
            {
                //It's an Inventory
                if (!GetAreaContainsStash()) //Area doesn't contain stash
                {
                    deleteStash();
                }
                else if(storedStashDisplay == null)
                {
                    RecipeBrowser.Log.LogInfo($"creating new stash display");
                    RectTransform parentTransform = inventoryContentDisplay.m_overrideContentHolder;
                    if (parentTransform == null)
                        parentTransform = inventoryContentDisplay.m_inventoriesScrollRect?.content;
                    var containerDisplayPrefab = inventoryContentDisplay.ContainerDisplayPrefab;
                    ContainerDisplay stashDisplay = UnityEngine.Object.Instantiate(containerDisplayPrefab).GetComponent<ContainerDisplay>();
                    stashDisplay.transform.SetParent(parentTransform);
                    stashDisplay.transform.ResetLocal();
                    stashDisplay.name = "HostStashDisplay";
                    stashDisplay.SetName("Host Display");

                    var header = stashDisplay.transform.Find("Header");
                    if(header != null)
                    {
                        var lblWeight = (RectTransform)header.transform.Find("lblWeight");
                        if(lblWeight != null)
                        {
                            UnityEngine.Object.Destroy(lblWeight.gameObject);
                        }
                        var iconWeight = header.transform.Find("iconWeight");
                        if(iconWeight != null)
                        {
                            UnityEngine.Object.Destroy(iconWeight.gameObject);
                        }
                    }
                    storedStashDisplay = stashDisplay;
                    ShowStashPanel(_instance);
                }
                else
                {
                    RecipeBrowser.Log.LogInfo($"Stash Display already exists");
                    ShowStashPanel(_instance);
                }
            }

            else
            {
                RecipeBrowser.Log.LogInfo($"not an Inventory?");
            }
        }

        private void ShowStashPanel(CharacterUI instance)
        {
            RecipeBrowser.Log.LogInfo($"Stash in Area: {GetAreaContainsStash()}");
            var charHost = CharacterManager.Instance.GetWorldHostCharacter();
            var localCharacter = CharacterManager.Instance.GetFirstLocalCharacter();
            var localStash = localCharacter.Stash;
            ItemContainer hostStash = charHost.Stash;
            var localStashPanel = localCharacter.CharacterUI.StashPanel;
            localStashPanel.SetStash(hostStash);
            hostStash.ShowContent(localCharacter);
            storedStashDisplay.SetReferencedContainer(hostStash);
        }
        public void deleteStash()
        {
            if(storedStashDisplay != null)
            {
                storedStashDisplay.gameObject.SetActive(false);
                GameObject.Destroy(storedStashDisplay.gameObject);
            }
            
        }

        public static bool TryGetCurrentAreaEnum(out AreaManager.AreaEnum area)
        {
            area = default;
            var sceneName = AreaManager.Instance?.CurrentArea?.SceneName;
            if (string.IsNullOrEmpty(sceneName))
                return false;

            area = (AreaManager.AreaEnum)AreaManager.Instance.GetAreaIndexFromSceneName(sceneName);
            return true;
        }
    }


}
