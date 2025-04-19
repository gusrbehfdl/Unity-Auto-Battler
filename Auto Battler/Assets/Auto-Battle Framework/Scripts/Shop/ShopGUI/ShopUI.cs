using AutoBattleFramework.Shop;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AutoBattleFramework.Shop.ShopGUI
{
    /// <summary>
    /// It is in charge of all <see cref="ShopItemUI"/> creation, so that the player can buy items from the store.
    /// </summary>
    public class ShopUI : MonoBehaviour
    {
        /// <summary>
        /// Parent panel where all <see cref="ShopItemUI"/> creations will be entered.
        /// </summary>
        [Tooltip("Panel where the items in the store are displayed.")]
        public Image ItemList;

        /// <summary>
        /// List of all created <see cref="ShopItemUI"/>.
        /// </summary>
        List<ShopItemUI> currentList;

        /// <summary>
        /// Panel that appears when an item will be sold.
        /// </summary>
        [Tooltip("Panel that appears when an item is about to be sold.")]
        public Image SellForText;

        void Start()
        {

        }

        /// <summary>
        /// Instantiate a new <see cref="ShopItemUI"/> from an item and assign <see cref="ItemList"/> as its parent.
        /// </summary>
        /// <param name="item">Item information to create the UI.</param>
        /// <param name="cost">Cost that the item will have.</param>
        /// <param name="shop">ShopSystem reference.</param>
        public void AddItem(ShopItemInfo item, int cost, ShopManager shop)
        {
            GameObject itemPrefab = Instantiate(item.scriptableShopItem.shopItemUIPrefab.gameObject, ItemList.transform);
            ShopItemUI itemUI = itemPrefab.GetComponent<ShopItemUI>();
            itemUI.SetInfo(item.scriptableShopItem, item, cost, shop);
            if (currentList == null)
            {
                currentList = new List<ShopItemUI>();
            }
            currentList.Add(itemUI);
        }

        /// <summary>
        /// Returns a list of all currently created <see cref="ShopItemUI"/>s.
        /// </summary>
        /// <returns>List of all current <see cref="ShopItemUI"/>.</returns>
        public List<ShopItemUI> GetCurrentShop()
        {
            return currentList;
        }

        /// <summary>
        /// Destroy all current <see cref="ShopItemUI"/>'s.
        /// </summary>
        public void ClearList()
        {
            foreach (ShopItemUI ui in currentList)
            {
                Destroy(ui.gameObject);
            }
            currentList.Clear();
        }
    }
}