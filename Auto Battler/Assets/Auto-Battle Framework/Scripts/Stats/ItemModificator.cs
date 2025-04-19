using AutoBattleFramework.BattleBehaviour;
using AutoBattleFramework.BattleBehaviour.GameActors;
using AutoBattleFramework.Shop;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.Stats
{
    /// <summary>
    /// Stat modificator related to a GameItem. Holds a reference to the ScriptableShopItem of such item.
    /// </summary>
    [System.Serializable]
    public class ItemModificator : StatsModificator
    {
        /// <summary>
        /// Item reference.
        /// </summary>
        [HideInInspector]
        public ScriptableShopItem scriptableShopItem;

        /// <summary>
        /// List of Traits that will be applied to the character when the item is equipped.
        /// </summary>
        public List<Trait> traits;

        /// <summary>
        /// Adds the item's modificator to the character's <see cref="BattleBehaviour.GameActors.GameCharacter.itemModificators"/>.
        /// Adds the traits in <see cref="traits"/> to <see cref="BattleBehaviour.GameActors.GameCharacter.traits"/>
        /// </summary>
        /// <param name="character">Character to be equipped with the item</param>
        public void AddItemModificator(GameCharacter character)
        {
            character.itemModificators.Add(this);
            sprite = scriptableShopItem.itemImage;
            AddStats(character, true);
            character.traits.AddRange(traits);
        }

        /// <summary>
        /// Removes the item's modificator from the character's <see cref="BattleBehaviour.GameActors.GameCharacter.itemModificators"/>.
        /// Removes the traits in <see cref="traits"/> from <see cref="BattleBehaviour.GameActors.GameCharacter.traits"/>
        /// </summary>
        /// <param name="character">Character to be equipped with the item</param>
        public void RemoveItemModificator(GameCharacter character)
        {
            if (character.itemModificators.Contains(this))
            {
                AddStats(character, false);

                foreach (Trait trait in traits)
                {
                    character.traits.Remove(trait);
                    if (!character.traits.Contains(trait)) {
                        trait.DeactivateOption(character, trait.ActivatedOption);
                    }
                }
                Battle.Instance.TraitCheck(Battle.Instance.teams[0].team, Battle.Instance.TeamBenches[0].GetGameCharacterInBench(), Battle.Instance.TraitsToCheck, Battle.Instance.TeamTraitListUI[0]);
                character.itemModificators.Remove(this);
            }

        }
    }

}