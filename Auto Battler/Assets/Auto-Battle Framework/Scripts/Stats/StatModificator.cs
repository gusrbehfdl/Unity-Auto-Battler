using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.Stats
{
    /// <summary>
    /// Describes a modification to a stat of a character.
    /// </summary>
    [System.Serializable]
    public class StatModificator
    {
        /// <summary>
        /// Stat to modify
        /// </summary>
        public CharacterStats.CharacterStat stat;

        /// <summary>
        /// Whether the change is fixed or percentage.
        /// </summary>
        public StatsModificator.ModificatorType modificatorType;

        /// <summary>
        /// Amount of the modification.
        /// </summary>
        public float value;

        /// <summary>
        /// Returns a string that describes the state
        /// </summary>
        /// <param name="stat">Stat to be described.</param>
        /// <returns>The amount of modification as string.</returns>
        public string GetModificatorStatString(CharacterStats.CharacterStat stat)
        {
            string Modificator = "";
            var amount = value;
            if (modificatorType == StatsModificator.ModificatorType.Percent)
            {
                Modificator = "%";
                amount *= 100;
            }

            return amount.ToString() + Modificator;
        }
    }
}