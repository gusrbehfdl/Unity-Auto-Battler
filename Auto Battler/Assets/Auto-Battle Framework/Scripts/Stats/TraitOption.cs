using AutoBattleFramework.BattleBehaviour.GameActors;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoBattleFramework.Stats
{
    /// <summary>
    /// Represents the effects that will be applied to a set of characters when the condition of having a certain number of characters in play is met.
    /// </summary>
    [System.Serializable]
    public class TraitOption
    {
        /// <summary>
        /// Number of characters in play required for this option to be activated.
        /// </summary>
        public int NumberOfTraits;

        /// <summary>
        /// Modification of the stats that will be applied to the characters when this option is applied.
        /// </summary>
        public StatsModificator modificator;

        /// <summary>
        /// Description of the effects of this option.
        /// </summary>
        public string OptionDescription;

        /// <summary>
        /// Description of the effects of this option.
        /// </summary>
        public Color TraitOptionColor;

        /// <summary>
        /// To whom the effects of this option apply. <br />
        /// Characters With Trait - To all the characters that share this trait. <br />
        /// All Characters - To all characters in player´s team.
        /// Selected Traits - To all characters that share at least one Trait in <see cref="SelectedTraits"/>.
        /// </summary>
        public enum TraitTarget
        {
            CharactersWithTrait,
            AllCharacters,
            SelectedTraits
        }

        /// <summary>
        /// To whom the effects of this option apply.
        /// </summary>
        public TraitTarget Target;

        /// <summary>
        /// The effects of this option will be applied to all characters that share at least one trait in this list. Only if <see cref="Target"/> is set to <see cref="TraitTarget.SelectedTraits"/>.
        /// </summary>
        public List<Trait> SelectedTraits;


        /// <summary>
        /// Method that is called only once when the option is activated.
        /// </summary>
        /// <param name="character">Character that activates the option.</param>
        /// <param name="trait">Trait containing this option.</param>
        protected virtual void OnActivation(GameCharacter character, Trait trait)
        {
            Debug.Log(trait.TraitName + " " + NumberOfTraits + " Activated on Character " + character.name + " | GameObject: " + character.gameObject.name);
        }

        /// <summary>
        /// Method that is called only once when the option is deactivated.
        /// </summary>
        /// <param name="character">Character that deactivates the option.</param>
        /// <param name="trait">Trait containing this option.</param>
        protected virtual void OnDeactivation(GameCharacter character, Trait trait)
        {
            Debug.Log(trait.TraitName + " " + NumberOfTraits + " Deactivated on Character " + character.name);
        }

        /// Checks if the activation condition of the options is fullfiled.
        bool ActivationCondition(GameCharacter character, Trait trait)
        {
            if (Target == TraitTarget.AllCharacters)
            {
                return true;
            }

            if (Target == TraitTarget.CharactersWithTrait)
            {
                if (character.traits.Contains(trait))
                {
                    return true;
                }
                return false;
            }

            if (Target == TraitTarget.SelectedTraits)
            {
                foreach (Trait selected in SelectedTraits)
                {
                    if (character.traits.Contains(selected))
                    {
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        /// <summary>
        /// Activate this option in a character.
        /// </summary>
        /// <param name="character">Character to which the effects of this option will be applied.</param>
        /// <param name="trait">Trait that contains this option.</param>
        public void Activate(GameCharacter character, Trait trait)
        {
            if (ActivationCondition(character, trait))
            {
                character.TraitModificators.Add(modificator);
                modificator.AddStats(character, true);
                OnActivation(character, trait);
            }
        }

        /// <summary>
        /// Dectivate this option in a character.
        /// </summary>
        /// <param name="character">Character to which the effects of this option will be removed.</param>
        /// <param name="trait">Trait that contains this option.</param>
        public void Deactivate(GameCharacter character, Trait trait)
        {
            if (character.TraitModificators.Contains(modificator))
            {
                character.TraitModificators.Remove(modificator);
                modificator.AddStats(character, false);
                OnDeactivation(character, trait);
            }
        }

    }
}