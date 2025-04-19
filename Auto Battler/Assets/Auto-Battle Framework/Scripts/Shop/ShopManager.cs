using AutoBattleFramework.BattleBehaviour.GameActors;
using AutoBattleFramework.Shop.ShopGUI;
using AutoBattleFramework.Shop.ShopList;
using AutoBattleFramework.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoBattleFramework.Shop
{
    /// <summary>
    /// Manages the items displayed in the store, and is in charge of buying and selling them for <see cref="currency"/>.
    /// </summary>
    public class ShopManager : MonoBehaviour
    {
        [Header("Levels and lists Settings")]
        /// <summary>
        /// Level manager, through which the <see cref="ShopList.IShopList"/> is obtained.
        /// </summary>
        [Tooltip("Manage the experience required to level up the store and the different shopping lists associated with each level.")]
        public ShopLevelManager shopLevelManager;

        /// <summary>
        /// Number of <see cref="ShopItemInfo"/> displayed in <see cref="shopUI"/>.
        /// </summary>
        [Tooltip("Number of items to display in the shop.")]
        public int numberOfItems;

        /// <summary>
        /// When extracting items from a <see cref="ShopList.IShopList"/>, show repeated items.
        /// </summary>
        [Tooltip("When displaying items, items can be repeated.")]
        public bool RepeatItems = false;

        /// <summary>
        /// When purchasing items, remove them from the list.
        /// </summary>
        [Tooltip("When an item is purchased, it is removed from the list.")]
        public bool RemoveFromListWhenBought = false;

        [Header("Currency Settings")]

        /// <summary>
        /// Amount of currency the player has. It is used to buy characters, items or experience from the store.
        /// </summary>
        [Tooltip("Amount of currency the player has.")]
        public int currency;

        /// <summary>
        /// Amount of <see cref="currency"/> to extract new items from the list.
        /// </summary>
        [Tooltip("How much does it cost to refresh the shop.")]
        public int RefreshCost = 2;

        /// <summary>
        /// Amount of <see cref="currency"/> to add experience to the <see cref="ShopLevelManager.CurrentExp"/>.
        /// </summary>
        [Tooltip("How much does it cost to buy experience.")]
        public int BuyExpCost = 4;

        /// <summary>
        /// Amount of experience bought and to be add to <see cref="ShopLevelManager.CurrentExp"/>.
        /// </summary>
        [Tooltip("How much experience to add when is bought.")]
        public int ExpToAdd = 4;

        [Header("UI Settings")]

        /// <summary>
        /// UI of the store, which displays the items to buy from <see cref="showList"/>.
        /// </summary>
        [Tooltip("Reference to the shop UI component.")]
        public ShopUI shopUI;

        [Header("Read-only")]

        /// <summary>
        /// List of items shown in <see cref="shopUI"/>.
        /// </summary>
        [Tooltip("List of items currently shown in the shop.")]
        [ReadOnly] public List<ShopItemInfo> showList;


        [Header("Edited Actors back up transforms")]

        /// <summary>
        /// "Transform where modified GameActors will be placed. Place far from the main game."
        /// </summary>
        [Tooltip("Transform where modified GameActors will be placed. Place far from the main game.")]
        public Transform GameActorsModified;

        /// <summary>
        /// "Transform where the backup of modified GameActors will be placed. Place far from the main game."
        /// </summary>
        [Tooltip("Transform where the backup of a State modified GameActors will be placed. Place far from the main game.")]
        public Transform GameActorsModifiedBackupState;

        /// <summary>
        /// "Transform where the backup of modified GameActors will be placed. Place far from the main game."
        /// </summary>
        [Tooltip("Transform where the backup of a Stage modified GameActors will be placed. Place far from the main game.")]
        public Transform GameActorsModifiedBackupStage;

        // Start is called before the first frame update
        void Start()
        {
            shopLevelManager.Initialize();
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Gets a random number of <see cref="ShopItemInfo"/> from the <see cref="ShopList.IShopList"/> associated with the <see cref="ShopLevelManager.CurrentLevel"/> based on the value of <see cref="numberOfItems"/>.
        /// Creates all the <see cref="ShopGUI.ShopItemUI"/> of the extracted items, allowing them to be purchased.
        /// </summary>
        public void GetRandomItems()
        {
            IShopList backup = shopLevelManager.GetCurrentList().Backup();
            if (shopUI.GetCurrentShop() != null)
            {
                shopUI.ClearList();
            }
            showList = shopLevelManager.GetCurrentList().GetRandomItems(numberOfItems, RepeatItems);

            foreach (ShopItemInfo item in showList)
            {
                shopUI.AddItem(item, item.itemCost, this);
            }
            shopLevelManager.SetCurrentList(backup);
        }

        /// <summary>
        /// Subtracts the <see cref="RefreshCost"/> from <see cref="currency"/> and retrieves a new set of items (see <see cref="GetRandomItems"/>).
        /// </summary>
        public void Refresh()
        {
            if (currency >= RefreshCost)
            {
                currency -= RefreshCost;
                GetRandomItems();
            }
        }

        /// <summary>
        /// Substracts the <see cref="BuyExpCost"/> from <see cref="currency"/> and adds <see cref="ExpToAdd"/> to the <see cref="ShopLevelManager.CurrentExp"/>.
        /// </summary>
        public void BuyExp()
        {
            if (currency >= BuyExpCost && !shopLevelManager.ShopMaxed())
            {
                currency -= BuyExpCost;
                shopLevelManager.AddExp(ExpToAdd);
            }
        }


        /// <summary>
        /// Remove a shop item from all list.
        /// </summary>
        /// <param name="info">Item to remove</param>
        public void RemoveShopItemFromAllLists(ShopItemInfo info)
        {
            foreach(ShopLevel level in shopLevelManager.shopLevels)
            {
                bool remove = level.list.RemoveItemInfo(info);                
            }
        }


        /// <summary>
        /// Restore a shop item that has been removed.
        /// </summary>
        /// <param name="info">Item to restore.</param>
        public void RestoreShopItemForAllLists(ScriptableShopItem info)
        {
            foreach(ShopLevel level in shopLevelManager.shopLevels)
            {
                bool restored = level.list.RestoreItemInfo(info);
            }
        }

        /// <summary>
        /// Edit a stat in runtime. Useful to create effects like "From now on X character purchased through the store has 1000 life", or similar.
        /// </summary>
        /// <param name="gameCharacter">Character to edit</param>
        /// <param name="modificator">Stat to edit. The value needs to be fixed.</param>
        public GameCharacter EditGameCharacterFromAllLists(GameCharacter gameCharacter, Stats.StatModificator modificator)
        {
            if (GameActorsModifiedBackupState.GetComponentsInChildren<GameCharacter>(true).Where(x => x.info.itemName == gameCharacter.info.itemName).FirstOrDefault() == null)
            {
                GameCharacter backup = Instantiate(gameCharacter, GameActorsModifiedBackupState);
                backup.gameObject.SetActive(false);
            }

            if (GameActorsModifiedBackupStage.GetComponentsInChildren<GameCharacter>(true).Where(x => x.info.itemName == gameCharacter.info.itemName).FirstOrDefault() == null)
            {
                GameCharacter backup = Instantiate(gameCharacter, GameActorsModifiedBackupStage);
                backup.gameObject.SetActive(false);
            }

            GameCharacter modifiedCharacter = GameActorsModified.GetComponentsInChildren<GameCharacter>(true).Where(x => x.info.itemName == gameCharacter.info.itemName).FirstOrDefault();

            if (modifiedCharacter == null)
            {
                modifiedCharacter = Instantiate(gameCharacter, GameActorsModified);
            }

            modifiedCharacter.gameObject.SetActive(false);

            modifiedCharacter.OriginalStats.SetStat(modificator.stat, modificator.value);
            modifiedCharacter.CurrentStats.SetStat(modificator.stat, modificator.value);
            modifiedCharacter.InitialStats.SetStat(modificator.stat, modificator.value);

            foreach (ShopLevel level in shopLevelManager.shopLevels)
            {
                level.list.ModifyGameActor(modifiedCharacter);
            }

            return modifiedCharacter;
        }

        /// <summary>
        /// Restore a character backup who has been edited in all list. 
        /// </summary>
        /// <param name="gameCharacter">Character to edit</param>
        /// <param name="modificator">Stat to edit. The value needs to be fixed.</param>
        public GameCharacter RestoreGameCharacterBackupForAllLists(GameCharacter gameCharacter, Transform backupParent)
        {
            GameCharacter modifiedCharacter = GameActorsModified.GetComponentsInChildren<GameCharacter>(true).Where(x => x.info.itemName == gameCharacter.info.itemName).FirstOrDefault();

            GameCharacter modifiedCharacterBackup = backupParent.GetComponentsInChildren<GameCharacter>(true).Where(x => x.info.itemName == gameCharacter.info.itemName).FirstOrDefault();

            if(modifiedCharacterBackup == null)
            {
                return null;
            }

            if (modifiedCharacter == null)
            {
                return null;
            }

            GameCharacter backup = Instantiate(modifiedCharacterBackup, GameActorsModified);
            backup.gameObject.SetActive(false);
            

            foreach (ShopLevel level in shopLevelManager.shopLevels)
            {
                level.list.ModifyGameActor(backup);
            }

            Destroy(modifiedCharacter.gameObject);
            Destroy(modifiedCharacterBackup.gameObject);

            return backup;
        }
    }
}