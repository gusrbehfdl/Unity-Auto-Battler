using AutoBattleFramework.BattleBehaviour.GameActors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.Shop.ShopList
{
    /// <summary>
    /// Contains the list of <see cref="ShopItemInfo"/> for purchasing characters and items. It handles the methods for adding and removing <see cref="ShopItemInfo"/> from the list, as well as getting a number of random <see cref="ShopItemInfo"/> to be displayed in the <see cref="ShopGUI.ShopUI"/>.
    /// </summary>
    public abstract class IShopList : ScriptableObject
    {

        /// <summary>
        /// Adds a <see cref="ShopItemInfo"/> to the list.
        /// </summary>
        /// <param name="info">Item to be added.</param>
        /// <returns>The added item.</returns>
        public abstract ShopItemInfo AddItemInfo(ShopItemInfo info);

        /// <summary>
        /// Removes a <see cref="ShopItemInfo"/> from the list. Returns true if successful.
        /// </summary>
        /// <param name="info">Item to be removed.</param>
        /// <returns>True if succesful</returns>
        public abstract bool RemoveItemInfo(ShopItemInfo info);

        /// <summary>
        /// Restore a <see cref="ShopItemInfo"/> that has been bought before. Returns true if successful.
        /// </summary>
        /// <param name="info">Item to be restored.</param>
        /// <returns>True if succesful</returns>
        public abstract bool RestoreItemInfo(ScriptableShopItem info);

        /// <summary>
        /// Get a list of random <see cref="ShopItemInfo"/> from the list.
        /// </summary>
        /// <param name="numberOfItems">Number of items to retrieve</param>
        /// <param name="RepeatItems">Allow repeated items</param>
        /// <returns>List of items from the lsit.</returns>
        public abstract List<ShopItemInfo> GetRandomItems(int numberOfItems, bool RepeatItems);

        /// <summary>
        /// Method called when the list is created. Used if it is necessary to initialize some variable of the list.
        /// </summary>
        public virtual void Initialize()
        {

        }

        /// <summary>
        /// Retrieves a single <see cref="ShopItemInfo"/> from the list and add it to an exisiting list.
        /// </summary>
        /// <param name="items">Item list</param>
        /// <param name="RepeatItems">Allow to retrieve an exisiting item in the list.</param>
        /// <returns></returns>
        public abstract ShopItemInfo Draw(List<ShopItemInfo> items, bool RepeatItems);

        /// <summary>
        /// It makes a copy of the list. It is used to not overwrite the ScriptableObject.
        /// </summary>
        /// <returns></returns>
        public abstract IShopList Backup();

        /// <summary>
        /// Method called when an object is purchased from the list.
        /// </summary>
        /// <param name="info">The bought item.</param>
        public abstract void OnBuy(ShopItemInfo info);

        /// <summary>
        /// Modify the stats of a GameActor.
        /// </summary>
        /// <param name="actor">GameActor to modify</param>
        public abstract void ModifyGameActor(GameActor actor);

        /// <summary>
        /// Given a list of weights, return a list of random indices. Used to extract <see cref="ShopItemInfo"/> based on their <see cref="ShopItemInfo.itemProbabilityWeight"/>.
        /// </summary>
        /// <param name="weights"></param>
        /// <returns></returns>
        protected int GetRandomWeightedIndex(int[] weights)
        {
            // Get the total sum of all the weights.
            int weightSum = 0;
            for (int i = 0; i < weights.Length; ++i)
            {
                weightSum += weights[i];
            }

            // Step through all the possibilities, one by one, checking to see if each one is selected.
            int index = 0;
            int lastIndex = weights.Length - 1;
            while (index < lastIndex)
            {
                // Do a probability check with a likelihood of weights[index] / weightSum.
                if (Random.Range(0, weightSum) < weights[index])
                {
                    return index;
                }

                // Remove the last item from the sum of total untested weights and try again.
                weightSum -= weights[index++];
            }

            // No other item was selected, so return very last index.
            return index;
        }
    }
}