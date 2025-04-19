using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AutoBattleFramework.Shop;
using System.Linq;
using AutoBattleFramework.BattleBehaviour.GameActors;

namespace AutoBattleFramework.BattleBehaviour.Fusion
{
    /// <summary>
    /// Merge the characters in the list into another result.
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterFusion", menuName = "Auto-Battle Framework/GameCharacterFusion", order = 1)]
    public class GameCharacterFusion : ScriptableObject
    {
        /// <summary>
        /// List of characters to merge.
        /// </summary>
        public List<ShopCharacter> CharactersToFusion;

        /// <summary>
        /// Resulting character.
        /// </summary>
        public ShopCharacter FusionResult;

    }
}