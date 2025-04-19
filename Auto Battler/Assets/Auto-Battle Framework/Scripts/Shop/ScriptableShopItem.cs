using AutoBattleFramework.BattleBehaviour.GameActors;
using AutoBattleFramework.Shop.ShopGUI;
using AutoBattleFramework.Utility;
using UnityEngine;

namespace AutoBattleFramework.Shop
{
    /// <summary>
    /// Allows the item to be sold in the shop. Defines the name of the item and the image to be displayed in the shop.
    /// </summary>
    public abstract class ScriptableShopItem : ScriptableObject
    {
        /// <summary>
        /// Name of the item
        /// </summary>
        public string itemName;

        /// <summary>
        /// Item identification. Used to check if a character or item is the same, but one level higher.
        /// </summary>
        public string itemID;

        /// <summary>
        /// Item that can be purchased in the shop.
        /// </summary>
        public GameActor shopItem;

        /// <summary>
        /// Image of the item that will be displayed in <see cref="Shop.ShopGUI.ShopItemUI.ItemImage"/>.
        /// </summary>
        public Sprite itemImage;

        /// <summary>
        /// Image of the item that will be displayed in <see cref="BattleUI.CharacterStatsUI"/>. If null, the image displayed is <see cref="itemImage"/>.
        /// </summary>
        public Sprite descriptionImage;

        /// <summary>
        /// Prefab of the item interface in the shop.
        /// </summary>
        public ShopItemUI shopItemUIPrefab;

        /// <summary>
        /// Description of the item.
        /// </summary>
        public string itemDescription;

        private void Reset()
        {
            shopItemUIPrefab = AutoBattleSettings.GetOrCreateSettings().defaultCharacterShopItemUI;
        }

        /// <summary>
        /// Displays additional information in the <see cref="Shop.ShopGUI.ShopItemUI"/> of the item in the store.
        /// </summary>
        /// <param name="ui">Shop item UI</param>
        public abstract void ShowUIAdditional(ShopItemUI ui);
    }
}