using AutoBattleFramework.BattleBehaviour.GameActors;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoBattleFramework.Stats
{
    /// <summary>
    /// It represents traits that characters have, and when they are placed in the game next to another character sharing the same trait, they unlock effects such as improved stats.
    /// </summary>
    [CreateAssetMenu(fileName = "Trait", menuName = "Auto-Battle Framework/Traits/Trait", order = 6)]
    public class Trait : ScriptableObject
    {
        /// <summary>
        /// The trait name.
        /// </summary>
        public string TraitName;
        /// <summary>
        /// Sprite that represents the trait.
        /// </summary>
        public Sprite TraitImage;
        /// <summary>
        /// Descrìption of the trait.
        /// </summary>
        public string TraitDescription;

        /// <summary>
        /// List of effects unlocked because a certain number of characters sharing the same trait are in play.
        /// </summary>
        public List<TraitOption> TraitOptions;

        /// <summary>
        /// Current unlocked effect due to a certain number of characters sharing the same trait being in play.
        /// </summary>
        [HideInInspector]
        public TraitOption ActivatedOption;

        /// <summary>
        /// Previous unlocked effect due to a certain number of characters sharing the same trait being in play.
        /// </summary>
        [HideInInspector]
        public TraitOption PreviousOption;

        /// <summary>
        /// Total number of characters in play that share the same trait.
        /// </summary>
        [HideInInspector]
        public int TraitNumber = 0;

        /// <summary>
        /// Check if the goal of having a specific number of characters sharing the same trait in play has been achieved.
        /// </summary>
        /// <param name="characters">List of characters in play to check.</param>
        /// <param name="TraitCheckForDistinctCharacters">Set it to true if you want only totally different characters to be taken into account when checking a trait activation.</param>
        public void CheckForActivation(List<GameCharacter> characters, bool TraitCheckForDistinctCharacters)
        {
            if (TraitCheckForDistinctCharacters)
            {
                TraitNumber = characters.GroupBy(x => x.info.itemID).Select(x => x.FirstOrDefault()).Count(x => x.traits.Contains(this));
            }
            else
            {
                TraitNumber = characters.Count(x => x.traits.Contains(this));
            }

            List<TraitOption> options = TraitOptions.Where(x => x.NumberOfTraits <= TraitNumber).OrderByDescending(x => x.NumberOfTraits).ToList();
            if (options.Count > 0)
            {
                PreviousOption = ActivatedOption;
                ActivatedOption = TraitOptions.Where(x => x.NumberOfTraits <= TraitNumber).OrderByDescending(x => x.NumberOfTraits).First();
            }
            else
            {
                PreviousOption = ActivatedOption;
                ActivatedOption = null;
            }
        }

        /// <summary>
        /// Adds the effects of <see cref="ActivatedOption"/> if the conditions have been met.
        /// </summary>
        /// <param name="character">Character to which the effects of <see cref="ActivatedOption"/> will be applied.</param>
        public void ActivateOption(GameCharacter character)
        {
            if (ActivatedOption != null)
            {
                ActivatedOption.Activate(character, this);
            }
        }

        /// <summary>
        /// Substracts the effects of an <see cref="TraitOption"/>
        /// </summary>
        /// <param name="character">Character to which the effects of and <see cref="TraitOption"/> will be substracted.</param>
        /// <param name="option"><see cref="TraitOption"/> to be substracted.</param>
        public void DeactivateOption(GameCharacter character, TraitOption option)
        {
            if (option != null)
                option.Deactivate(character, this);
        }

        /// <summary>
        /// Checks if the <see cref="ActivatedOption"/> has changed.
        /// </summary>
        /// <returns>If <see cref="ActivatedOption"/> has changed.</returns>
        public bool OptionChange()
        {
            return ActivatedOption != PreviousOption;
        }

        /// <summary>
        /// Initialize the variables of the Trait.
        /// </summary>
        public void InitializeTrait()
        {
            ActivatedOption = null;
            PreviousOption = null;
        }

        public override bool Equals(object other)
        {
            Trait tOther = other as Trait;
            return tOther.TraitName == TraitName;
        }

        public override int GetHashCode()
        {
            System.HashCode hash = new System.HashCode();
            hash.Add(name);
            return hash.ToHashCode();
        }
    }
}