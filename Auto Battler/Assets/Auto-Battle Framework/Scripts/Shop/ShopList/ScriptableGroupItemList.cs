using AutoBattleFramework.BattleBehaviour.GameActors;
using AutoBattleFramework.Shop;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoBattleFramework.Shop.ShopList
{
    /// <summary>
    /// A list whose items are in groups, each with a fixed cost and probability that one of its items will be displayed in the store.
    /// </summary>
    [CreateAssetMenu(fileName = "GroupItemList", menuName = "Auto-Battle Framework/IShopList/ScriptableGroupItemList", order = 5)]
    public class ScriptableGroupItemList : IShopList
    {
        /// <summary>
        /// Group of items.
        /// </summary>
        public List<ShopItemList> GroupItemList;

        /// <summary>
        /// Group of items.
        /// </summary>
        List<ShopItemList> RemovedGroupItemList;

        /// <summary>
        /// Each item info is updated with the group information.
        /// </summary>
        public override void Initialize()
        {
            foreach (ShopItemList itemList in GroupItemList)
            {
                foreach (ShopItemInfo item in itemList.shopItems)
                {
                    item.itemCost = itemList.cost;
                    item.itemProbabilityWeight = itemList.probability;
                    item.scriptableShopItem = Instantiate(item.scriptableShopItem);
                } 
            }
        }

        void InitializeRemovedList()
        {
            RemovedGroupItemList = new List<ShopItemList>();
            foreach (ShopItemList itemList in GroupItemList)
            {
                ShopItemList removedList = new ShopItemList();
                removedList.cost = itemList.cost;
                removedList.probability = itemList.probability;
                removedList.shopItems = new List<ShopItemInfo>();
                RemovedGroupItemList.Add(removedList);
            }


        }

        /// <summary>
        /// Add a item information to a group, based on its cost.
        /// </summary>
        /// <param name="info">Information to be added.</param>
        /// <returns>The added info.</returns>
        public override ShopItemInfo AddItemInfo(ShopItemInfo info)
        {
            ShopItemList list = FindShopItemListByCost(info.itemCost);
            list.shopItems.Add(info);
            return info;
        }

        /// <summary>
        /// Return a number of items to be shown in <see cref="ShopGUI.ShopUI"/>, based on each group <see cref="ShopItemList.probability"/>.
        /// </summary>
        /// <param name="numberOfItems">Number of items to retrieve.</param>
        /// <param name="RepeatItems">The items retrieved can be repeated.</param>
        /// <returns>List of items retrieved.</returns>
        public override List<ShopItemInfo> GetRandomItems(int numberOfItems, bool RepeatItems)
        {
            int[] weights = GroupItemList.Select(x => x.probability).ToArray();
            List<ShopItemInfo> selectedItems = new List<ShopItemInfo>();

            List<ShopItemList> copyOfList = new List<ShopItemList>(GroupItemList);

            numberOfItems = Mathf.Min(numberOfItems, GroupItemList.Sum(x=>x.shopItems.Count));

            for (int i = 0; i < numberOfItems; i++)
            {
                int listIndex = GetRandomWeightedIndex(weights);
                ShopItemInfo selected = copyOfList[listIndex].shopItems[Random.Range(0, copyOfList[listIndex].shopItems.Count)];
                selectedItems.Add(selected);

                if (!RepeatItems)
                {
                    copyOfList[listIndex].shopItems.Remove(selected);
                    if (copyOfList[listIndex].shopItems.Count == 0)
                    {
                        copyOfList.RemoveAt(listIndex);
                        weights = copyOfList.Select(x => x.probability).ToArray();
                    }
                }

            }
            return selectedItems;
        }

        /// <summary>
        /// Remove the item information from its group.
        /// </summary>
        /// <param name="info">Information to be removed.</param>
        /// <returns>If item has been removed.</returns>
        public override bool RemoveItemInfo(ShopItemInfo info)
        {
            bool removed = false;
            if(RemovedGroupItemList == null)
            {
                InitializeRemovedList();
            }
            foreach(var list in GroupItemList)
            {
                int listIndex = GroupItemList.IndexOf(list);
                int removedNumber = list.shopItems.RemoveAll(x => x.scriptableShopItem == info.scriptableShopItem);

                if (removedNumber > 0)
                {
                    RemovedGroupItemList[listIndex].shopItems.Add(info);
                    removed = true;
                }
            }
            return removed;
        }

        /// <summary>
        /// Restore a <see cref="ShopItemInfo"/> that has been bought before. Returns true if successful.
        /// </summary>
        /// <param name="info">Item to be restored.</param>
        /// <returns>True if succesful</returns>
        public override bool RestoreItemInfo(ScriptableShopItem info)
        {
            bool restored = false;
            if(RemovedGroupItemList == null)
            {
                InitializeRemovedList();
            }
            foreach(var list in RemovedGroupItemList)
            {
                int listIndex = RemovedGroupItemList.IndexOf(list);
                ShopItemInfo sii = list.shopItems.Where(x => x.scriptableShopItem.shopItem == info.shopItem).FirstOrDefault();
                if(sii != null)
                {
                    int restoredNumber = list.shopItems.RemoveAll(x => x.scriptableShopItem.shopItem == info.shopItem);
                    if (restoredNumber > 0)
                    {
                        GroupItemList[listIndex].shopItems.Add(sii);
                        restored = true;
                    }
                }
            }
            return restored;
        }

        /// <summary>
        /// Find the group by its cost.
        /// </summary>
        /// <param name="cost">Cost of the items in a group</param>
        /// <returns>Group of items that have the given cost.</returns>
        ShopItemList FindShopItemListByCost(int cost)
        {
            foreach (ShopItemList sil in GroupItemList)
            {
                if (sil.cost == cost)
                {
                    return sil;
                }
            }
            return null;
        }

        /// <summary>
        /// Make a new instance of the list.
        /// </summary>
        /// <returns>New instance of the list.</returns>
        public override IShopList Backup()
        {
            return Instantiate(this);
        }

        /// <summary>
        /// Retrieve a single item from the list and add it to a list.
        /// </summary>
        /// <param name="items">List where the item will be added.</param>
        /// <param name="RepeatItems">The retrieved item can be repeated in the list.</param>
        /// <returns></returns>
        public override ShopItemInfo Draw(List<ShopItemInfo> items, bool RepeatItems)
        {
            ShopItemList sil = GroupItemList.Where(x => x.probability == items.First().itemProbabilityWeight && x.cost == items.First().itemCost).FirstOrDefault();
            List<ShopItemInfo> selectedItems = items;

            if (sil != null)
            {
                return sil.shopItems[Random.Range(0, sil.shopItems.Count)];
            }
            return null;
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
        /// Modify the stats of a GameActor.
        /// </summary>
        /// <param name="actor">GameActor to modify</param>
        public override void ModifyGameActor(GameActor actor)
        {
            foreach (ShopItemList sil in GroupItemList)
            {
                var sameItems = sil.shopItems.Where(x => x.scriptableShopItem.itemName == actor.info.itemName);
                foreach (var item in sameItems)
                {
                    item.scriptableShopItem.shopItem = actor;
                }
            }
        }
    }
}