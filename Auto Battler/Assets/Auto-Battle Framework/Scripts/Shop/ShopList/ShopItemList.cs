using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.Shop.ShopList
{
    /// <summary>
    /// List used in <see cref="Shop.ShopList.ScriptableGroupItemList"/> to display items in the store. All items belonging to this list will have a fixed probability of appearance and price, regardless of their <see cref="ShopItemInfo.itemProbabilityWeight"/> and <see cref="ShopItemInfo.itemCost"/>.
    /// </summary>
    [System.Serializable]
    public class ShopItemList
    {
        /// <summary>
        /// Probability that an item from this list will appear in the store (ignoring <see cref="ShopItemInfo.itemProbabilityWeight"/>).
        /// </summary>
        public int probability = 50;

        /// <summary>
        /// Cost of an item from the list that appears in the store (ignoring <see cref="ShopItemInfo.itemCost"/>).
        /// </summary>
        public int cost = 1;

        /// <summary>
        /// List of items with a fixed <see cref="probability"/> and <see cref="cost"/>.
        /// </summary>
        public List<ShopItemInfo> shopItems;
    }
}