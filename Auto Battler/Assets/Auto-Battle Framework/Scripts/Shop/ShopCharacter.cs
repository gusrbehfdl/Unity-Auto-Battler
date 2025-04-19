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
    [CreateAssetMenu(fileName = "ShopCharacter", menuName = "Auto-Battle Framework/IShopList/ShopCharacter", order = 5)]
    public class ShopCharacter : ScriptableShopItem
    {

        /// <summary>
        /// The characters need to show their traits in the UI.
        /// </summary>
        /// <param name="ui">Shop item UI.</param>
        public override void ShowUIAdditional(ShopItemUI ui)
        {       
            GameCharacter character = shopItem as GameCharacter;

            foreach(Trait t in character.traits)
            {
                TraitStatsUI traitUI = Instantiate(ui.TraitPrefab, ui.ItemTraits.transform);
                traitUI.SetUI(t);
            }
        }
    }
}