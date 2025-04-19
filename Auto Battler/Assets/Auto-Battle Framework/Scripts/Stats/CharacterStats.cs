using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.Stats
{
    /// <summary>
    /// Character statistics.
    /// </summary>
    [System.Serializable]
    public class CharacterStats
    {
        /// <summary>
        /// Health points.
        /// </summary>
        public int Health = 500;

        /// <summary>
        /// Physical damage.
        /// </summary>
        public float Damage = 100;

        /// <summary>
        /// Magic damage.
        /// </summary>
        public float MagicDamage = 100;

        /// <summary>
        /// Physical defense.
        /// </summary>
        public float Defense = 0;

        /// <summary>
        /// Magic defense.
        /// </summary>
        public float MagicDefense = 0;

        /// <summary>
        /// Attack speed.
        /// </summary>
        public float AttackSpeed = 1f;

        /// <summary>
        /// Movement speed.
        /// </summary>
        public float MovementSpeed = 1f;

        /// <summary>
        /// Range of attacks.
        /// </summary>
        public int Range = 2;

        /// <summary>
        /// Probability of an attack being critical.
        /// </summary>
        public float CriticalProbability = 0.1f;

        /// <summary>
        /// In case of critical attack, a multiplicative bonus is applied to the damage.
        /// </summary>
        public float CriticalDamage = 1.5f;

        /// <summary>
        /// Energy needed by a character to perform a special attack. The energy is recharged with each attack of the character according to <see cref="EnergyRecoveryPerAttack"/>, and when he receives damage according to <see cref="EnergyRecoveryOnHit"/>.
        /// </summary>
        public int Energy = 50;

        /// <summary>
        /// Amount of energy that the character gains when attacking.
        /// </summary>
        public float EnergyRecoveryPerAttack = 10;

        /// <summary>
        /// A percentage of the damage received is converted into energy.
        /// </summary>
        public float EnergyRecoveryOnHit = 0.05f;

        /// <summary>
        /// Enumeration of all character stats.
        /// </summary>
        public enum CharacterStat
        {
            Health,
            Damage,
            MagicDamage,
            Defense,
            MagicDefense,
            AttackSpeed,
            MovementSpeed,
            Range,
            CriticalProbability,
            CriticalDamage,
            Energy,
            EnergyRecoveryPerAttack,
            EnergyRecoveryOnHit
        }

        /// <summary>
        /// Makes a copy of the stats.
        /// </summary>
        /// <returns>Copy of the stats.</returns>
        public CharacterStats Copy()
        {
            CharacterStats stats = new CharacterStats();
            stats.Health = Health;
            stats.Damage = Damage;
            stats.MagicDamage = MagicDamage;
            stats.Defense = Defense;
            stats.MagicDefense = MagicDefense;
            stats.AttackSpeed = AttackSpeed;
            stats.MovementSpeed = MovementSpeed;
            stats.Range = Range;
            stats.CriticalProbability = CriticalProbability;
            stats.CriticalDamage = CriticalDamage;
            stats.Energy = Energy;
            stats.EnergyRecoveryPerAttack = EnergyRecoveryPerAttack;
            stats.EnergyRecoveryOnHit = EnergyRecoveryOnHit;
            return stats;
        }

        /// <summary>
        /// Adds a fixed amount to a given stat.
        /// </summary>
        /// <param name="stat">Stat to be modified.</param>
        /// <param name="amount">Amount to be modified.</param>
        public void AddAmountToStat(CharacterStat stat, float amount)
        {
            switch (stat)
            {
                case CharacterStat.Health:
                    Health += (int)amount;
                    break;
                case CharacterStat.Damage:
                    Damage += amount;
                    break;
                case CharacterStat.MagicDamage:
                    MagicDamage += amount;
                    break;
                case CharacterStat.Defense:
                    Defense += amount;
                    break;
                case CharacterStat.MagicDefense:
                    MagicDefense += amount;
                    break;
                case CharacterStat.AttackSpeed:
                    AttackSpeed += amount;
                    break;
                case CharacterStat.MovementSpeed:
                    MovementSpeed += amount;
                    break;
                case CharacterStat.Range:
                    Range += (int)amount;
                    break;
                case CharacterStat.CriticalProbability:
                    CriticalProbability += amount;
                    break;
                case CharacterStat.CriticalDamage:
                    CriticalDamage += amount;
                    break;
                case CharacterStat.Energy:
                    Energy += (int)amount;
                    break;
                case CharacterStat.EnergyRecoveryPerAttack:
                    EnergyRecoveryPerAttack += amount;
                    break;
                case CharacterStat.EnergyRecoveryOnHit:
                    EnergyRecoveryOnHit += amount;
                    break;
            }
        }

        /// <summary>
        /// Returns the value of the stat by its enum.
        /// </summary>
        /// <param name="stat">Stat enumeration value.</param>
        /// <returns>Stat value.</returns>
        public float GetStat(CharacterStat stat)
        {
            switch (stat)
            {
                case CharacterStat.Health:
                    return Health;
                case CharacterStat.Damage:
                    return Damage;
                case CharacterStat.MagicDamage:
                    return MagicDamage;
                case CharacterStat.Defense:
                    return Defense;
                case CharacterStat.MagicDefense:
                    return MagicDefense;
                case CharacterStat.AttackSpeed:
                    return AttackSpeed;
                case CharacterStat.MovementSpeed:
                    return MovementSpeed;
                case CharacterStat.Range:
                    return Range;
                case CharacterStat.CriticalProbability:
                    return CriticalProbability;
                case CharacterStat.CriticalDamage:
                    return CriticalDamage;
                case CharacterStat.Energy:
                    return Energy;
                case CharacterStat.EnergyRecoveryPerAttack:
                    return EnergyRecoveryPerAttack;
                case CharacterStat.EnergyRecoveryOnHit:
                    return EnergyRecoveryOnHit;
            }
            return 0;
        }


        /// <summary>
        /// Set value of the stat by its enum.
        /// </summary>
        /// <param name="stat">Stat enumeration value.</param>
        public float SetStat(CharacterStat stat, float value)
        {
            switch (stat)
            {
                case CharacterStat.Health:
                    Health = (int)value;
                    return Health;
                case CharacterStat.Damage:
                    Damage = value;
                    return Damage;
                case CharacterStat.MagicDamage:
                    MagicDamage = value;
                    return MagicDamage;
                case CharacterStat.Defense:
                    Defense = value;
                    return Defense;
                case CharacterStat.MagicDefense:
                    MagicDefense = value;
                    return MagicDefense;
                case CharacterStat.AttackSpeed:
                    AttackSpeed = value;
                    return AttackSpeed;
                case CharacterStat.MovementSpeed:
                    MovementSpeed = value;
                    return MovementSpeed;
                case CharacterStat.Range:
                    Range = (int)value;
                    return Range;
                case CharacterStat.CriticalProbability:
                    CriticalProbability = value;
                    return CriticalProbability;
                case CharacterStat.CriticalDamage:
                    CriticalDamage = value;
                    return CriticalDamage;
                case CharacterStat.Energy:
                    Energy = (int)value;
                    return Energy;
                case CharacterStat.EnergyRecoveryPerAttack:
                    EnergyRecoveryPerAttack = value;
                    return EnergyRecoveryPerAttack;
                case CharacterStat.EnergyRecoveryOnHit:
                    EnergyRecoveryOnHit = value;
                    return EnergyRecoveryOnHit;
            }
            return 0;
        }
    }
}