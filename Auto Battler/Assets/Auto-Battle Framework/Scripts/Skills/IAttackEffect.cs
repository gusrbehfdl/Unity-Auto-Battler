using AutoBattleFramework.BattleBehaviour.GameActors;
using AutoBattleFramework.Formulas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.Skills
{
    /// <summary>
    /// Represents the effects of an attack, including damage, sound effects and visual effects.
    /// </summary>
    public abstract class IAttackEffect : ScriptableObject
    {
        /// <summary>
        /// Attacking GameCharacter.
        /// </summary>
        protected GameCharacter ai;

        /// <summary>
        /// Image depicting the attack. Used in the description of the character's statistics.
        /// </summary>
        public Sprite EffectImage;

        /// <summary>
        /// Description of the efffect
        /// </summary>
        public string EffectDescription;

        /// <summary>
        /// If the attack uses two separated animations. For example, aim with an arrow and shoot.
        /// </summary>
        public bool DoubleAnimation;

        /// <summary>
        /// Type of damage of the attack
        /// </summary>
        public BattleFormulas.DamageType DamageType = BattleFormulas.DamageType.Physical;

        /// <summary>
        /// List of On-Hit Effects associated to the Attack Effect.
        /// </summary>
        public List<OnHitEffect> OnHitEffects;

        /// <summary>
        /// On attack method. For example, a simple melee attack will call the OnHit method, while an ranged effect should spawn a <see cref="Projectile"/>.
        /// </summary>
        /// <param name="ai">Attacking GameCharacter</param>
        /// <param name="shootingPoint">The transform from which the projectile will be launched.</param>
        public abstract void Attack(GameCharacter ai, Transform shootingPoint);

        /// <summary>
        /// On hit effect. It should call a method that subtracts life from the target, like the ones in <see cref="Formulas.BattleFormulas"/>.
        /// </summary>
        /// <param name="target">The target of the character of projectile.</param>
        public abstract void OnHit(GameCharacter target);
    }
}