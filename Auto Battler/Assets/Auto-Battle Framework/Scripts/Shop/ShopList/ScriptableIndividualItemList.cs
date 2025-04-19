using AutoBattleFramework.BattleBehaviour.GameActors;
using AutoBattleFramework.Shop;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoBattleFramework.Shop.ShopList
{
    /// <summary>
    /// A list of items, each with its own cost and probability of appearing in the shop.
    /// </summary>
    [CreateAssetMenu(fileName = "IndividualItemList", menuName = "Auto-Battle Framework/IShopList/ScriptableIndividualItemList", order = 5)]
    public class ScriptableIndividualItemList : IShopList
    {
        /// <summary>
        /// List of items
        /// </summary>
        public List<ShopItemInfo> IndividualShopList;

        /// <summary>
        /// List of removed items.
        /// </summary>
        List<ShopItemInfo> RemovedItems;

        /// <summary>
        /// Add the information of a item to the list.
        /// </summary>
        /// <param name="info">Item information</param>
        /// <returns>Added information</returns>
        public override ShopItemInfo AddItemInfo(ShopItemInfo info)
        {
            IndividualShopList.Add(info);
            return IndividualShopList.Last();
        }

        /// <summary>
        /// Makes a new instance of the list.
        /// </summary>
        /// <returns></returns>
        public override IShopList Backup()
        {
            return Instantiate(this);
        }

        public override void Initialize()
        {
            foreach (ShopItemInfo item in IndividualShopList)
            {
                item.scriptableShopItem = Instantiate(item.scriptableShopItem);
            }
        }

        /// <summary>
        /// Retrieve a single item from the list and add it to a list.
        /// </summary>
        /// <param name="items">List of items where the item will be added.</param>
        /// <param name="RepeatItems">The retrieved item can be repeated in the list.</param>
        /// <returns>The retrieved item.</returns>
        public override ShopItemInfo Draw(List<ShopItemInfo> items, bool RepeatItems)
        {
            int[] weights = IndividualShopList.Select(x => x.itemProbabilityWeight).ToArray();
            List<ShopItemInfo> selectedItems = items;

            List<ShopItemInfo> copyOfList = new List<ShopItemInfo>(IndividualShopList);

            if (!RepeatItems)
            {
                for (int i = 0; i < selectedItems.Count; i++)
                {
                    copyOfList.Remove(selectedItems[i]);
                }
            }

            int index = GetRandomWeightedIndex(weights);
            selectedItems.Add(copyOfList[index]);

            return copyOfList[index];
        }

        /// <summary>
        /// Return a number of items to be shown in <see cref="ShopGUI.ShopUI"/>, based on each item <see cref="ShopItemInfo.itemCost"/>.
        /// </summary>
        /// <param name="numberOfItems">Number of items to retrieve.</param>
        /// <param name="RepeatItems">The items retrieved can be repeated.</param>
        /// <returns></returns>
        public override List<ShopItemInfo> GetRandomItems(int numberOfItems, bool RepeatItems)
        {
            int[] weights = IndividualShopList.Select(x => x.itemProbabilityWeight).ToArray();
            List<ShopItemInfo> selectedItems = new List<ShopItemInfo>();

            List<ShopItemInfo> copyOfList = new List<ShopItemInfo>(IndividualShopList);

            numberOfItems = Mathf.Min(numberOfItems, copyOfList.Count);

            for (int i = 0; i < numberOfItems; i++)
            {
                int index = GetRandomWeightedIndex(weights);
                selectedItems.Add(copyOfList[index]);

                if (!RepeatItems)
                {
                    copyOfList.RemoveAt(index);
                    weights = copyOfList.Select(x => x.itemProbabilityWeight).ToArray();
                }

            }
            return selectedItems;
        }

        /// <summary>
        /// On buy do nothing.
        /// </summary>
        /// <param name="info">Bought item</param>
        public override void OnBuy(ShopItemInfo info)
        {
            //
        }

        /// <summary>
        /// Remove the item information from the list.
        /// </summary>
        /// <param name="info">Information to be removed.</param>
        /// <returns>If item has been removed.</returns>
        public override bool RemoveItemInfo(ShopItemInfo info)
        {
            if (RemovedItems==null)
            {
                RemovedItems = new List<ShopItemInfo>();
            }
            ShopItemInfo sii = IndividualShopList.Where(x => x.scriptableShopItem.shopItem == info.scriptableShopItem.shopItem).FirstOrDefault();
            RemovedItems.Add(sii);
            return IndividualShopList.Remove(sii);
        }

        public override bool RestoreItemInfo(ScriptableShopItem info)
        {
            if (RemovedItems == null)
            {
                RemovedItems = new List<ShopItemInfo>();
            }
            ShopItemInfo sii = RemovedItems.Where(x => x.scriptableShopItem.shopItem == info.shopItem).FirstOrDefault();
            if (sii == null)
            {
                return false;
            }
            else
            {
                IndividualShopList.Add(sii);
                RemovedItems.Remove(sii);
                return true;
            }

        }

        /// <summary>
        /// Modify the stats of a GameActor.
        /// </summary>
        /// <param name="actor">GameActor to modify</param>
        public override void ModifyGameActor(GameActor actor)
        {
            var sameItems = IndividualShopList.Where(x => x.scriptableShopItem.itemName == actor.info.itemName);

            foreach (ShopItemInfo item in sameItems)
            {
                item.scriptableShopItem.shopItem = actor;
            }
        }
    }
}