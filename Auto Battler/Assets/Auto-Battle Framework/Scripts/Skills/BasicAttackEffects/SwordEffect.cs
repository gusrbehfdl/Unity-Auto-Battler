using AutoBattleFramework.BattleBehaviour.GameActors;
using AutoBattleFramework.Formulas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattleFramework.Skills
{
    /// <summary>
    /// Simple melee attack
    /// </summary>
    [CreateAssetMenu(fileName = "SwordEffect", menuName = "Auto-Battle Framework/Effects/MeleeEffect/SwordEffect", order = 4)]
    public class SwordEffect : MeleeEffect
    {
        /// <summary>
        /// On attack method. For example, a simple melee attack will call the OnHit method.
        /// </summary>
        /// <param name="ai">Attacking GameCharacter</param>
        /// <param name="shootingPoint">The transform from which the projectile will be launched. In this case it can be set to null.</param>
        public override void Attack(GameCharacter ai, Transform shootingPoint)
        {
            this.ai = ai;
            OnHit(ai.TargetEnemy);
        }

        /// <summary>
        /// Calls <see cref="Formulas.BattleFormulas.BasicAttackDamage"/>.
        /// </summary>
        /// <param name="target">The target of the character of projectile.</param>
        public override void OnHit(GameCharacter target)
        {
            if (!ai.DamageVisualOnly)
            {
                BattleFormulas.BasicAttackDamage(DamageType, ai.TargetEnemy, ai);
            }
        }
    }
}