using AutoBattleFramework.BattleBehaviour.GameActors;
using AutoBattleFramework.Skills;
using AutoBattleFramework.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.Formulas
{
    /// <summary>
    /// Class in charge of calculating the damage between an attacking and a defending <see cref="BattleBehaviour.GameActors.GameCharacter"/>, and of reducing the life of the latter.
    /// </summary>
    public class BattleFormulas
    {
        /// <summary>
        /// Type of damage. Specific statistics are applied in the damage formula depending on the type of damage.
        /// </summary>
        public enum DamageType
        {
            Physical,
            Magic,
            Effect
        };

        /// <summary>
        /// Calculates the damage between the <see cref="Stats.CharacterStats"/> of an attacking and a defending <see cref="BattleBehaviour.GameActors.GameCharacter"/>.<br />
        /// <see cref="BattleBehaviour.GameActors.GameCharacter.CurrentStats"/> should be entered as parameters in most cases.
        /// </summary>
        /// <param name="damageType">Type of the infringed damage.</param>
        /// <param name="defending">Character who will recieve damage.</param>
        /// <param name="attacking">Character who makes damage.</param>
        protected static float CalculateDamage(DamageType damageType, CharacterStats defending, CharacterStats attacking)
        {
            switch (damageType)
            {
                case DamageType.Physical:
                    if (defending.Defense >= 0)
                        return attacking.Damage * (100 / (100 + defending.Defense));
                    else
                        return 2 - attacking.Damage * (100 / (100 + defending.Defense));

                case DamageType.Magic:
                    if (defending.MagicDefense >= 0)
                        return attacking.MagicDamage * (100 / (100 + defending.MagicDefense));
                    else
                        return 2 - attacking.MagicDamage * (100 / (100 + defending.MagicDefense));
            }
            return 0;
        }

        /// <summary>
        /// Calculates the damage between the <see cref="Stats.CharacterStats"/> of an attacking and a defending <see cref="BattleBehaviour.GameActors.GameCharacter"/>, given a pre-calculated damage (usually from a special ability).<br />
        /// <see cref="BattleBehaviour.GameActors.GameCharacter.CurrentStats"/> should be entered as parameters in most cases.
        /// </summary>
        /// <param name="damageType">Type of the infringed damage.</param>
        /// <param name="damage">Amount of pre-calculatedd damage.</param>
        /// <param name="defending">Character who will recieve damage.</param>
        /// <param name="attacking">Character who makes damage.</param>
        protected static float CalculateSpecialDamage(DamageType damageType, float damage, CharacterStats defending, CharacterStats attacking)
        {
            switch (damageType)
            {
                case DamageType.Physical:
                    if (defending.Defense >= 0)
                        return damage * (100 / (100 + defending.Defense));
                    else
                        return 2 - damage * (100 / (100 + defending.Defense));

                case DamageType.Magic:
                    if (defending.MagicDefense >= 0)
                        return damage * (100 / (100 + defending.MagicDefense));
                    else
                        return 2 - damage * (100 / (100 + defending.MagicDefense));
            }
            return 0;
        }

        /// <summary>
        /// Calculates the normal attack damage between the <see cref="Stats.CharacterStats"/> of an attacking and a defending <see cref="BattleBehaviour.GameActors.GameCharacter"/>.<br />
        /// Check if the damage has been critical (based on <see cref="Stats.CharacterStats.CriticalProbability"/>) and apply the critical damage bonus (based on <see cref="Stats.CharacterStats.CriticalDamage"/>) accordingly.<br />
        /// Applies  <see cref="Skills.OnHitEffect"/> of <see cref="Skills.IAttackEffect.OnHitEffects"/>, equipped <see cref="BattleBehaviour.GameActors.GameItem"/> (<see cref="BattleBehaviour.GameActors.GameCharacter.itemModificators"/>), <see cref="BattleBehaviour.GameActors.GameCharacter.TraitModificators"/> and active buffs (<see cref="BattleBehaviour.GameActors.GameCharacter.BuffList"/>).<br />
        /// Subtract the calculated damage from the defender's remaining <see cref="Stats.CharacterStats.Health"/>.<br />
        /// Adds <see cref="Stats.CharacterStats.Energy"/> to the attacking (based on <see cref="Stats.CharacterStats.EnergyRecoveryPerAttack"/>) and defending (based on <see cref="Stats.CharacterStats.EnergyRecoveryOnHit"/>) <see cref="BattleBehaviour.GameActors.GameCharacter"/>.<br />
        /// <see cref="BattleBehaviour.GameActors.GameCharacter.CurrentStats"/> should be entered as parameters in most cases.
        /// </summary>
        /// <param name="damageType">Type of the infringed damage.</param>
        /// <param name="defending">Character who will recieve damage.</param>
        /// <param name="attacking">Character who makes damage.</param>
        /// <returns>Damage done to the defending character.</returns>
        public static float BasicAttackDamage(DamageType damageType, GameCharacter defending, GameCharacter attacking)
        {
            if (defending && attacking)
            {
                float damage = CalculateDamage(damageType, defending.CurrentStats, attacking.CurrentStats);
                if (CriticalHit(attacking.CurrentStats.CriticalProbability))
                {
                    damage *= attacking.CurrentStats.CriticalDamage;
                }

                foreach (OnHitEffect effect in attacking.attackEffect.OnHitEffects)
                {
                    effect.OnHit(defending, attacking, damage);
                }

                foreach (StatsModificator item in attacking.itemModificators)
                {
                    foreach (OnHitEffect effect in item.onHitEffects)
                    {
                        effect.OnHit(defending, attacking, damage);
                    }
                }

                foreach (StatsModificator trait in attacking.TraitModificators)
                {
                    foreach (OnHitEffect effect in trait.onHitEffects)
                    {
                        effect.OnHit(defending, attacking, damage);
                    }
                }

                foreach (BuffEffectInfo info in attacking.BuffList)
                {
                    foreach (OnHitEffect effect in info.buff.modificator.onHitEffects)
                    {
                        effect.OnHit(defending, attacking, damage);
                    }
                }

                RecieveDamage(defending, damage, damageType, Color.clear);
                AddEnergyOnAttack(damage, defending, attacking, false);

                return damage;
            }
            return 0;
        }

        /// <summary>
        /// Calculates the special attack damage between the <see cref="Stats.CharacterStats"/> of an attacking and a defending <see cref="BattleBehaviour.GameActors.GameCharacter"/>.<br />
        /// Subtract the calculated damage from the defender's remaining <see cref="Stats.CharacterStats.Health"/>.<br />
        /// /// Applies  <see cref="Skills.OnHitEffect"/> of <see cref="Skills.IAttackEffect.OnHitEffects"/>. <br />
        /// Adds <see cref="Stats.CharacterStats.Energy"/> to the defending (based on <see cref="Stats.CharacterStats.EnergyRecoveryOnHit"/>) <see cref="BattleBehaviour.GameActors.GameCharacter"/>.<br />
        /// <see cref="BattleBehaviour.GameActors.GameCharacter.CurrentStats"/> should be entered as parameters in most cases.
        /// </summary>
        /// <param name="damageType">Type of the infringed damage.</param>
        /// <param name="damage">Amount of pre-calculatedd damage.</param>
        /// <param name="defending">Character who will recieve damage.</param>
        /// <param name="attacking">Character who makes damage.</param>
        /// <returns>Damage done to the defending character.</returns>
        public static float SpecialAttackDamage(DamageType damageType, float damage, GameCharacter defending, GameCharacter attacking)
        {
            if (defending && attacking)
            {
                float dmg = CalculateSpecialDamage(damageType, damage, defending.CurrentStats, attacking.CurrentStats);

                foreach (OnHitEffect effect in attacking.SpecialAttackEffect.OnHitEffects)
                {
                    effect.OnHit(defending, attacking, damage);
                }

                RecieveDamage(defending, dmg, damageType, Color.clear);
                AddEnergyOnAttack(damage, defending, attacking, true);

                return damage;
            }
            return 0;
        }

        /// <summary>
        /// Adds <see cref="Stats.CharacterStats.Energy"/> to the attacking (based on <see cref="Stats.CharacterStats.EnergyRecoveryPerAttack"/>) and defending (based on <see cref="Stats.CharacterStats.EnergyRecoveryOnHit"/>) <see cref="BattleBehaviour.GameActors.GameCharacter"/>.<br />
        /// <see cref="BattleBehaviour.GameActors.GameCharacter.CurrentStats"/> should be entered as parameters in most cases.
        /// </summary>
        /// <param name="damage">Amount of pre-calculatedd damage.</param>
        /// <param name="defending">Character who will recieve damage.</param>
        /// <param name="attacking">Character who makes damage.</param>
        /// <param name="IsSpecial">if is special damage, only the defending one gets energy.</param>
        protected static void AddEnergyOnAttack(float damage, GameCharacter defending, GameCharacter attacking, bool IsSpecial)
        {
            if (!IsSpecial)
            {
                attacking.CurrentStats.Energy = Mathf.Clamp(attacking.CurrentStats.Energy + (int)attacking.CurrentStats.EnergyRecoveryPerAttack, 0, attacking.InitialStats.Energy);
            }
            defending.CurrentStats.Energy = Mathf.Clamp(defending.CurrentStats.Energy + (int)(defending.CurrentStats.EnergyRecoveryOnHit * damage), 0, defending.InitialStats.Energy);
        }

        /// <summary>
        /// Returns true if an attack has been critical, given a probability (<see cref="Stats.CharacterStats.CriticalProbability"/> in most cases).
        /// </summary>
        /// <param name="probability">Probability of attack being critical.</param>
        protected static bool CriticalHit(float probability)
        {
            float randValue = Random.value;
            return randValue < probability;
        }

        /// <summary>
        /// Subtracts <see cref="Stats.CharacterStats.Health"/> from a <see cref="BattleBehaviour.GameActors.GameCharacter"/> and creates a popup with the damage received.
        /// </summary>
        /// <param name="character">Character who will recieve damage.</param>
        /// <param name="damage">Fixed amount of damage.</param>
        /// <param name="damageType">Type of damage being inflicted.</param>
        /// <param name="damageColor">Color of popup text.</param>
        public static void RecieveDamage(GameCharacter character, float damage, DamageType damageType, Color damageColor)
        {
            if (character)
            {
                character.CurrentStats.Health = Mathf.Clamp(character.CurrentStats.Health - (int)damage, 0, character.InitialStats.Health);
            }
            if(damageColor == Color.clear)
            {
                damageColor = character.GetDamageColor(damageType);
            }
            string healing = string.Empty;
            if (damage < 0)
            {
                healing += "+";
            }
            if (character)
            {
                character.CreateDamagePopup(healing + ((int)Mathf.Abs(damage)).ToString(), damageColor);
            }
        }

    }
}