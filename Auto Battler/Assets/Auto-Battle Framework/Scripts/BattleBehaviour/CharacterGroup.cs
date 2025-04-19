using AutoBattleFramework.Shop;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.BattleBehaviour
{
    /// <summary>
    /// Contains the possible characters that can spawn in a cell, being used by <see cref="ScriptableBattlePosition"/> Inspector for easy stage customization.
    /// </summary>
    [System.Serializable]
    public class CharacterGroup
    {
        /// <summary>
        /// List of possible characters that can appear in a cell. One will be selected at random.
        /// </summary>
        public List<ShopCharacter> characterGroup;

        public CharacterGroup()
        {
            characterGroup = new List<ShopCharacter>();
        }

        /// <summary>
        /// Returns the image of the first character in the group. It is used in the <see cref="ScriptableBattlePosition"/> inspector in order to differentiate groups.
        /// </summary>
        public Texture GetGroupTexture()
        {
            if (characterGroup != null)
            {
                if (characterGroup.Count > 0)
                {
                    if (characterGroup[0] != null)
                    {
                        if (characterGroup[0].itemImage != null)
                        {
                            return characterGroup[0].itemImage.texture;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a random character from <see cref="characterGroup"/>. It will be used to spawn it and give enemy variety to the different phases.
        /// </summary>
        public ShopCharacter GetRandomCharacter()
        {
            if (characterGroup != null)
            {
                if (characterGroup.Count > 0)
                {
                    return characterGroup[Random.Range(0, characterGroup.Count)];
                }
            }
            return null;
        }
    }
}