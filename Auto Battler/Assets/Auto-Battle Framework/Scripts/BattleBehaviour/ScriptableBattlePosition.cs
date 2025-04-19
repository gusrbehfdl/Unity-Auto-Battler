using AutoBattleFramework.Battlefield;
using AutoBattleFramework.Shop;
using AutoBattleFramework.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.BattleBehaviour
{
    /// <summary>
    /// It contains the positions of a <see cref="CharacterGroup"/>, which will serve to spawn a random member of that group. 
    /// It has a custom inspector to facilitate the positioning of the groups.
    /// </summary>
    [CreateAssetMenu(fileName = "BattlePosition", menuName = "Auto-Battle Framework/BattlePosition/BattlePosition", order = 3)]
    public class ScriptableBattlePosition : ScriptableObject
    {
        /// <summary>
        /// Texture used in the custom inspector to represent the grid cells.
        /// </summary>
        public Texture GridCellTexture;

        /// <summary>
        /// Contains the indexes of the groups in <see cref="characterGroups"/>, so that they coincide with a grid cell.
        /// </summary>
        [HideInInspector]
        public int[] battlePositions;

        /// <summary>
        /// List of character groups.
        /// </summary>
        public List<CharacterGroup> characterGroups;

        /// <summary>
        /// Selected grid cell index.
        /// </summary>
        [HideInInspector]
        public int selected;

        public GUIStyle s;

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void Reset()
        {
            Battle battle = Battle.Instance;

            if (!battle)
            {
                battle = FindObjectOfType<Battle>();
            }

            if (battle.grid.GridShape == BattleGrid.GridType.Hex)
            {
                GridCellTexture = AutoBattleSettings.GetOrCreateSettings().defaultHexTexture;
            }
            else
            {
                GridCellTexture = AutoBattleSettings.GetOrCreateSettings().defaultSquaredTexture;
            }
            characterGroups = new List<CharacterGroup>();
        }

        /// <summary>
        /// Given an index, returns the <see cref="CharacterGroup"/> from <see cref="characterGroups"/>.
        /// </summary>
        /// <param name="index">Index of character group</param>
        public CharacterGroup GetCharacterGroup(int index)
        {
            if (index > 0)
            {
                int bpindex = battlePositions[index];
                bpindex--;
                if (bpindex >= 0)
                {
                    CharacterGroup group = characterGroups[bpindex];
                    return group;
                }
            }
            return null;
        }

        /// <summary>
        /// Given an index, returns the list of <see cref="Shop.ShopCharacter"/> from a <see cref="CharacterGroup"/> contained in <see cref="characterGroups"/>.
        /// </summary>
        /// <param name="index">Index of character group</param>
        /// <returns></returns>
        public List<ShopCharacter> GetCharacterGroupList(int index)
        {
            CharacterGroup group = GetCharacterGroup(index);
            if (group != null)
            {
                return group.characterGroup;
            }
            return null;
        }
    }
}