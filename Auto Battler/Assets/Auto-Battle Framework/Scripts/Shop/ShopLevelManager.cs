using AutoBattleFramework.Shop.ShopList;
using AutoBattleFramework.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.Shop
{
    /// <summary>
    /// Manage the <see cref="ShopLevel"/>, the <see cref="ShopLevel.list"/> of the current shop level, and the experience to level up the shop.
    /// </summary>
    [System.Serializable]
    public class ShopLevelManager
    {
        /// <summary>
        /// List of store levels.
        /// </summary>
        public List<ShopLevel> shopLevels;

        /// <summary>
        /// Current experience of the shop. When reached the current <see cref="ShopLevel.ExpRequired"/>, the <see cref="CurrentLevel"/> goes up by one.
        /// </summary>
        [ReadOnly] public int CurrentExp;

        /// <summary>
        /// Current level of the shop.
        /// </summary>
        [ReadOnly] public int CurrentLevel = 0;

        /// <summary>
        /// Adds experience to the <see cref="CurrentExp"/>. If <see cref="ShopLevel.ExpRequired"/> is reached, the <see cref="CurrentLevel"/> goes up by one.
        /// </summary>
        /// <param name="amount"></param>
        public void AddExp(int amount)
        {
            CurrentExp += amount;
            if (CurrentLevel+1 < shopLevels.Count) {
                if (CurrentExp >= shopLevels[CurrentLevel+1].ExpRequired)
                {
                    int remainingExp = CurrentExp - shopLevels[CurrentLevel+1].ExpRequired;
                    CurrentExp = 0;
                    CurrentLevel++;
                    //Check if the shop has been leveled one more level
                    AddExp(remainingExp);
                }
            }
        }

        /// <summary>
        /// Initialize all levels in <see cref="shopLevels"/>.
        /// </summary>
        public void Initialize()
        {
            foreach(ShopLevel level in shopLevels)
            {
                level.list = ScriptableObject.Instantiate(level.list);
                level.list.Initialize();
                level.list = level.list.Backup();
            }
        }

        /// <summary>
        /// Returns true if the maximum shop level has been reached.
        /// </summary>
        /// <returns>If the maximum shop level has been reached.</returns>
        public bool ShopMaxed()
        {
            if (CurrentLevel + 1 < shopLevels.Count)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get the <see cref="ShopLevel.list"/> of the current level.
        /// </summary>
        /// <returns>List of the current level.</returns>
        public IShopList GetCurrentList()
        {
            return shopLevels[CurrentLevel].list;
        }

        /// <summary>
        /// Set the list of the current level.
        /// </summary>
        /// <param name="list">List with which the current level will be updated.</param>
        public void SetCurrentList(IShopList list)
        {
            shopLevels[CurrentLevel].list = list;
        }

    }
}
