using AutoBattleFramework.BattleBehaviour.GameActors;
using AutoBattleFramework.Formulas;
using AutoBattleFramework.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.Skills
{
    /// <summary>
    /// When an attack hits the defender, the attacker receives a percentage proportional to the damage inflicted.
    /// </summary>
    [CreateAssetMenu(fileName = "HealthStealEffect", menuName = "Auto-Battle Framework/Effects/OnHitEffect/HealthStealEffect", order = 4)]
    public class HealthStealEffect : OnHitEffect
    {
        /// <summary>
        /// Ratio of life points to damage inflicted that the attacker will recover.
        /// </summary>
        public float percentage = 0.2f;

        /// <summary>
        /// Color of the heal popup
        /// </summary>
        public Color healColor = Color.green;

        /// <summary>
        /// When an attack hits the defender, the attacker receives a percentage proportional to the damage inflicted.
        /// </summary>
        /// <param name="defender">Defending GameCharacter. In this case it is not necessary.</param>
        /// <param name="attacker">Attacking GameCharacter that will recover life points.</param>
        /// <param name="damage">Damage that has been infringed.</param>
        public override void OnHit(GameCharacter defender, GameCharacter attacker, float damage)
        {
            float recover = damage * percentage;
            BattleFormulas.RecieveDamage(attacker, -recover, BattleFormulas.DamageType.Effect, healColor);
        }
    }
}