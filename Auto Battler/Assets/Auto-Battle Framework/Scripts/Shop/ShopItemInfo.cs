using AutoBattleFramework.BattleBehaviour.GameActors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.Shop
{
    /// <summary>
    /// Purchase information, price and probability of appearing in the <see cref="ShopManager"/> of the <see cref="BattleBehaviour.GameActors.GameActor"/>.
    /// </summary>
    [System.Serializable]
    public class ShopItemInfo
    {
        /// <summary>
        /// The information of the character or object that can be purchased.
        /// </summary>
        public ScriptableShopItem scriptableShopItem;

        /// <summary>
        /// Purchase price of the <see cref="ScriptableShopItem.shopItem"/>.
        /// </summary>
        public int itemCost;

        /// <summary>
        /// Individual probability of occurrence of the <see cref="BattleBehaviour.GameActors.GameActor"/> in the store. This can be ignored depending on the type of <see cref="Shop.ShopList.IShopList"/> being used.
        /// </summary>
        public int itemProbabilityWeight;
    }
}