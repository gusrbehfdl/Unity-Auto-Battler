using AutoBattleFramework.Shop.ShopGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AutoBattleFramework.Stats;
using AutoBattleFramework.BattleUI;
using AutoBattleFramework.BattleBehaviour.GameActors;

namespace AutoBattleFramework.Shop
{
    /// <summary>
    /// Derived from <see cref="ScriptableShopItem"/>. Allows easier navigation in the Unity engine to find items of a specific nature, such as assigning to lists that only allow characters.
    /// </summary>
    [CreateAssetMenu(fileName = "ShopGameItem", menuName = "Auto-Battle Framework/IShopList/ShopGameItem", order = 5)]
    public class ShopGameItem : ScriptableShopItem
    {
        /// <summary>
        /// Game items do not show any additional information.
        /// </summary>
        /// <param name="ui"></param>
        public override void ShowUIAdditional(ShopItemUI ui)
        {
            //Game items do not show any additional information.
        }
    }
}