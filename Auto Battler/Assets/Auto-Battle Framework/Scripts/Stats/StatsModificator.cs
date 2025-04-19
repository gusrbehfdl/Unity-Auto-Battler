using AutoBattleFramework.BattleBehaviour.GameActors;
using AutoBattleFramework.Skills;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.Stats
{
    /// <summary>
    /// Class used by elements like <see cref="Skills.IAttackEffect"/>, <see cref="BattleBehaviour.GameActors.GameItem"/> or <see cref="Stats.Trait"/> to modify character statistics.
    /// </summary>
    [System.Serializable]
    public class StatsModificator
    {
        /// <summary>
        /// List of stats modificators to be applied.
        /// </summary>
        public List<StatModificator> statsModificator;

        /// <summary>
        /// List of AttackEffects to be attached.
        /// </summary>
        public List<IAttackEffect> attackEffects;

        /// <summary>
        /// List of on hits effects to be applied when the character attacks and hits another charater.
        /// </summary>
        public List<OnHitEffect> onHitEffects;

        /// <summary>
        /// Sprite of the modificator.
        /// </summary>
        [HideInInspector]
        public Sprite sprite;

        /// <summary>
        /// Set it to fixed if you want to add a fixed value to the statistics.
        /// Set it to percent if you want to add a percentage value to the statistics.
        /// </summary>
        public enum ModificatorType
        {
            Fixed,
            Percent
        }

        /// <summary>
        /// Add the modificator to the character stats.
        /// Setting Add to false results in the removal of the stats modification.
        /// </summary>
        /// <param name="character">Character whose statistics are to be modified.</param>
        /// <param name="Add">Set it to true if you want to add statistics. Set to false if you want to subtract them.</param>
        public void AddStats(GameCharacter character, bool Add)
        {
            foreach(StatModificator modificator in statsModificator)
            {
                switch (modificator.modificatorType)
                {
                    case ModificatorType.Fixed:
                        AddFixedStats(character, modificator, Add);
                        break;
                    case ModificatorType.Percent:
                        AddPercentStats(character, modificator, Add);
                        break;
                }
            }

        }

        //Add the fixed stats to the character
        void AddFixedStats(GameCharacter character, StatModificator modificator, bool Add)
        {
            float multiplier = 1;
            if (!Add)
                multiplier = -1;
            character.InitialStats.AddAmountToStat(modificator.stat, modificator.value * multiplier);

            character.CurrentStats.AddAmountToStat(modificator.stat, modificator.value * multiplier);
        }

        //Add the percent stats to the character
        void AddPercentStats(GameCharacter character, StatModificator modificator, bool Add)
        {
            float multiplier = 1;
            if (!Add)
                multiplier = -1;
            character.InitialStats.AddAmountToStat(modificator.stat, modificator.value * character.OriginalStats.GetStat(modificator.stat) * multiplier);

            character.CurrentStats.AddAmountToStat(modificator.stat, modificator.value * character.OriginalStats.GetStat(modificator.stat) * multiplier);
        
        }

    }
}