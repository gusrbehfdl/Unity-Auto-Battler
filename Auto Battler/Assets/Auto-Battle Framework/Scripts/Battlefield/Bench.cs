using AutoBattleFramework.BattleBehaviour.GameActors;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoBattleFramework.Battlefield
{
    /// <summary>
    /// The same as a grid, but is used for easy differentiation between the <see cref="BattleGrid"/> and the bench. The Custom Inspector allows to create a bench quickly.
    /// </summary>
    public class Bench : BattleGrid
    {

        /// <summary>
        /// Get all GameCharacters in the bench.
        /// </summary>
        /// <returns>List of GameCharacters in the bench.</returns>
        public List<GameCharacter> GetGameCharacterInBench()
        {
            return GridCells.Where(x => x.shopItem != null).Select(x => x.shopItem as GameCharacter).ToList();
        }

        /// <summary>
        /// Get all GameItems in the bench.
        /// </summary>
        /// <returns>List of GameItems in the bench.</returns>
        public List<GameItem> GetGameItemsInBench()
        {
            return GridCells.Where(x => x.shopItem != null).Select(x => x.shopItem as GameItem).ToList();
        }

        /// <summary>
        /// Get all ShopItems in the bench.
        /// </summary>
        /// <returns>List of ShopItems in the bench.</returns>
        public List<GameActor> GetShopItemInBench()
        {
            return GridCells.Where(x => x.shopItem != null).Select(x => x.shopItem).ToList();
        }

    }
}