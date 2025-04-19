using AutoBattleFramework.Shop.ShopList;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.Shop
{
    /// <summary>
    /// A Shop level contains a specific list. When a sufficient level of experience is reached, the level increases and another list is applied.
    /// </summary>
    [System.Serializable]
    public class ShopLevel
    {
        /// <summary>
        /// List applied to this level.
        /// </summary>
        public IShopList list;

        /// <summary>
        /// Required experience level to level up the shop.
        /// </summary>
        public int ExpRequired;
    }
}